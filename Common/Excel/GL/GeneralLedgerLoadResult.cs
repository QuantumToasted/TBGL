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
        var max = worksheet.LastRowUsed()!.RowNumber() + 1;
        for (var i = 8; i <= max; i++)
        {
            var firstCell = worksheet.Row(i).FirstCell().TryGetValue(out string s) ? s : string.Empty;
            if (historyStart == -1)
            {
                if (GeneralLedgerAccountMetadata.TryParse(firstCell, out _))
                {
                    historyStart = i;
                }
            }
            else if (worksheet.Row(i).IsEmpty())
            {
                var range = worksheet.Range(historyStart, 1, i - 1, 11);
                
                yield return new GeneralLedgerTransactionHistory(range);
                
                historyStart = -1;
            }
        }
    }
}