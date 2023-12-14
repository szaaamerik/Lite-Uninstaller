using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Controls;

namespace Lite_Uninstaller.ViewModels;

public partial class UninstallPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private bool _isInitialized;

    [ObservableProperty] 
    private bool _noBackgroundActionsRunning;

    [ObservableProperty] 
    private Models.App _selectedApp = new();

    [ObservableProperty] 
    private List<Models.App> _appsList = [];

    public UninstallPageViewModel()
    {
        if (IsInitialized)
        {
            return;
        }

        InitializeViewModel();
        
    }
    
    public void OnNavigatedTo()
    {
    }

    public void OnNavigatedFrom() { }

    private async void InitializeViewModel()
    {
        var alphabet = GenerateRandomStrings(100, 5, 10);
        var enumerable = alphabet.ToArray();
        var dataMods = enumerable.Select(t => new Models.App()
        {
            Name = t,
        }).ToList();
        var cpuThreads = Environment.ProcessorCount;
        var batchSize = cpuThreads * 2;
        var sleepTime = cpuThreads <= 4 ? cpuThreads : cpuThreads / 2;
        
        for (var i = 0; i < dataMods.Count; i += batchSize)
        {
            var batch = dataMods.Skip(i).Take(batchSize).ToList();

            await Task.Run(() =>
            {
                foreach (var t in batch)
                {
                    Application.Current.Dispatcher.InvokeAsync(() => AppsList.Add(t), DispatcherPriority.Render);
                }
            });

            await Task.Delay(sleepTime); 
        }
        
        
        IsInitialized = NoBackgroundActionsRunning = true;
    }
    
    private static List<string> GenerateRandomStrings(int numberOfStrings, int minLength, int maxLength)
    {
        var randomStrings = new List<string>();
        var random = new Random();

        for (var i = 0; i < numberOfStrings; i++)
        {
            var stringLength = random.Next(minLength, maxLength + 1);
            var randomString = GenerateRandomString(stringLength);
            randomStrings.Add(randomString);
        }

        return randomStrings;
    }
    
    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();

        var randomString = new char[length];
        for (var i = 0; i < length; i++)
        {
            randomString[i] = chars[random.Next(chars.Length)];
        }

        return new string(randomString);
    }
}