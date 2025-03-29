using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ClosedXML.Excel;
using TBGL.Common;

namespace TBGL.Services;

public sealed class TBGLExcelService : IExcelService
{
    public TrialBalanceLoadResult? TrialBalanceReport { get; private set; }
    
    public GeneralLedgerLoadResult? GeneralLedgerReport { get; private set; }

    public async Task<TrialBalanceLoadResult> LoadTrialBalanceWorkbookAsync(Uri workbookPath)
    {
        var stream = await GetWorkbookStreamAsync(workbookPath);
        var workbook = new XLWorkbook(stream);
        return TrialBalanceReport = new TrialBalanceLoadResult(workbookPath.LocalPath, workbook.Worksheet(1));
    }

    public async Task<GeneralLedgerLoadResult> LoadGeneralLedgerWorkbookAsync(Uri workbookPath)
    {
        var stream = await GetWorkbookStreamAsync(workbookPath);
        var workbook = new XLWorkbook(stream);
        return GeneralLedgerReport = new GeneralLedgerLoadResult(workbookPath.LocalPath, workbook.Worksheet(1));
    }

    public async Task GenerateFilesAsync(TemplateModel template)
    {
        throw new System.NotImplementedException();
    }

    private static async Task<MemoryStream> GetWorkbookStreamAsync(Uri workbookPath)
    {
        await using var file = File.OpenRead(workbookPath.LocalPath);
        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }
}