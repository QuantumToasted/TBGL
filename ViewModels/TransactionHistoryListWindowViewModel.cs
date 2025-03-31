using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using TBGL.Common;
using TBGL.Services;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryListWindowViewModel(IExcelService excelService, IWindowService windowService) : ViewModelBase
{
    public ObservableCollection<GeneralLedgerTransactionHistory> TransactionHistories { get; } = new(excelService.GeneralLedgerReport!.TransactionHistories);

    [RelayCommand]
    public void ShowDetails(GeneralLedgerTransactionHistory history)
    {
        windowService.ShowTransactionHistoryDetailsWindow(history);
    }
}