using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using TBGL.Common;
using TBGL.Views;

namespace TBGL.Services;

public sealed class TBGLFileDialogService : IFileDialogService
{
    public Task<Uri?> ShowTrialBalanceFileDialogAsync()
        => OpenFilePickerAsync("Trial Balance Report location?", "Trial_Balance_Report", Filter.XLSX);

    public Task<Uri?> ShowGeneralLedgerFileDialogAsync()
        => OpenFilePickerAsync("General Ledger Report location?", "General_Ledger_Report", Filter.XLSX);

    public Task<Uri?> ShowTemplateFileDialogAsync()
        => OpenFilePickerAsync("Property Template location?", "Template", Filter.TOML);

    public async Task<Uri?> ShowGeneratedWorkpaperDialogAsync(PropertyMetadata property)
    {
        try
        {
            var mainWindow = GetMainWindow();
            var suggestedStartLocation = await mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
            var file = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                FileTypeChoices = [Filter.XLSX],
                DefaultExtension = "xlsx",
                ShowOverwritePrompt = true,
                SuggestedFileName = property.Code.ToString(),
                Title = "Save Generated Workpaper",
                SuggestedStartLocation = suggestedStartLocation
            });

            return file?.Path;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    private static async Task<Uri?> OpenFilePickerAsync(string title, string suggestedFileName, FilePickerFileType fileType)
    {
        try
        {
            var mainWindow = GetMainWindow();
            var suggestedStartLocation = await mainWindow.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
            var options = new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = title,
                SuggestedFileName = suggestedFileName,
                FileTypeFilter = [fileType],
                SuggestedStartLocation = suggestedStartLocation
            };

            var files = await mainWindow.StorageProvider.OpenFilePickerAsync(options);
            return files.FirstOrDefault()?.Path;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
    }

    private static MainWindow GetMainWindow()
    {
        var lifetime = Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        return lifetime?.MainWindow as MainWindow ?? throw new InvalidOperationException("Invalid context for accessing main window.");
    }

    private static class Filter
    {
        public static FilePickerFileType TOML => new("TOML markup file") { Patterns = ["*.toml"] };

        public static FilePickerFileType XLSX => new("Excel spreadsheet") { Patterns = ["*.xlsx"] };
    }
}