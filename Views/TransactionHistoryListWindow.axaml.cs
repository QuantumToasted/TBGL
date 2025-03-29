using System;
using TBGL.ViewModels;

namespace TBGL.Views;

public partial class TransactionHistoryListWindow : ViewBase<TransactionHistoryListWindowViewModel>
{
    public TransactionHistoryListWindow(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }
}