# PinSharp

Cross-platform .NET 10 batch pin generator built with `ASP.NET Core` and `SkiaSharp`.

## Projects

- `PinSharp.Web`: browser dashboard for Linux RDP, VPS, or local use
- `PinSharp.Core`: shared batch parsing and rendering engine
- `PinCreator`: archived Windows frontend, excluded from the Linux solution build

## Web Run

```powershell
dotnet run --project PinSharp.Web -c Release --urls http://127.0.0.1:5099
```

Open:

```text
http://127.0.0.1:5099
```

## Web Flow

1. Upload multiple images.
2. Upload `input.txt` with one `title|code` line per row.
3. Choose canvas, format, JPG quality, and thread count.
4. Run the batch.
5. Preview outputs and download the zip.

## Build

```bash
dotnet build PinCreator.sln -c Release
```

The solution contains only the Linux-compatible web application and its core dependency. Run it with:

```bash
dotnet run --project PinSharp.Web/PinSharp.Web.csproj -c Release --urls http://0.0.0.0:5099
```

## Notes

- The web app is the recommended Linux path.
- .NET 10 SDK and ASP.NET Core 10 runtime are required.
- Rendered browser batches are written under `PinSharp.Web/wwwroot/runs/`.
- The archived `PinCreator` WPF project is not part of `PinCreator.sln`.
