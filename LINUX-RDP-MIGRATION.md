Pin Creator Linux RDP Migration

Current state

- The app is a WPF desktop app targeting `net8.0-windows`.
- WPF does not run on Linux, including Linux RDP desktops.
- The batch rendering logic is usable as a base, but the UI layer must change for Linux.

Best migration path

1. Keep the current Windows app for now.
2. Move batch logic into a shared cross-platform library.
3. Replace the WPF frontend with one of these:
   - Avalonia UI for a desktop app on Linux RDP
   - ASP.NET Core web UI for browser access from Linux RDP
   - Console/worker service for fully headless batch processing
4. Replace Windows-only rendering pieces if needed with cross-platform graphics such as SkiaSharp or ImageSharp.Drawing.

Recommended next step

- Build a shared `PinCreator.Core` project for:
  - input parsing
  - queue pairing
  - layout selection
  - batch export orchestration
- Then choose either Avalonia or a web dashboard for Linux.

Fastest option

- If you want the quickest Linux move, a web dashboard is usually faster than rebuilding a rich desktop UI.
- If you want a native desktop feel on Linux RDP, Avalonia is the better path.
