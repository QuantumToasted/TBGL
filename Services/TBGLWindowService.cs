using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Common;
using TBGL.Views;

namespace TBGL.Services;

public sealed class TBGLWindowService(IServiceProvider services) : IWindowService
{
    public void ShowTransactionHistoryListWindow()
        => ShowWindow<TransactionHistoryListWindow>();
    
    public Task ShowTransactionHistoryListWindowAsync()
        => ShowWindowDialogAsync<TransactionHistoryListWindow, MainWindow>();

    public void ShowTransactionHistoryDetailsWindow(GeneralLedgerTransactionHistory history)
    {
        var scope = services.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<TransactionHistoryDetailsWindow>();

        window.Show();
        
        foreach (var transaction in history.Transactions)
        {
            window.ViewModel.Transactions.Add(transaction);
        }
        
        //window.ViewModel.Transactions = new ObservableCollection<GeneralLedgerTransaction>(history.Transactions);
    }

    private void ShowWindow<TWindow>() 
        where TWindow : Window
    {
        var scope = services.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<TWindow>();
        
        window.Show();
    }
    
    private Task ShowWindowDialogAsync<TWindow, TParentWindow>() 
        where TWindow : Window 
        where TParentWindow : Window
    {
        var scope = services.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<TWindow>();
        var parentWindow = scope.ServiceProvider.GetRequiredService<TParentWindow>();
        return window.ShowDialog(parentWindow);
    }
}