using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using TBGL.ViewModels;
using TBGL.Views;

namespace TBGL;

public sealed class ViewLocator(IServiceProvider services) : IDataTemplate
{
    public Control Build(object? param)
    {
        Console.WriteLine($"Build() called for [{param}]");
        if (param is not ViewModelBase viewModel ||
            services.GetKeyedService<IView>(viewModel.GetType()) is not { } view)
        {
            throw new InvalidOperationException();
        }
            
        if (services.GetKeyedService<IView>(param.GetType()) is not Control control)
            return new TextBlock { Text = $"No view found for type {param.GetType().Name}." };

        view.ViewModel = viewModel;
        return control;
    }

    public bool Match(object? data)
    {
        Console.WriteLine($"Matching on {data?.GetType()}");
        return data is ViewModelBase;
    }
}