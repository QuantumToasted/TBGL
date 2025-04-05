using System.Threading.Tasks;
using TBGL.Common;

namespace TBGL.Services;

public interface IWindowService
{
    Task ShowTransactionHistoryListWindowAsync();
    void ShowTransactionHistoryDetailsWindow(GeneralLedgerTransactionHistory history);
    Task ShowTrialBalanceWindowAsync();
}