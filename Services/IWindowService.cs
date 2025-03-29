using System.Threading.Tasks;

namespace TBGL.Services;

public interface IWindowService
{
    void ShowTransactionHistoryListWindow();
    Task ShowTransactionHistoryListWindowAsync();
}