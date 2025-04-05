using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using TBGL.Common;
using TBGL.Services;
using TBGL.Views;

namespace TBGL.ViewModels;

public sealed partial class MainWindowViewModel(IFileDialogService dialogService, IExcelService excelService, IWindowService windowService) : ViewModelBase, IDisposable
{
    private const string DEFAULT_TEMPLATE = "Auto-detect";
    private const string OVERRIDE_TEMPLATE = "Override";
    
    private static readonly Dictionary<string, Uri> GeneratedTemplates;

    [ObservableProperty]
    private TrialBalanceLoadResult? _trialBalanceReport;

    [ObservableProperty]
    private GeneralLedgerLoadResult? _generalLedgerReport;

    [ObservableProperty]
    private Uri? _selectedTemplateFilePath;

    [ObservableProperty] 
    private bool _reportSelected;

    [ObservableProperty]
    private string _selectedTemplateCode = DEFAULT_TEMPLATE;

    [ObservableProperty] 
    private bool _selectedTemplatePathIsOverride;

    public override string Title => "TBGL";

    public ObservableCollection<string> PropertyTemplates { get; } = new(new [] {DEFAULT_TEMPLATE}.Concat(GeneratedTemplates.Keys).Append(OVERRIDE_TEMPLATE));

    [RelayCommand]
    public async Task BrowseTrialBalance()
    {
        if (await dialogService.ShowTrialBalanceFileDialogAsync() is not { } file)
            return;
        
        try
        {
            var result = await excelService.LoadTrialBalanceWorkbookAsync(file);

            TrialBalanceReport = result;

            UpdateAutoDetectedTemplate(result.Property);
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", $"Failed to load trial balance report worksheet:\n{ex}").ShowAsync();
        }
    }

    [RelayCommand]
    public async Task BrowseGeneralLedger()
    {
        if (await dialogService.ShowGeneralLedgerFileDialogAsync() is not { } file)
            return;
        
        try
        {
            var result = await excelService.LoadGeneralLedgerWorkbookAsync(file);

            await windowService.ShowTransactionHistoryListWindowAsync();
            
            GeneralLedgerReport = result;

            UpdateAutoDetectedTemplate(result.Property);
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
                SelectedTemplateFilePath = await dialogService.ShowTemplateFileDialogAsync();
                break;
            case DEFAULT_TEMPLATE:
                return;
            default:
                SelectedTemplateFilePath = GeneratedTemplates[added];
                return;
        }
    }

    [RelayCommand]
    public async Task GenerateWorkpaper()
    {
    }

    private void UpdateAutoDetectedTemplate(PropertyMetadata property)
    {
        ReportSelected = true;
        PropertyTemplates.Remove(DEFAULT_TEMPLATE);

        var code = property.Code;
        if (!GeneratedTemplates.TryGetValue(property.Code, out var path))
            (code, path) = GeneratedTemplates.First();
        
        SelectedTemplateCode = code;
        SelectedTemplateFilePath = path;
        SelectedTemplatePathIsOverride = false;
    }

    static MainWindowViewModel()
    {
        GeneratedTemplates = Directory.GetFiles("./Templates", "*.toml", SearchOption.TopDirectoryOnly)
            .ToDictionary(x => Path.GetFileNameWithoutExtension(x)!, x => new Uri(Path.GetFullPath(x)));
    }

    public void Dispose()
    {
        TrialBalanceReport?.Dispose();
        GeneralLedgerReport?.Dispose();
    }
}