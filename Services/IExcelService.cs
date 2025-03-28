using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;
using TBGL.Common;

namespace TBGL.Services;

public interface IExcelService
{
    Task<TrialBalanceLoadResult> LoadTrialBalanceWorkbookAsync(IStorageFile storageFile);
    Task<GeneralLedgerLoadResult> LoadGeneralLedgerWorkbookAsync(IStorageFile storageFile);
    Task GenerateFilesAsync(TemplateModel template);
}