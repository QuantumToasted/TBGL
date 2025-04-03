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

    public IReadOnlyList<GeneralLedgerTransaction> Transactions { get; } = range.Rows(2, range.RowCount() - 1).Select(GeneralLedgerTransaction.FromRow).ToList();

    public void Validate()
    {
        var balance = StartingBalance;
        foreach (var transaction in Transactions)
        {
            balance = balance + transaction.Debit.GetValueOrDefault() - transaction.Credit.GetValueOrDefault();
        }

        if (balance != EndingBalance)
            throw new Exception($"Account {Metadata} ending balance should be {EndingBalance:C}, was {balance:C}");
    }
}