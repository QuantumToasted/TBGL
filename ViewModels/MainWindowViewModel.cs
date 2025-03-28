using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using TBGL.Common;
using TBGL.Services;
using TBGL.Views;

namespace TBGL.ViewModels;

public sealed partial class MainWindowViewModel(IDialogService dialogService, IExcelService excelService, 
    IStorageProvider storageProvider, IServiceProvider services) : ViewModelBase, IDisposable
{
    private const string DEFAULT_TEMPLATE = "Auto-detect";
    private const string OVERRIDE_TEMPLATE = "Override";
    
    private static readonly IReadOnlyDictionary<string, string> GeneratedTemplates;

    [ObservableProperty]
    private TrialBalanceLoadResult? _trialBalanceReport;

    [ObservableProperty]
    private GeneralLedgerLoadResult? _generalLedgerReport;

    [ObservableProperty]
    private IStorageFile? _selectedTemplateFile;

    [ObservableProperty]
    private bool _trialBalanceReportSelected;

    public ObservableCollection<string> PropertyTemplates { get; } = new(new[] { DEFAULT_TEMPLATE }.Concat(GeneratedTemplates.Keys).Append(OVERRIDE_TEMPLATE));
    
    public static string DefaultTemplate => DEFAULT_TEMPLATE;

    [RelayCommand]
    public async Task BrowseTrialBalance(RoutedEventArgs e)
    {
        if (await dialogService.ShowTrialBalanceFileDialogAsync() is not { } file)
            return;

        try
        {
            var result = await excelService.LoadTrialBalanceWorkbookAsync(file);

            TrialBalanceReport = result;
            TrialBalanceReportSelected = true;
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", $"Failed to load trial balance report worksheet:\n{ex}").ShowAsync();
            TrialBalanceReportSelected = false;
        }
    }

    [RelayCommand]
    public async Task BrowseGeneralLedger(RoutedEventArgs e)
    {
        if (await dialogService.ShowGeneralLedgerFileDialogAsync() is not { } file)
            return;

        try
        {
            var result = await excelService.LoadGeneralLedgerWorkbookAsync(file);
            GeneralLedgerReport = result;

            foreach (var history in result.TransactionHistories)
            {
                history.Validate();
            }

            var listWindow = services.GetRequiredService<TransactionHistoryListWindow>();
            var mainWindow = services.GetRequiredService<MainWindow>();

            await mainWindow.ShowDialog(listWindow);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", $"Failed to load general ledger report worksheet:\n{ex}").ShowAsync();
        }
    }

    [RelayCommand]
    public async Task BrowseTemplate(string added)
    {
        switch (added)
        {
            case OVERRIDE_TEMPLATE:
                SelectedTemplateFile = await dialogService.ShowTemplateFileDialogAsync();
                break;
            case DEFAULT_TEMPLATE:
                return;
            default:
                SelectedTemplateFile = await storageProvider.TryGetFileFromPathAsync(GeneratedTemplates[added]);
                return;
        }
    }

    [RelayCommand]
    public async Task GenerateWorkpaper(RoutedEventArgs e)
    {
    }

    static MainWindowViewModel()
    {
        GeneratedTemplates = Directory.GetFiles("./Templates", "*.toml", SearchOption.TopDirectoryOnly)
            .ToDictionary(x => Path.GetFileNameWithoutExtension(x)!, x => x);
    }

    public void Dispose()
    {
        TrialBalanceReport?.Dispose();
        GeneralLedgerReport?.Dispose();
        SelectedTemplateFile?.Dispose();
    }
}