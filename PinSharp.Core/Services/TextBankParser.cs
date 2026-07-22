using PinSharp.Core.Models;

namespace PinSharp.Core.Services;

public static class TextBankParser
{
    public static IReadOnlyList<BatchInputRow> Parse(string content)
    {
        var rows = new List<BatchInputRow>();
        foreach (var rawLine in content.ReplaceLineEndings("\n").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            if (!rawLine.Contains('|'))
            {
                continue;
            }

            var parts = rawLine.Split('|', 2, StringSplitOptions.TrimEntries);
            var title = parts[0];
            var code = parts[1];
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(code))
            {
                continue;
            }

            rows.Add(new BatchInputRow(title, code));
        }

        return rows;
    }
}
