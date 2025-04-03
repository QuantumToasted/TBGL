using System;
using System.Diagnostics.CodeAnalysis;
using ClosedXML.Excel;
using TBGL.Extensions;

namespace TBGL.Common;

public sealed record GeneralLedgerTransaction(
    DateOnly? PostedDate = null,
    DateOnly? DocDate = null,
    string? DocId = null,
    string? Memo = null,
    string? Department = null,
    string? Location = null,
    string? Unit = null,
    string? Journal = null,
    decimal? Debit = null,
    decimal? Credit = null)
{
    public static GeneralLedgerTransaction Empty => new();
    
    public static GeneralLedgerTransaction FromRow(IXLRangeRow row)
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
}