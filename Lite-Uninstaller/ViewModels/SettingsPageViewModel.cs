using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lite_Uninstaller.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.ViewModels;

public partial class SettingsPageViewModel(INavigationService navigationService) : ObservableObject, INavigationAware
{
    public void OnNavigatedTo() { }

    public void OnNavigatedFrom() { }
}