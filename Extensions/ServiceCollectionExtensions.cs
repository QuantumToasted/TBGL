using System.Linq;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TBGL.Common;
using TBGL.ViewModels;
using TBGL.Views;

namespace TBGL.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonWithImplementation<TBase, TImpl>(this IServiceCollection serviceCollection)
        where TBase : class
        where TImpl : class, TBase
    {
        return serviceCollection.AddSingleton<TImpl>().AddSingleton<TBase>(static x => x.GetRequiredService<TImpl>());
    }
    
    public static IServiceCollection AddTransientWithImplementation<TBase, TImpl>(this IServiceCollection serviceCollection)
        where TBase : class
        where TImpl : class, TBase
    {
        return serviceCollection.AddTransient<TImpl>().AddTransient<TBase>(static x => x.GetRequiredService<TImpl>());
    }

    public static IServiceCollection AddViewWithViewModel<TView, TViewModel>(this IServiceCollection serviceCollection,
        ServiceLifetime viewModelLifetime = ServiceLifetime.Transient)
        where TView : ViewBase<TViewModel>
        where TViewModel : ViewModelBase
    {
        return serviceCollection.Add<TViewModel>(viewModelLifetime)
            .AddTransient<TView>()
            .AddTransient<IView<TViewModel>, TView>()
            .AddKeyedTransient<IView, TView>(typeof(TViewModel));
    }

    private static IServiceCollection Add<TService>(this IServiceCollection serviceCollection, ServiceLifetime lifetime)
    {
        serviceCollection.Add(ServiceDescriptor.Describe(typeof(TService), typeof(TService), lifetime));
        return serviceCollection;
    }
}