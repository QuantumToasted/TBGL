using System;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Common;
using TBGL.ViewModels;

namespace TBGL.Views;

public partial class TransactionHistoryListWindow : ViewBase<TransactionHistoryListWindowViewModel>
{
    public TransactionHistoryListWindow(IServiceProvider services)
    {
        InitializeComponent();

        DataContext = services.GetRequiredService<TransactionHistoryListWindow>();
    }
}