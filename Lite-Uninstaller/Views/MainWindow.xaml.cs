using System.Windows;
using Lite_Uninstaller.ViewModels;
using Lite_Uninstaller.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.Views;

public partial class MainWindow : INavigationWindow
{
    public MainWindowViewModel ViewModel { get; }

    private bool _paneToggledFromCode;
    private bool _userClosedPane;

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider,
        INavigationService navigationService,
        ISnackbarService snackBarService,
        IContentDialogService contentDialogService)
    {
        ViewModel = viewModel;
        DataContext = this;
        
        InitializeComponent();
        
        snackBarService.SetSnackbarPresenter(SnackbarPresenter);
        navigationService.SetNavigationControl(NavigationView);
        contentDialogService.SetContentPresenter(RootContentDialog);
        NavigationView.SetServiceProvider(serviceProvider);
        Loaded += (_, _) => NavigationView.IsPaneOpen = false;
    }

    #region INavigationWindow methods

    public bool Navigate(Type pageType) => NavigationView.Navigate(pageType);

    public void SetPageService(IPageService pageService) => NavigationView.SetPageService(pageService);

    public void ShowWindow() => Show();

    public void CloseWindow() => Close();

    #endregion INavigationWindow methods

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }

    INavigationView INavigationWindow.GetNavigation()
    {
        throw new NotImplementedException();
    }

    public void SetServiceProvider(IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }
    
    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_userClosedPane)
        {
            return;
        }

        _paneToggledFromCode = true;
        NavigationView.SetCurrentValue(NavigationView.IsPaneOpenProperty, e.NewSize.Width > 1200);
        _paneToggledFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_paneToggledFromCode)
        {
            return;
        }

        _userClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_paneToggledFromCode)
        {
            return;
        }

        _userClosedPane = true;
    }
}