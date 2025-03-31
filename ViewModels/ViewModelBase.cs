using CommunityToolkit.Mvvm.ComponentModel;

namespace TBGL.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    public abstract string? Title { get; }
}