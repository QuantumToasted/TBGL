using System;
using System.Threading.Tasks;
using TBGL.Common;

namespace TBGL.Services;

public interface IFileDialogService
{
    Task<Uri?> ShowTrialBalanceFileDialogAsync();
    Task<Uri?> ShowGeneralLedgerFileDialogAsync();
    Task<TemplateModel?> ShowTemplateFileDialogAsync();
    Task<Uri?> ShowGeneratedWorkpaperDialogAsync(TemplateModel template);
}