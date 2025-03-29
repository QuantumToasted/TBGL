using System.Collections.ObjectModel;
using TBGL.Common;
using TBGL.Services;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryListWindowViewModel(IExcelService excelService) : ViewModelBase
{
    public ObservableCollection<GeneralLedgerTransactionHistory> TransactionHistories { get; } = new(excelService.GeneralLedgerReport!.TransactionHistories);
}