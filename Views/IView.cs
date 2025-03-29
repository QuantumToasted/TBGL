using TBGL.ViewModels;

namespace TBGL.Views;

public interface IView
{
    ViewModelBase ViewModel { get; }
}

public interface IView<out TViewModel> : IView
    where TViewModel : ViewModelBase
{
    new TViewModel ViewModel { get; }

    ViewModelBase IView.ViewModel => ViewModel;
}