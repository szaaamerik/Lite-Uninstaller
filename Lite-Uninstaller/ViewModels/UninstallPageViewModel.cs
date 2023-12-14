using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wpf.Ui;
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
    private ObservableCollection<Models.App> _appsList = [];
    
    [ObservableProperty] 
    private List<Models.App> _batchedAppsList = [];
    
    [ObservableProperty] 
    private ObservableCollection<string> _appNames = [];

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

    private void InitializeViewModel()
    {
        SetupList();
        IsInitialized = NoBackgroundActionsRunning = true;
    }

    public void SetupList()
    {
        var alphabet = GenerateRandomStrings(50, 5, 10);
        var enumerable = alphabet.ToArray();
        BatchedAppsList = enumerable.Select(t => new Models.App
        {
            Name = t,
            Publisher = GenerateRandomString(10)
        }).ToList();
        
        LoadList();
    }
    
    public async void LoadList(TaskCompletionSource<bool>? taskCompletionSource = null, string text = "")
    {
        if (AppsList.Any())
        {
            AppsList.Clear();
        }

        var cpuThreads = Environment.ProcessorCount;
        var batchSize = cpuThreads * 2;
        var sleepTime = cpuThreads <= 4 ? cpuThreads : cpuThreads / 2;

        var batchedList = BatchedAppsList;
        
        if (!string.IsNullOrWhiteSpace(text))
        {
            batchedList = BatchedAppsList.Where(t => t.Name!.Contains(text, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }
        
        for (var i = 0; i < batchedList.Count; i += batchSize)
        {
            var batch = batchedList.Skip(i).Take(batchSize).ToList();

            await Task.Run(() =>
            {
                foreach (var t in batch)
                {
                    Application.Current.Dispatcher.InvokeAsync(() => AppsList.Add(t), DispatcherPriority.Render);
                }
            });

            await Task.Delay(sleepTime); 
        }
 
        AppNames = new ObservableCollection<string>(AppsList.Select(app => app.Name)!);
        
        if (taskCompletionSource == null)
        {
            SortAppsList(0);
            return;
        }

        taskCompletionSource.SetResult(true);
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

    public void SortAppsList(int parameter)
    {
        NoBackgroundActionsRunning = false;
        var selectedIndex = parameter + 1;

        AppsList = selectedIndex switch
        {
            1 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.Name)),
            2 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.Name)),
            3 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.Publisher)),
            4 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.Publisher)),
            5 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.AppSize)),
            6 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.AppSize)),
            7 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.InstallDate)),
            8 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.InstallDate)),
            _ => AppsList
        };

        NoBackgroundActionsRunning = true;
    }
}