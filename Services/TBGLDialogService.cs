using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using TBGL.ViewModels;
using TBGL.Views;

namespace TBGL.Services;

public sealed class TBGLDialogService(IView<MainWindowViewModel> view) : IDialogService
{
    private readonly MainWindow _mainWindow = (MainWindow)view;
    public Task<IStorageFile?> ShowTrialBalanceFileDialogAsync()
        => OpenFilePickerAsync("Trial Balance Report location?", "Trial_Balance_Report", Filter.XLSX);

    public Task<IStorageFile?> ShowGeneralLedgerFileDialogAsync()
        => OpenFilePickerAsync("General Ledger Report location?", "General_Ledger_Report", Filter.XLSX);

    public Task<IStorageFile?> ShowTemplateFileDialogAsync()
        => OpenFilePickerAsync("Property Template location?", "Template", Filter.TOML);

    public async Task<IStorageFile?> ShowGeneratedWorkpaperDialogAsync(string property)
    {
        try
        {
            var suggestedStartLocation = await _mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
            var file = await _mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = [Filter.XLSX],
                DefaultExtension = "xlsx",
                ShowOverwritePrompt = true,
                SuggestedFileName = property,
                Title = "Save Generated Workpaper",
                SuggestedStartLocation = suggestedStartLocation
            });

            return file;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    private async Task<IStorageFile?> OpenFilePickerAsync(string title, string suggestedFileName, FilePickerFileType fileType)
    {
        try
        {
            var suggestedStartLocation = await _mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
            var options = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                SuggestedFileName = suggestedFileName,
                FileTypeFilter = [fileType],
                SuggestedStartLocation = suggestedStartLocation
            };

            var files = await _mainWindow.StorageProvider.OpenFilePickerAsync(options);
            return files.Count > 0 ? files[0] : null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    private static class Filter
    {
        public static FilePickerFileType TOML => new("TOML markup file") { Patterns = ["*.toml"] };

        public static FilePickerFileType XLSX => new("Excel spreadsheet") { Patterns = ["*.xlsx"] };
    }
}