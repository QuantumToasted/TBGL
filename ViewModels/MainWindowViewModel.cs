using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using TBGL.Common;
using TBGL.Services;
using TBGL.Views;
using Tomlyn;

namespace TBGL.ViewModels;

public sealed partial class MainWindowViewModel(IFileDialogService dialogService, IExcelService excelService, IWindowService windowService) : ViewModelBase, IDisposable
{
    private const string DEFAULT_TEMPLATE = "Auto-detect";
    private const string OVERRIDE_TEMPLATE = "Override";
    
    private static readonly Dictionary<string, TemplateModel> LocalTemplates;

    [ObservableProperty]
    private TrialBalanceLoadResult? _trialBalanceReport;

    [ObservableProperty]
    private GeneralLedgerLoadResult? _generalLedgerReport;

    [ObservableProperty]
    private TemplateModel? _selectedTemplate;

    [ObservableProperty] 
    private bool _reportSelected;

    [ObservableProperty]
    private string _selectedTemplateCode = DEFAULT_TEMPLATE;

    [ObservableProperty] 
    private bool _selectedTemplatePathIsOverride;

    public override string Title => "TBGL";

    public ObservableCollection<string> PropertyTemplates { get; } = new(new [] {DEFAULT_TEMPLATE}.Concat(LocalTemplates.Keys).Append(OVERRIDE_TEMPLATE));

    [RelayCommand]
    public async Task BrowseTrialBalance()
    {
        if (await dialogService.ShowTrialBalanceFileDialogAsync() is not { } file)
            return;
        
        try
        {
            var result = await excelService.LoadTrialBalanceWorkbookAsync(file);
            
            await windowService.ShowTrialBalanceWindowAsync();

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
                SelectedTemplate = await dialogService.ShowTemplateFileDialogAsync();
                break;
            case DEFAULT_TEMPLATE:
                return;
            default:
                SelectedTemplate = LocalTemplates[added];
                return;
        }
    }

    [RelayCommand]
    public async Task GenerateWorkpaper()
    {
        if (!IsGenerationReady(out var error))
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", $"Cannot generate workpaper:\n\n{error}").ShowAsync();
            return;
        }
        
        var path = await dialogService.ShowGeneratedWorkpaperDialogAsync(SelectedTemplate!);
        if (path is null)
            return;
        
        excelService.GenerateWorkpaper(SelectedTemplate!, path!);
    }

    private void UpdateAutoDetectedTemplate(PropertyMetadata property)
    {
        ReportSelected = true;
        PropertyTemplates.Remove(DEFAULT_TEMPLATE);

        var code = property.Code;
        if (!LocalTemplates.TryGetValue(property.Code, out var template))
            (code, template) = LocalTemplates.First();
        
        SelectedTemplateCode = code;
        SelectedTemplate = template;
        SelectedTemplatePathIsOverride = false;
    }

    private bool IsGenerationReady([NotNullWhen(false)] out string? error)
    {
        var builder = new StringBuilder();
        var ready = true;

        if (GeneralLedgerReport is null)
        {
            ready = false;
            builder.AppendLine("General ledger report is not loaded.");
        }

        if (TrialBalanceReport is null)
        {
            ready = false;
            builder.AppendLine("Trial balance report is not loaded.");
        }

        if (SelectedTemplate is null)
        {
            ready = false;
            builder.AppendLine("Property template is not selected.");
        }

        error = builder.Length > 0 ? builder.ToString() : null;
        return ready;
    }

    static MainWindowViewModel()
    {
        LocalTemplates = Directory.GetFiles("./Templates", "*.toml", SearchOption.TopDirectoryOnly)
            .ToDictionary(x => Path.GetFileNameWithoutExtension(x)!, x => Toml.ToModel<TemplateModel>(File.ReadAllText(x)));
    }

    public void Dispose()
    {
        TrialBalanceReport?.Dispose();
        GeneralLedgerReport?.Dispose();
    }
}