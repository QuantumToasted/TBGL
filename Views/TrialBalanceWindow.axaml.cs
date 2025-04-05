using System;
using TBGL.ViewModels;

namespace TBGL.Views;

public partial class TrialBalanceWindow : ViewBase<TrialBalanceViewModel>
{
    public TrialBalanceWindow(IServiceProvider services) : base(services)
    {
        InitializeComponent();
    }
}