using TBGL.ViewModels;

namespace TBGL.Views;

public interface IView
{
    ViewModelBase ViewModel { get; set; }
}

public interface IView<TViewModel> : IView
    where TViewModel : ViewModelBase
{
    new TViewModel ViewModel { get; set; }

    ViewModelBase IView.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel)value;
    }
}