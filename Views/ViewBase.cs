using System;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TBGL.ViewModels;

namespace TBGL.Views;

public class ViewBase<TViewModel> : Window, IView<TViewModel> 
    where TViewModel : ViewModelBase
{
    protected ViewBase(IServiceProvider services)
    {
        DataContext = services.GetRequiredService<TViewModel>();
    }
    
    public TViewModel ViewModel => (TViewModel)DataContext!;
}