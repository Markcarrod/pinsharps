using SkiaSharp;

namespace PinSharp.Core.Services;

public static class FontResolver
{
    private static readonly string[] PreferredFonts =
    [
        "DejaVu Sans",
        "Arial",
        "Liberation Sans",
        "Noto Sans",
        "Segoe UI"
    ];

    public static SKTypeface Resolve(string preferredFamily, bool bold)
    {
        var weight = bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal;
        var preferred = SKTypeface.FromFamilyName(preferredFamily, weight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
        if (preferred is not null)
        {
            return preferred;
        }

        foreach (var family in PreferredFonts)
        {
            var candidate = SKTypeface.FromFamilyName(family, weight, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
            if (candidate is not null)
            {
                return candidate;
            }
        }

        return SKTypeface.Default;
    }
}
