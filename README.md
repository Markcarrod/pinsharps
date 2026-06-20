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

1. Enter the Linux path to the source image folder; images are read directly without upload.
2. Upload `input.txt` with one `title|code` line per row.
3. Enter an output folder and choose canvas, format, JPG quality, and thread count.
4. Optionally enter a font folder such as `/home/kayan/fonts` and use `random` or a partial name like `monst`.
5. Run the batch.
6. Find the generated images and ZIP in the selected output folder.

Font folders are scanned recursively, so `/home/kayan/fonts/Monse/monst.ttf` works when the page has:

```text
Font folder: /home/kayan/fonts
Font name: monst
```

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
- Each batch is written to a timestamped directory under the selected output folder.
- The archived `PinCreator` WPF project is not part of `PinCreator.sln`.
