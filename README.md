# PinSharp

Cross-platform batch pin generator built with `ASP.NET Core` and `SkiaSharp`.

## Projects

- `PinSharp.Web`: browser dashboard for Linux RDP, VPS, or local use
- `PinSharp.Core`: shared batch parsing and rendering engine
- `PinCreator`: original WPF desktop app kept for Windows

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

```powershell
dotnet build PinCreator.sln -c Release
```

## Notes

- The web app is the recommended Linux path.
- Rendered browser batches are written under `PinSharp.Web/wwwroot/runs/`.
- `PinCreator` remains available if you still want the native Windows app.
