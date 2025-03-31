using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using TBGL.Extensions;
using TBGL.Services;
using TBGL.ViewModels;
using TBGL.Views;

namespace TBGL;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
        // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
        DisableAvaloniaDataAnnotationValidation();

        var services = new ServiceCollection()
            .AddViewWithViewModel<MainWindow, MainWindowViewModel>(ServiceLifetime.Singleton)
            .AddViewWithViewModel<TransactionHistoryListWindow, TransactionHistoryListWindowViewModel>()
            .AddViewWithViewModel<TransactionHistoryDetailsWindow, TransactionHistoryDetailsWindowViewModel>()
            //.AddSingleton<ViewLocator>()
            .AddSingletonWithImplementation<IWindowService, WindowService>()
            .AddSingletonWithImplementation<IFileDialogService, FileDialogService>()
            .AddSingletonWithImplementation<IExcelService, ExcelService>()
            .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
        
        //DataTemplates.Add(services.GetRequiredService<ViewLocator>());
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // MainWindow is singleton
            var mainWindow = services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}