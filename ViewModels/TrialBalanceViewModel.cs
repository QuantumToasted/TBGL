using System.Collections.ObjectModel;
using TBGL.Common;
using TBGL.Services;

namespace TBGL.ViewModels;

public sealed class TrialBalanceViewModel(IExcelService excelService) : ViewModelBase
{
    public override string Title => $"Trial balance report for {excelService.TrialBalanceReport!.Property}";

    public ObservableCollection<TrialBalanceAccount> Accounts { get; } = new(excelService.TrialBalanceReport!.Accounts);
}