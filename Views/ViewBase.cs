using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using TBGL.ViewModels;

namespace TBGL.Views;

public class ViewBase<TViewModel> : Window, IView<TViewModel> 
    where TViewModel : ViewModelBase
{
    public TViewModel ViewModel { get; set; } = null!;
}