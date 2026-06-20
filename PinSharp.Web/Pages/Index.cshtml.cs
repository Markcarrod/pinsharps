using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PinSharp.Core.Models;
using PinSharp.Core.Services;

namespace PinSharp.Web.Pages;

public class IndexModel : PageModel
{
    private readonly BatchRenderService _batchRenderService;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(BatchRenderService batchRenderService, IWebHostEnvironment environment)
    {
        _batchRenderService = batchRenderService;
        _environment = environment;
    }

    [BindProperty]
    public List<IFormFile> Images { get; set; } = [];

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

    public IReadOnlyList<PinSize> Sizes => PinSize.Presets;

    public IReadOnlyList<int> ThreadOptions => Enumerable.Range(1, Math.Max(1, Environment.ProcessorCount)).ToArray();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Images.Count == 0 || InputFile is null || InputFile.Length == 0)
        {
            ErrorMessage = "Upload images and an input.txt file before running the batch.";
            return Page();
        }

        var format = Format.Equals("jpg", StringComparison.OrdinalIgnoreCase) ? "jpg" : "png";
        var size = PinSize.FromId(SizeId);
        var quality = Math.Clamp(JpegQuality, 1, 100);
        var threads = Math.Clamp(ThreadCount, 1, Math.Max(1, Environment.ProcessorCount));
        var jobId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];
        var jobRoot = Path.Combine(_environment.WebRootPath, "runs", jobId);
        var sourceFolder = Path.Combine(jobRoot, "source");
        var outputFolder = Path.Combine(jobRoot, "output");
        Directory.CreateDirectory(sourceFolder);
        Directory.CreateDirectory(outputFolder);

        try
        {
            var imagePaths = new List<string>();
            foreach (var image in Images)
            {
                var extension = Path.GetExtension(image.FileName);
                var safeName = Path.GetFileNameWithoutExtension(image.FileName);
                var filePath = Path.Combine(sourceFolder, $"{safeName}{extension}");
                await using var stream = System.IO.File.Create(filePath);
                await image.CopyToAsync(stream, cancellationToken);
                imagePaths.Add(filePath);
            }

            string inputContent;
            await using (var stream = InputFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                inputContent = await reader.ReadToEndAsync(cancellationToken);
            }

            var rows = TextBankParser.Parse(inputContent);
            var zipPath = Path.Combine(jobRoot, "batch-output.zip");
            Summary = await _batchRenderService.RenderAsync(
                jobId,
                imagePaths,
                rows,
                new BatchRenderOptions(size, format, quality, threads),
                outputFolder,
                zipPath,
                cancellationToken);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return Page();
    }
}
