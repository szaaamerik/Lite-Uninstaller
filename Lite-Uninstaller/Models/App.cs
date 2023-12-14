namespace Lite_Uninstaller.Models;

public class App
{
    public enum AppType
    {
        Unknown = 0,
        UWP,
        Hidden,
        Desktop
    }

    public string? Name { get; init; } = "Unknown";
    public string? Publisher { get; init; } = "Unknown";
    public string? Version { get; init; } = "Unknown";
    public string? InstallPath { get; init; } = "Unknown";
    public AppType Type { get; init; } = AppType.Unknown;

    public bool IsSystemApp { get; init; } = false;
    public bool IsStartupApp { get; init; } = false;

    public string? ImageSource { get; init; } = "";

    public DateTime InstallDate { get; init; } = new DateTime(1969, 1, 1);

    public long AppSize { get; init; } = 0L;
    public string AppSizeInGigaBytes { get; init; } = "124 GB";
}