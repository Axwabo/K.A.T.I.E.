using Avalonia;
using Katie.UI.Audio;
using Katie.UI.PhraseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Desktop;

internal static class Program
{

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App {Services = CreateServiceCollection()})
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static IServiceCollection CreateServiceCollection() => new ServiceCollection()
        .AddSingleton<IAudioPlayerFactory, SoundFlowFactory>()
        .AddSingleton<IFileToPhraseConverter, FileStreamToPhraseConverter>()
        .AddInitialProviders();

}
