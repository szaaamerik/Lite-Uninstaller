using System.Windows;
using Lite_Uninstaller.Views;
using Lite_Uninstaller.Views.Pages;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace Lite_Uninstaller.Services;

public class ApplicationHostService(IServiceProvider serviceProvider) : IHostedService
{
    private INavigationWindow? _navigationWindow;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return HandleActivationAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task HandleActivationAsync()
    {
        await Task.CompletedTask;

        if (!Application.Current.Windows.OfType<MainWindow>().Any())
        {
            _navigationWindow = (serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
            _navigationWindow.ShowWindow();
            _navigationWindow.Navigate(typeof(HomePage));
        }

        await Task.CompletedTask;
    }
}