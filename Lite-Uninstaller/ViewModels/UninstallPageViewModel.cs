using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
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

    private async void InitializeViewModel()
    {
        await SetupList();
        IsInitialized = NoBackgroundActionsRunning = true;
    }

    public Task SetupList()
    {
        return Task.Run(() =>
        {

            foreach (var app in GetWin32Apps())
            {
                BatchedAppsList.Add(app);
            }
            
            LoadList();
        });
    }

    // https://stackoverflow.com/a/58147170/22198018
    private static List<Models.App> GetWin32Apps()
    {
        var installs = new List<Models.App>();
        var keys = new List<string>
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
        };

        FindWin32Installs(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64), keys, installs);
        FindWin32Installs(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64), keys, installs);

        installs = installs.Where(app => !string.IsNullOrWhiteSpace(app.Name)).Distinct().ToList();
        installs.Sort((app1, app2) => string.Compare(app1.Name, app2.Name, StringComparison.Ordinal));

        return installs;
    }
    
    // https://stackoverflow.com/a/58147170/22198018
    private static void FindWin32Installs(RegistryKey regKey, List<string> keys, List<Models.App> installed)
    {
        foreach (var key in keys)
        {
            using var rk = regKey.OpenSubKey(key);
            if (rk == null)
            {
                continue;
            }

            foreach (var skName in rk.GetSubKeyNames())
            {
                using var sk = rk.OpenSubKey(skName);
                try
                {
                    var tryParse = int.TryParse(sk.GetValue("InstallDate").ToString(), out var numericDate);
                    var dateTime = "Unknown";

                    if (tryParse)
                    {
                        var year = numericDate / 10000;
                        var month = numericDate / 100 % 100;
                        var day = numericDate % 100;
                        dateTime = new DateTime(year, month, day).ToShortDateString();
                    }

                    var app = new Models.App
                    {
                        Name = Convert.ToString(sk.GetValue("DisplayName")),
                        Publisher = sk.GetValue("Publisher") as string ?? "Unknown",
                        Version = Convert.ToString(sk.GetValue("DisplayVersion")),
                        InstallPath = Convert.ToString(sk.GetValue("InstallLocation")),
                        AppSize = sk.GetValue("EstimatedSize") is int sizeValue ? sizeValue.ToString() : "Unknown",
                        AppSizeLong = Convert.ToInt64(sk.GetValue("EstimatedSize")),
                        InstalledDate = dateTime,
                        Type = Models.App.AppType.Desktop
                    };

                    installed.Add(app);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
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
            5 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.AppSizeLong)),
            6 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.AppSizeLong)),
            7 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.InstalledDate)),
            8 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.InstalledDate)),
            _ => AppsList
        };

        NoBackgroundActionsRunning = true;
    }
}