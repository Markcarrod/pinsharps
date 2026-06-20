using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PinSharp.Core.Models;
using PinSharp.Core.Services;

namespace PinSharp.Web.Pages;

public class IndexModel : PageModel
{
    private static readonly HashSet<string> SupportedImageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".bmp", ".webp" };

    private readonly BatchRenderService _batchRenderService;

    public IndexModel(BatchRenderService batchRenderService)
    {
        _batchRenderService = batchRenderService;
    }

    [BindProperty]
    public string ImageFolder { get; set; } = string.Empty;

    [BindProperty]
    public string OutputFolder { get; set; } = string.Empty;

    [BindProperty]
    public string FontFilePath { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? InputFile { get; set; }

    [BindProperty]
    public string SizeId { get; set; } = PinSize.Presets[0].Id;

    [BindProperty]
    public string Format { get; set; } = "png";

    [BindProperty]
    public int JpegQuality { get; set; } = 90;

    [BindProperty]
    public int ThreadCount { get; set; } = Math.Max(1, Environment.ProcessorCount / 2);

    public BatchRenderSummary? Summary { get; private set; }

    public string? ErrorMessage { get; private set; }

    public string? CompletedOutputFolder { get; private set; }

    public string? CompletedZipPath { get; private set; }

    public IReadOnlyList<PinSize> Sizes => PinSize.Presets;

    public IReadOnlyList<int> ThreadOptions => Enumerable.Range(1, Math.Max(1, Environment.ProcessorCount)).ToArray();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ImageFolder) || string.IsNullOrWhiteSpace(OutputFolder) ||
            InputFile is null || InputFile.Length == 0)
        {
            ErrorMessage = "Enter the image and output folder paths, then select an input.txt file.";
            return Page();
        }

        try
        {
            var imageFolder = Path.GetFullPath(ImageFolder.Trim());
            var outputRoot = Path.GetFullPath(OutputFolder.Trim());
            if (!Directory.Exists(imageFolder))
            {
                throw new DirectoryNotFoundException($"Image folder not found: {imageFolder}");
            }

            if (imageFolder.Equals(outputRoot, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The image and output folders must be different.");
            }

            string inputContent;
            await using (var stream = InputFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                inputContent = await reader.ReadToEndAsync(cancellationToken);
            }

            var rows = TextBankParser.Parse(inputContent);
            if (rows.Count == 0)
            {
                throw new InvalidOperationException("The input file has no valid title|code rows.");
            }

            var imagePaths = SelectImages(imageFolder, rows.Count);
            var fontFilePath = ResolveFontFile(FontFilePath);
            var format = Format.Equals("jpg", StringComparison.OrdinalIgnoreCase) ? "jpg" : "png";
            var size = PinSize.FromId(SizeId);
            var quality = Math.Clamp(JpegQuality, 1, 100);
            var threads = Math.Clamp(ThreadCount, 1, Math.Max(1, Environment.ProcessorCount));
            var jobId = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];
            var outputFolder = Path.Combine(outputRoot, $"pinsharp-{jobId}");
            var zipPath = Path.Combine(outputRoot, $"pinsharp-{jobId}.zip");
            Directory.CreateDirectory(outputRoot);

            Summary = await _batchRenderService.RenderAsync(
                jobId,
                imagePaths,
                rows,
                new BatchRenderOptions(size, format, quality, threads, fontFilePath),
                outputFolder,
                zipPath,
                cancellationToken);
            CompletedOutputFolder = outputFolder;
            CompletedZipPath = zipPath;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return Page();
    }

    private static IReadOnlyList<string> SelectImages(string folder, int requiredCount)
    {
        var selected = new List<string>(requiredCount);
        foreach (var path in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
        {
            if (!SupportedImageExtensions.Contains(Path.GetExtension(path)))
            {
                continue;
            }

            selected.Add(path);
            if (selected.Count == requiredCount)
            {
                break;
            }
        }

        if (selected.Count == 0)
        {
            throw new InvalidOperationException("No supported images were found in the image folder.");
        }

        var availableCount = selected.Count;
        while (selected.Count < requiredCount)
        {
            selected.Add(selected[selected.Count % availableCount]);
        }

        return selected;
    }

    private static string? ResolveFontFile(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var path = Path.GetFullPath(value.Trim());
        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension is not (".ttf" or ".otf" or ".ttc") || !System.IO.File.Exists(path))
        {
            throw new InvalidOperationException($"Font file not found or unsupported: {path}");
        }

        return path;
    }
}
