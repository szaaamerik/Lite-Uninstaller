namespace Lite_Uninstaller.Models;

public class App
{
    public enum AppType
    {
        Unknown = 0,
        UWP,
        Hidden,
        System,
        Desktop
    }

    public string? Name { get; init; } = "Unknown";
    public string? Publisher { get; init; } = "Unknown";
    public string? Version { get; init; } = "Unknown";
    public string? InstallPath { get; init; } = "Unknown";
    public AppType Type { get; init; } = AppType.Unknown;

    public bool IsStartupApp { get; init; } = false;

    public string? ImageSource { get; init; } = "";

    public string? InstalledDate { get; init; } = "Unknown";

    public string? AppSize { get; init; }
    public long AppSizeLong { get; init; } = 0L;
}