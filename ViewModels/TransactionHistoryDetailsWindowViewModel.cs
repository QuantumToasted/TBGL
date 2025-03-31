using System.Collections.ObjectModel;
using System.Linq;
using TBGL.Common;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryDetailsWindowViewModel : ViewModelBase
{
    public override string? Title => $"Transaction History Details for account {Account}";

    public ObservableCollection<GeneralLedgerTransaction> Transactions { get; set; } = [];
    
    public GeneralLedgerAccountMetadata? Account { get; set; }
}