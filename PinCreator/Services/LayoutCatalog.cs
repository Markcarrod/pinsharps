using System.Windows;
using PinCreator.Models;

namespace PinCreator.Services;

public static class LayoutCatalog
{
    public static IReadOnlyList<LayoutDefinition> All { get; } =
    [
        new("top-sheet", "Top Story", "Clean editorial sheet across the upper third", LayoutKind.TopSheet, "#E96B4C", "#F8F4EC", "#171A1F", "#5E646D", "Bahnschrift", FontWeights.Bold),
        new("top-center", "Centered Header", "Balanced centered headline with generous breathing room", LayoutKind.TopCenter, "#DDAF54", "#FFF9EE", "#171A1F", "#66615A", "Georgia", FontWeights.Bold),
        new("center-card", "Gallery Card", "High-contrast floating card for busy photography", LayoutKind.CenterCard, "#F06449", "#FFFDF8", "#17212B", "#626A70", "Bahnschrift", FontWeights.Bold),
        new("lower-card", "Bottom Brief", "Structured lower card with publication styling", LayoutKind.LowerCard, "#0E7C70", "#F5F0E7", "#17212B", "#5D666C", "Georgia", FontWeights.Bold),
        new("left-column", "Left Editorial", "Magazine column with strong reading rhythm", LayoutKind.LeftColumn, "#BE4F38", "#F4EBDD", "#251C18", "#6D5B50", "Georgia", FontWeights.Bold),
        new("right-column", "Right Editorial", "Image-led composition with a right-side story block", LayoutKind.RightColumn, "#D7A53A", "#12191F", "#FFF8E9", "#C9C2B6", "Bahnschrift", FontWeights.Bold, true),
        new("gradient-bottom", "Afterglow", "Soft cinematic fade with luminous lower text", LayoutKind.GradientBottom, "#F2B84B", "#121820", "#FFFFFF", "#DFE3E6", "Georgia", FontWeights.Bold, true),
        new("soft-panel", "Soft Focus", "Rounded translucent panel for lifestyle content", LayoutKind.SoftPanel, "#E56B55", "#F9F5ED", "#182129", "#62686D", "Bahnschrift", FontWeights.Bold),
        new("full-veil", "Quiet Veil", "Subtle wash and oversized centered typography", LayoutKind.FullVeil, "#B94E39", "#F4EFE5", "#1A1D21", "#555C62", "Georgia", FontWeights.Bold),
        new("border-frame", "Framed Minimal", "Gallery border and disciplined typographic lockup", LayoutKind.BorderFrame, "#F0B44A", "#10171D", "#FFFFFF", "#D6D9DC", "Bahnschrift", FontWeights.Bold, true),
        new("split-editorial", "Split Edition", "Bold split canvas for products and how-to stories", LayoutKind.SplitEditorial, "#E45C3E", "#F3EBDD", "#1B2228", "#5F6568", "Bahnschrift", FontWeights.Bold),
        new("quote-focus", "Quote Focus", "Expressive serif quote treatment with attribution space", LayoutKind.QuoteFocus, "#E36D50", "#FFF9EF", "#211D1A", "#69605A", "Georgia", FontWeights.Bold),
        new("diagonal", "Diagonal Energy", "Angled color field for punchy statements", LayoutKind.DiagonalStatement, "#EB6042", "#101920", "#FFFFFF", "#D8DFE2", "Bahnschrift", FontWeights.Bold, true),
        new("minimal-poster", "Type Poster", "Typography-first poster with a restrained image window", LayoutKind.MinimalPoster, "#DB543C", "#F1E8D8", "#192128", "#65615B", "Bahnschrift", FontWeights.Bold),
        new("cinematic", "Cinematic", "Wide-screen image treatment with film-title hierarchy", LayoutKind.Cinematic, "#E9B949", "#0D141A", "#FFFFFF", "#CDD2D5", "Georgia", FontWeights.Bold, true)
    ];
}
