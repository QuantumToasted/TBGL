using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace TBGL.Common;

public sealed partial record GeneralLedgerAccountMetadata(string Category, string SubCategory, string Name)
{
    private static readonly Regex BalanceForwardRegex = MyRegex();

    public override string ToString()
        => $"{Category}-{SubCategory}: {Name}";

    // example input: "1001-0000 - Example 1-0 (Balance Forward As of 01/01/2025)"
    public static GeneralLedgerAccountMetadata Parse(string text)
    {
        text = BalanceForwardRegex.Replace(text, string.Empty);
        
        // example result: "1001-0000 - Example 1-0"
        var split = text.Split(" - ");

        var categorySplit = split[0].Split('-');
        var name = split[1];
        return new GeneralLedgerAccountMetadata(categorySplit[0], categorySplit[1], name);
    }

    public static bool TryParse(string text, [NotNullWhen(true)] out GeneralLedgerAccountMetadata? metadata)
    {
        try
        {
            metadata = Parse(text);
            return true;
        }
        catch
        {
            metadata = null;
            return false;
        }
    }

    [GeneratedRegex(@" \(Balance Forward As of [\d\/]+\)")]
    private static partial Regex MyRegex();
}