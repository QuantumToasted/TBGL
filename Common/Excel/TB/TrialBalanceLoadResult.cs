using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class TrialBalanceLoadResult(string path, IXLWorksheet worksheet) : XLWorksheetLoadResult(path, worksheet)
{
    public IReadOnlyList<TrialBalanceAccount> Accounts { get; } = EnumerateRows(worksheet).ToList();

    public decimal OpeningBalanceTotal => Accounts.Sum(x => x.OpeningBalance);
    
    public decimal DebitTotal => Accounts.Sum(x => x.Debit);
    
    public decimal CreditTotal => Accounts.Sum(x => x.Credit);
    
    public decimal ClosingBalanceTotal => Accounts.Sum(x => x.ClosingBalance);

    private static IEnumerable<TrialBalanceAccount> EnumerateRows(IXLWorksheet worksheet)
    {
        foreach (var row in worksheet.Rows(9, worksheet.RowCount()))
        {
            if (row.Cell(2).IsEmpty()) // "Totals" row has nothing for account name
                yield break;

            yield return new TrialBalanceAccount(row);
        }
    }
}