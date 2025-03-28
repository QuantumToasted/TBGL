using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace TBGL.Services;

public interface IDialogService
{
    Task<IStorageFile?> ShowTrialBalanceFileDialogAsync();
    Task<IStorageFile?> ShowGeneralLedgerFileDialogAsync();
    Task<IStorageFile?> ShowTemplateFileDialogAsync();
    Task<IStorageFile?> ShowGeneratedWorkpaperDialogAsync(string property);
}