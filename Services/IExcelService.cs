using System;
using System.Threading.Tasks;
using TBGL.Common;

namespace TBGL.Services;

public interface IExcelService
{
    TrialBalanceLoadResult? TrialBalanceReport { get; }
    GeneralLedgerLoadResult? GeneralLedgerReport { get; }
    
    Task<TrialBalanceLoadResult> LoadTrialBalanceWorkbookAsync(Uri workbookPath);
    Task<GeneralLedgerLoadResult> LoadGeneralLedgerWorkbookAsync(Uri workbookPath);
    void GenerateWorkpaper(PropertyMetadata property, TemplateModel template, Uri path);
}