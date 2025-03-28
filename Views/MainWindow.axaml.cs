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
            if (sender is not Button { DataContext: MainWindowViewModel viewModel })
                return;

            if (!viewModel.BrowseTrialBalanceCommand.CanExecute(null))
                return;

            await viewModel.BrowseTrialBalanceCommand.ExecuteAsync(e);
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
            if (sender is not Button { DataContext: MainWindowViewModel viewModel })
                return;

            if (!viewModel.BrowseGeneralLedgerCommand.CanExecute(null))
                return;

            await viewModel.BrowseGeneralLedgerCommand.ExecuteAsync(e);
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
            if (sender is not ComboBox { DataContext: MainWindowViewModel viewModel })
                return;

            if (!viewModel.BrowseTemplateCommand.CanExecute(null))
                return;

            if (e.AddedItems is not [string added])
                return;

            await viewModel.BrowseTemplateCommand.ExecuteAsync(added);
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
            if (sender is not ComboBox { DataContext: MainWindowViewModel viewModel })
                return;

            if (!viewModel.GenerateWorkpaperCommand.CanExecute(null))
                return;

            await viewModel.GenerateWorkpaperCommand.ExecuteAsync(e);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}