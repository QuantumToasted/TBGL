using System;
using System.Linq;
using ClosedXML.Excel;
using TBGL.Services;

namespace TBGL.Extensions;

public static class ExcelExtensions
{
    public const string ACCOUNTING_CELL_FORMAT = """_(""$""* #,##0.00_);_(""$""* \(#,##0.00\);_(""$""* ""-""??_);_(@_)""";
    
    public static string? GetStringOrDefault(this IXLCell cell)
    {
        return cell.TryGetValue(out string s) && !string.IsNullOrWhiteSpace(s)
            ? s
            : null;
    }

    public static IXLRange Range(this IXLWorksheet worksheet, int row, Range columnRange)
        => worksheet.Range(row, columnRange.Start.Value, row, columnRange.End.Value);

    public static IXLRange Range(this IXLWorksheet worksheet, Range rowRange, int column)
        => worksheet.Range(rowRange.Start.Value, column, rowRange.End.Value, column);

    public static IXLCell SetCurrencyFormat(this IXLCell cell)
    {
        cell.Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
        return cell;
    }

    public static IXLRange SetCurrencyFormat(this IXLRange range)
    {
        range.Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
        return range;
    }

    public static IXLCell SetCurrencyValue(this IXLCell cell, decimal? value)
    {
        cell.SetValue(value.GetValueOrDefault()).Style.NumberFormat.SetFormat(ACCOUNTING_CELL_FORMAT);
        return cell;
    }

    public static IXLCell SetTextValue(this IXLCell cell, string? value)
    {
        cell.SetValue(value ?? string.Empty).Style.NumberFormat
            .SetNumberFormatId((int)XLPredefinedFormat.Number.Text);

        return cell;
    }
    
    public static IXLRange FillValues(this IXLWorksheet worksheet, Range rowRange, Range columnRange, params XLCellValue[] values)
    {
        var range = worksheet.Range(rowRange.Start.Value, columnRange.Start.Value, rowRange.End.Value, columnRange.End.Value);
        return range.FillValues(values);
    }
    
    public static IXLRange FillValues(this IXLWorksheet worksheet, Range rowRange, int column, params XLCellValue[] values)
    {
        var range = worksheet.Range(rowRange.Start.Value, column, rowRange.End.Value, column);
        return range.FillValues(values);
    }

    public static IXLRange FillValues(this IXLWorksheet worksheet, int row, Range columnRange, params XLCellValue[] values)
    {
        var range = worksheet.Range(row, columnRange.Start.Value, row, columnRange.End.Value);
        return range.FillValues(values);
    }

    public static IXLRange FillValues(this IXLRange range, params XLCellValue[] values)
    {
        var cells = range.Cells().ToList();
        if (cells.Count != values.Length)
            throw new ArgumentException($"Expected {cells.Count} values, got {values.Length}.");

        for (var i = 0; i < cells.Count; i++)
        {
            cells[i].SetValue(values[i]);
        }

        return range;
    }
}