using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lite_Uninstaller.ViewModels;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.Views.Pages;

public partial class UninstallPage : INavigableView<UninstallPageViewModel>
{
    public UninstallPageViewModel ViewModel { get; }
    private bool _textModifiedFromCode;
    
    public UninstallPage(UninstallPageViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
    }

    private void SortByComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.SortAppsList(SortByComboBox.SelectedIndex);
    }

    private async void Refresh_OnClick(object sender, RoutedEventArgs e)
    {
        _textModifiedFromCode = true;
        AutoSuggestBox.Text = "";
        _textModifiedFromCode = false;
        var taskCompletionSource = new TaskCompletionSource<bool>();
        await ViewModel.SetupList(taskCompletionSource);
        await taskCompletionSource.Task;
        ListView.SelectedIndex = 0;
    }

    private void AutoSuggestBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (_textModifiedFromCode)
        {
            return;
        }
        
        RefreshListWithFilter(args.Text);
    }

    private async void RefreshListWithFilter(string text = "")
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        ViewModel.LoadList(taskCompletionSource, text);
        await taskCompletionSource.Task;
        ListView.SelectedIndex = 0;
        ViewModel.SortAppsList(SortByComboBox.SelectedIndex == 0 ? 0 : SortByComboBox.SelectedIndex = 0);
    }

    private void ListView_OnSelectionChanged(object sender, RoutedEventArgs e)
    {
        ViewModel.NoAppSelected = ListView.SelectedItem == null;
        ViewModel.MultipleAppsSelected = ListView.SelectedItems.Count > 1;
        
        if (ListView.SelectedIndex < 0)
        {
            return;
        }
        
        ViewModel.SelectedApp = ViewModel.AppsList[ListView.SelectedIndex];
    }

    private void PathLabel_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        UninstallPageViewModel.OpenInstallPath(PathLabel.Text);
    }
}
