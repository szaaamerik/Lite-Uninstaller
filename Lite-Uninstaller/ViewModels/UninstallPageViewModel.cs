using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

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

    [ObservableProperty] 
    private bool _multipleAppsSelected;
    
    [ObservableProperty] 
    private bool _noAppSelected;

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
        var taskCompletionSource = new TaskCompletionSource<bool>();
        await SetupList(taskCompletionSource);
        await taskCompletionSource.Task;
        IsInitialized = NoBackgroundActionsRunning = true;
    }

    public Task SetupList(TaskCompletionSource<bool> taskCompletionSource)
    {
        BatchedAppsList.Clear();
        
        foreach (var app in GetWin32Apps())
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {   
                BatchedAppsList.Add(app);
            }).Wait();
        }
        
        LoadList();
        taskCompletionSource.SetResult(true);
        return Task.CompletedTask;
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
                    DateTime dateTime;
                    string dateTimeString;

                    try
                    {
                        int.TryParse(sk.GetValue("InstallDate").ToString(), out var numericDate);
                        var year = numericDate / 10000;
                        var month = numericDate / 100 % 100;
                        var day = numericDate % 100;
                        dateTime = new DateTime(year, month, day);
                        dateTimeString = dateTime.ToShortDateString();
                    }
                    catch
                    {
                        dateTime = new DateTime(1969, 12, 1);
                        dateTimeString = "Unknown";
                    }
                    
                    BitmapSource? iconBitmapSource;
                    
                    try
                    {
                        var iconParse = (sk.GetValue("DisplayIcon") as string)?.Replace(",0", "");
                        var appIcon = Icon.ExtractAssociatedIcon(iconParse!);
                        iconBitmapSource = Imaging.CreateBitmapSourceFromHIcon(appIcon!.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    catch (Exception)
                    {
                        var explorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                        var defaultExeIcon = Icon.ExtractAssociatedIcon(explorerPath);
                        iconBitmapSource = Imaging.CreateBitmapSourceFromHIcon(defaultExeIcon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    }
                    
                    var app = new Models.App
                    {
                        Name = Convert.ToString(sk.GetValue("DisplayName")),
                        Publisher = sk.GetValue("Publisher") as string ?? "Unknown",
                        Version = Convert.ToString(sk.GetValue("DisplayVersion")),
                        InstallPath = Convert.ToString(sk.GetValue("InstallLocation")),
                        ImageSource = iconBitmapSource,
                        InstalledDate = dateTime,
                        InstalledDateString = dateTimeString,
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
            5 => new ObservableCollection<Models.App>(AppsList.OrderBy(app => app.InstalledDate)),
            6 => new ObservableCollection<Models.App>(AppsList.OrderByDescending(app => app.InstalledDate)),
            _ => AppsList
        };

        NoBackgroundActionsRunning = true;
    }

    public static void OpenInstallPath(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        Process.Start("explorer.exe", path);
    }
}