using System;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Common;
using TBGL.Views;

namespace TBGL.ViewModels;

public sealed partial class TransactionHistoryListWindowViewModel : ViewModelBase
{
    public TransactionHistoryListWindowViewModel(IServiceProvider services)
    {
        var histories = services.GetRequiredService<MainWindowViewModel>().GeneralLedgerReport!.TransactionHistories;
        TransactionHistories = new ObservableCollection<GeneralLedgerTransactionHistory>(histories);
    }

    public ObservableCollection<GeneralLedgerTransactionHistory> TransactionHistories { get; }
}