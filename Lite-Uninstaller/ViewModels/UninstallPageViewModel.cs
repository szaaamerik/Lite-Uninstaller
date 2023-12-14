using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.ViewModels;

public partial class UninstallPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty] 
    private string? _versionText;

    public void OnNavigatedTo()
    {
        if (IsInitialized)
        {
            return;
        }

        InitializeViewModel();
    }

    public void OnNavigatedFrom() { }

    private void InitializeViewModel()
    {
        VersionText = $"Tool Version - {Assembly.GetExecutingAssembly().GetName().Version!.ToString()}";
        IsInitialized = true;
    }

    [RelayCommand]
    private static void OnPageSwitch(string parameter)
    {
        throw parameter switch
        {
            "game_presets" => new NotImplementedException(),
            _ => new ArgumentOutOfRangeException(parameter)
        };
    }
}