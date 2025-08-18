using System;
using System.IO;
using Avalonia;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

internal static class Program
{

    private const string Samples = "samples";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (Directory.Exists(Samples))
            foreach (var directory in Directory.EnumerateDirectories(Samples))
                IPhraseProvider.InitialProviders[Path.GetFileName(directory)] = new DirectoryPhraseProvider(directory);
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
