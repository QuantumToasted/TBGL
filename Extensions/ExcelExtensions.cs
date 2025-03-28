using ClosedXML.Excel;

namespace TBGL.Extensions;

public static class ExcelExtensions
{
    public static string? GetStringOrDefault(this IXLCell cell)
    {
        return cell.TryGetValue(out string s) && !string.IsNullOrWhiteSpace(s)
            ? s
            : null;
    }
}