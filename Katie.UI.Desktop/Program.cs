using System.IO;
using Avalonia;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;

namespace Katie.UI.Desktop;

internal static class Program
{

    private const string Samples = "samples";
    private const string Signals = "signals";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (Directory.Exists(Samples))
            foreach (var directory in Directory.EnumerateDirectories(Samples))
                IPhraseProvider.InitialProviders[Path.GetFileName(directory)] = new DirectoryPhraseProvider(directory);
        if (Directory.Exists(Signals))
            ISignalProvider.InitialProvider = new DirectorySignalProvider(Signals);
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
