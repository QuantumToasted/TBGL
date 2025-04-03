using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Common;
using TBGL.Views;

namespace TBGL.Services;

public sealed class WindowService(IServiceProvider services) : IWindowService
{
    public Task ShowTransactionHistoryListWindowAsync()
        => ShowWindowDialogAsync<TransactionHistoryListWindow, MainWindow>();

    public void ShowTransactionHistoryDetailsWindow(GeneralLedgerTransactionHistory history)
    {
        var window = ShowWindow<TransactionHistoryDetailsWindow>();
        window.ViewModel.Account = history.Metadata;
        
        foreach (var transaction in history.EnumerateTransactions())
        {
            window.ViewModel.Transactions.Add(transaction);
        }
    }

    private TWindow ShowWindow<TWindow>() 
        where TWindow : Window
    {
        var scope = services.CreateScope();
        var window = scope.ServiceProvider.GetRequiredService<TWindow>();
        
        window.Show();
        return window;
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