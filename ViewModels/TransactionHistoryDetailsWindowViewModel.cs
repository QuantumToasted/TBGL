using System.Collections.ObjectModel;
using TBGL.Common;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryDetailsWindowViewModel : ViewModelBase
{
    public ObservableCollection<GeneralLedgerTransaction> Transactions { get; set; } = [];
}