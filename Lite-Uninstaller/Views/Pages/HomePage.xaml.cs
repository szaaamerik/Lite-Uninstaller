using System.Windows.Controls;
using Lite_Uninstaller.ViewModels;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.Views.Pages;

public partial class HomePage : INavigableView<HomePageViewModel>
{
    public HomePageViewModel ViewModel { get; }
    
    public HomePage(HomePageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
    }
}