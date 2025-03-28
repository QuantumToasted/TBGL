using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;
using TBGL.ViewModels;

namespace TBGL.Views;

public partial class MainWindow : ViewBase<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private async void OnTrialBalanceButtonClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (!ViewModel.BrowseTrialBalanceCommand.CanExecute(null))
                return;

            await ViewModel.BrowseTrialBalanceCommand.ExecuteAsync(e);
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

            await ViewModel.BrowseGeneralLedgerCommand.ExecuteAsync(e);
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

            await ViewModel.GenerateWorkpaperCommand.ExecuteAsync(e);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}