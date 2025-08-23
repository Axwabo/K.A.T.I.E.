using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Katie.UI.Audio;
using Katie.UI.PhraseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Browser;

internal static class Program
{

    private static async Task Main()
    {
        await JSHost.ImportAsync(WebAudioFunctions.Module, $"/{WebAudioFunctions.Module}.js");
        await JSHost.ImportAsync(CacheFunctions.Module, $"/{CacheFunctions.Module}.js");

        await BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App {Services = CreateServiceCollection()})
            .WithInterFont();

    private static IServiceCollection CreateServiceCollection() => new ServiceCollection()
        .AddSingleton<IAudioPlayerFactory, WebAudioFactory>()
        .AddSingleton<IFileToPhraseConverter, MemoryPhraseConverter>()
        .AddSingleton<IPhraseCacheSaver, PhraseCacheStorageSaver>();

}
