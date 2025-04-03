using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TBGL.Common;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryDetailsWindowViewModel : ViewModelBase
{
    private GeneralLedgerAccountMetadata? _account;

    [ObservableProperty]
    private string? _boundTitle;
    
    public override string Title => $"Transaction History Details for account {Account}";

    public ObservableCollection<GeneralLedgerTransaction> Transactions { get; set; } = [];

    public GeneralLedgerAccountMetadata? Account
    {
        get => _account;
        set
        {
            _account = value;
            BoundTitle = $"Transaction History Details for account {_account}";
        }
    }
}