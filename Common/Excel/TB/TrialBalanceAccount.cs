using ClosedXML.Excel;

namespace TBGL.Common;

public sealed class TrialBalanceAccount(IXLRow row)
{
    public AccountMetadata Metadata { get; } = AccountMetadata.FromRaw(row.Cell(1).GetString(), row.Cell(2).GetString());

    public decimal OpeningBalance { get; } = row.Cell(3).GetValue<decimal>();
    
    public decimal Debit { get; } = row.Cell(4).GetValue<decimal>();
    
    public decimal Credit { get; } = row.Cell(5).GetValue<decimal>();
    
    public decimal ClosingBalance { get; } = row.Cell(6).GetValue<decimal>();
}