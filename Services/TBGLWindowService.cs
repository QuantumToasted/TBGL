using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Views;

namespace TBGL.Services;

public sealed class TBGLWindowService(IServiceProvider services) : IWindowService
{
    public void ShowTransactionHistoryListWindow()
        => ShowWindow<TransactionHistoryListWindow>();
    
    public Task ShowTransactionHistoryListWindowAsync()
        => ShowWindowDialogAsync<TransactionHistoryListWindow, MainWindow>();

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