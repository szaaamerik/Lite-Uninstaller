using System.Windows;
using System.Windows.Threading;
using Lite_Uninstaller.Services;
using Lite_Uninstaller.ViewModels;
using Lite_Uninstaller.Views;
using Lite_Uninstaller.Views.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace Lite_Uninstaller;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(c =>
        {
            c.SetBasePath(AppContext.BaseDirectory);
        })
        .ConfigureServices((_, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<HomePageViewModel>();
            services.AddSingleton<HomePage>();
            services.AddSingleton<UninstallPageViewModel>();
            services.AddSingleton<UninstallPage>();
            services.AddSingleton<SettingsPageViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<AboutPage>();
        }).Build();
    
    private void App_OnStartup(object sender, StartupEventArgs e)
    {
        Host.StartAsync();
    }

    private void App_OnExit(object sender, ExitEventArgs e)
    {
        Host.StopAsync();
        Host.Dispose();
    }

    private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
    }
}