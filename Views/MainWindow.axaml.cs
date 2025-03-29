using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TBGL.ViewModels;

namespace TBGL.Views;

public partial class MainWindow : ViewBase<MainWindowViewModel>
{
    public MainWindow(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }
    
    private async void OnTrialBalanceButtonClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.BrowseTrialBalanceCommand.CanExecute(null))
                return;

            await ViewModel.BrowseTrialBalanceCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void OnGeneralLedgerButtonClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.BrowseGeneralLedgerCommand.CanExecute(null))
                return;

            await ViewModel.BrowseGeneralLedgerCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void OnTemplateSelected(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (!ViewModel.BrowseTemplateCommand.CanExecute(null))
                return;

            if (e.AddedItems is not [string added])
                return;

            await ViewModel.BrowseTemplateCommand.ExecuteAsync(added);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void OnGenerateButtonClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.GenerateWorkpaperCommand.CanExecute(null))
                return;

            await ViewModel.GenerateWorkpaperCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}