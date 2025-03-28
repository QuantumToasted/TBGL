using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class GeneralLedgerLoadResult(string path, IXLWorksheet worksheet) : XLWorksheetLoadResult(path, worksheet)
{
    public IReadOnlyList<GeneralLedgerTransactionHistory> TransactionHistories { get; } = EnumerateHistories(worksheet).ToList();
    
    public static IEnumerable<GeneralLedgerTransactionHistory> EnumerateHistories(IXLWorksheet worksheet)
    {
        var historyStart = -1;
        var max = worksheet.LastRowUsed()!.RowNumber();
        for (var i = 8; i < max; i++)
        {
            if (historyStart == -1 && 
                worksheet.Row(i).FirstCell().TryGetValue(out string s) &&
                GeneralLedgerAccountMetadata.TryParse(s, out _))
            {
                historyStart = i;
            }
            
            if (historyStart != -1 && worksheet.Row(i).IsEmpty())
            {
                var range = worksheet.Range(historyStart, 1, i - 1, 11);
                historyStart = -1;
                
                yield return new GeneralLedgerTransactionHistory(range);
            }
        }
    }
}