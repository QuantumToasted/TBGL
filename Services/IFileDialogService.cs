using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using TBGL.Common;

namespace TBGL.Services;

public interface IFileDialogService
{
    Task<Uri?> ShowTrialBalanceFileDialogAsync();
    Task<Uri?> ShowGeneralLedgerFileDialogAsync();
    Task<Uri?> ShowTemplateFileDialogAsync();
    Task<Uri?> ShowGeneratedWorkpaperDialogAsync(PropertyMetadata property);
}