using System.Windows.Input;
using Lite_Uninstaller.ViewModels;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.Views.Pages;

public partial class UninstallPage : INavigableView<UninstallPageViewModel>
{
    public UninstallPageViewModel ViewModel { get; }
    
    public UninstallPage(UninstallPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
    }

    private void AutoSuggestBox_OnKeyDown(object sender, KeyEventArgs e)
    {
    }
}