using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;
using TBGL.Common;

namespace TBGL.Services;

public sealed class TBGLExcelService : IExcelService
{
    public async Task<TrialBalanceLoadResult> LoadTrialBalanceWorkbookAsync(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        var workbook = new XLWorkbook(stream);
        return new TrialBalanceLoadResult(storageFile.Path.AbsolutePath, workbook.Worksheet(1));
    }

    public async Task<GeneralLedgerLoadResult> LoadGeneralLedgerWorkbookAsync(IStorageFile storageFile)
    {
        await using var stream = await storageFile.OpenReadAsync();
        var workbook = new XLWorkbook(stream);
        return new GeneralLedgerLoadResult(storageFile.Path.AbsolutePath, workbook.Worksheet(1));
    }

    public async Task GenerateFilesAsync(TemplateModel template)
    {
        throw new System.NotImplementedException();
    }
}