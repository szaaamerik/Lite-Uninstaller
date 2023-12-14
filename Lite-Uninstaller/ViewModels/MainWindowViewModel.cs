using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Lite_Uninstaller.Views.Pages;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty]
    private string _applicationTitle = string.Empty;

    [ObservableProperty]
    private ObservableCollection<object> _mainItems = [];

    [ObservableProperty]
    private ObservableCollection<object> _footerItems = [];

    public MainWindowViewModel()
    {
        if (IsInitialized)
        {
            return;
        }

        InitializeViewModel();
    }

    private void InitializeViewModel()
    {
        ApplicationTitle = "Lite Uninstaller";
        
        MainItems =
        [
            new NavigationViewItem
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(HomePage)
            },
            new NavigationViewItem
            {
                Content = "Uninstall Software",
                Icon = new SymbolIcon {Symbol = SymbolRegular.Recycle20 },
                TargetPageType = typeof(UninstallPage)
            }
        ];

        FooterItems =
        [
            new NavigationViewItem
            {
                Content = "About",
                Icon = new SymbolIcon {Symbol = SymbolRegular.Info20 },
                TargetPageType = typeof(AboutPage)
            },
            new NavigationViewItem
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            }
        ];

        IsInitialized = true;
    }
}