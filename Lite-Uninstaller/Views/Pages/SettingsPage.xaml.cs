using System.Windows.Controls;
using Lite_Uninstaller.ViewModels;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.Views.Pages;

public partial class SettingsPage : INavigableView<SettingsPageViewModel>
{
    public SettingsPageViewModel ViewModel { get; }
    
    public SettingsPage(SettingsPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        
    }
}