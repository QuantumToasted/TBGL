using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosedXML.Excel;

namespace TBGL.Common;

public class GeneralLedgerTransactionHistory(IXLRange range)
{
    public GeneralLedgerAccountMetadata Metadata { get; } = GeneralLedgerAccountMetadata.Parse(range.FirstCell().GetString());

    public decimal StartingBalance { get; } = range.FirstRow().LastCell().GetValue<decimal>();

    public decimal EndingBalance { get; } = range.LastRow().LastCell().GetValue<decimal>();

    public IEnumerable<GeneralLedgerTransaction> EnumerateTransactions(bool includeBalances = true)
    {
        var balance = StartingBalance;
        
        if (includeBalances)
            yield return GeneralLedgerTransaction.MemoOnly("Balance Forward", balance);

        foreach (var row in range.Rows(2, range.RowCount() - 1))
        {
            var transaction = GeneralLedgerTransaction.FromRow(row, balance);
            yield return transaction;
            balance = transaction.EndingBalance;
        }
        
        if (balance != EndingBalance)
            throw new Exception("Balance calculation failed.");
        
        if (includeBalances)
            yield return GeneralLedgerTransaction.MemoOnly("Ending Balance", balance);
    }
}