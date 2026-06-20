using System.IO.Compression;
using System.Collections.Concurrent;
using PinSharp.Core.Models;

namespace PinSharp.Core.Services;

public sealed class BatchRenderService
{
    private readonly PinImageRenderer _renderer = new();

    public async Task<BatchRenderSummary> RenderAsync(
        string jobId,
        IReadOnlyList<string> imagePaths,
        IReadOnlyList<BatchInputRow> inputRows,
        BatchRenderOptions options,
        string outputDirectory,
        string zipPath,
        CancellationToken cancellationToken = default)
    {
        if (imagePaths.Count == 0)
        {
            throw new InvalidOperationException("No source images were selected.");
        }

        if (inputRows.Count == 0)
        {
            throw new InvalidOperationException("Upload an input file with title|code rows.");
        }

        var pairedCount = Math.Min(imagePaths.Count, inputRows.Count);
        var duplicateCode = inputRows
            .Take(pairedCount)
            .GroupBy(row => row.Code, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => string.IsNullOrWhiteSpace(group.Key) || group.Count() > 1);

        if (duplicateCode is not null)
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(duplicateCode.Key)
                ? "Every input row must include both title and code."
                : $"The code '{duplicateCode.Key}' appears more than once.");
        }

        Directory.CreateDirectory(outputDirectory);
        var logPath = Path.Combine(outputDirectory, "pinsharp-run.log");
        await File.AppendAllTextAsync(logPath, $"[{DateTimeOffset.Now:O}] Starting {pairedCount} pins with {options.ThreadCount} threads.{Environment.NewLine}", cancellationToken);

        var items = Enumerable.Range(0, pairedCount)
            .Select(index => new BatchRenderItem(imagePaths[index], inputRows[index].Title, inputRows[index].Code, index))
            .ToArray();

        var results = new RenderedPinResult[items.Length];
        var failures = new ConcurrentBag<string>();
        await Parallel.ForEachAsync(items, new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, options.ThreadCount),
            CancellationToken = cancellationToken
        }, async (item, token) =>
        {
            token.ThrowIfCancellationRequested();
            try
            {
                var layout = SelectLayout(item);
                var fileName = SafeFileName(item.Code) + "." + options.Format.ToLowerInvariant();
                var outputPath = Path.Combine(outputDirectory, fileName);
                await _renderer.RenderToFileAsync(item.ImagePath, item.Title, outputPath, options, layout, token);
                results[item.Index] = new RenderedPinResult(item.Title, item.Code, fileName, fileName, layout.Kind);
            }
            catch (Exception ex)
            {
                failures.Add($"{item.Code}: {Path.GetFileName(item.ImagePath)} - {ex.GetType().Name}: {ex.Message}");
            }
        });

        if (!failures.IsEmpty)
        {
            await File.AppendAllLinesAsync(logPath, failures.OrderBy(line => line, StringComparer.OrdinalIgnoreCase), cancellationToken);
        }

        var completed = results.OfType<RenderedPinResult>().ToArray();
        if (completed.Length == 0)
        {
            var sample = failures.Take(5).ToArray();
            throw new InvalidOperationException("No pins rendered. Check pinsharp-run.log in the output folder. " + string.Join(" | ", sample));
        }

        var zipName = string.Empty;
        if (options.CreateZip)
        {
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(outputDirectory, zipPath);
            zipName = Path.GetFileName(zipPath);
        }

        await File.AppendAllTextAsync(logPath, $"[{DateTimeOffset.Now:O}] Completed {completed.Length}/{pairedCount} pins. Failed: {failures.Count}.{Environment.NewLine}", cancellationToken);

        return new BatchRenderSummary(
            jobId,
            imagePaths.Count,
            inputRows.Count,
            completed.Length,
            options.ThreadCount,
            zipName,
            completed);
    }

    private static LayoutDefinition SelectLayout(BatchRenderItem item)
    {
        var wordCount = item.Title.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        var choices = wordCount > 8
            ? LayoutCatalog.All.Where(layout => layout.Kind is not LayoutKind.SideSplit).ToArray()
            : LayoutCatalog.All.ToArray();
        return choices[item.Index % choices.Length];
    }

    private static string SafeFileName(string value)
    {
        var cleaned = string.Join("-", value.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
            .Trim()
            .Replace(' ', '-');
        return string.IsNullOrWhiteSpace(cleaned) ? "pin" : cleaned[..Math.Min(80, cleaned.Length)];
    }
}
