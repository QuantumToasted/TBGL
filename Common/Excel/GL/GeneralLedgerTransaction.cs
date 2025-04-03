using System;
using System.Diagnostics.CodeAnalysis;
using ClosedXML.Excel;
using TBGL.Extensions;

namespace TBGL.Common;

public sealed record GeneralLedgerTransaction(
    DateOnly PostedDate,
    DateOnly DocDate,
    string? DocId,
    string? Memo,
    string? Department,
    string Location,
    string? Unit,
    string Journal,
    decimal? Debit,
    decimal? Credit)
{
    public static GeneralLedgerTransaction Parse(IXLRangeRow row)
    {
        return new(
            DateOnly.Parse(row.Cell(1).GetString()),
            DateOnly.Parse(row.Cell(2).GetString()),
            row.Cell(3).GetStringOrDefault(),
            row.Cell(4).GetStringOrDefault(),
            row.Cell(5).GetStringOrDefault(),
            row.Cell(6).GetValue<string>(),
            row.Cell(7).GetStringOrDefault(),
            row.Cell(8).GetString(),
            row.Cell(9).TryGetValue(out decimal debit) ? debit : null,
            row.Cell(10).TryGetValue(out decimal credit) ? credit : null);
    }

    public static bool TryParse(IXLRangeRow row, [NotNullWhen(true)] out GeneralLedgerTransaction? transaction)
    {
        try
        {
            transaction = Parse(row);
            return true;
        }
        catch
        {
            transaction = null;
            return false;
        }
    }
}