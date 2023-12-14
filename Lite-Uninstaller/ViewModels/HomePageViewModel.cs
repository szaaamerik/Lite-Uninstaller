using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lite_Uninstaller.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.ViewModels;

public partial class HomePageViewModel(INavigationService navigationService) : ObservableObject, INavigationAware
{
    public void OnNavigatedTo() { }

    public void OnNavigatedFrom() { }

    [RelayCommand]
    private void OnPageSwitch(string parameter)
    {
        switch (parameter)
        {
            case "uninstall_software":
            {
                navigationService.Navigate(typeof(UninstallPage));                
                break;
            }

            case "about":
            {
                navigationService.Navigate(typeof(AboutPage));                
                break;
            }

            case "settings":
            {
                //navigationService.Navigate(typeof(SettingsPage));
                break;
            }

            default:
            {
                throw new ArgumentOutOfRangeException(parameter);
            }
        }
    }
}