using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Katie.UI.Audio;
using Katie.UI.Browser.Caching;
using Katie.UI.Browser.JSInterop;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Browser;

internal static class Program
{

    private static async Task Main()
    {
        await JSHost.ImportAsync(WebAudioFunctions.Module, $"/K.A.T.I.E./{WebAudioFunctions.Module}.js");
        await JSHost.ImportAsync(PhraseCacheFunctions.Module, $"/K.A.T.I.E./{PhraseCacheFunctions.Module}.js");
        await JSHost.ImportAsync(SignalCacheFunctions.Module, $"/K.A.T.I.E./{SignalCacheFunctions.Module}.js");

        await BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure(() => new App {Services = CreateServiceCollection()})
            .WithInterFont();

    private static IServiceCollection CreateServiceCollection() => new ServiceCollection()
        .AddSingleton<IAudioPlayerFactory, WebAudioFactory>()
        .AddSingleton<IStreamToPhraseConverter, MemoryPhraseConverter>()
        .AddSingleton<IPhraseCacheManager, CacheStoragePhraseManager>()
        .AddSingleton<IInitialPhraseLoader, CacheStoragePhraseLoader>()
        .AddSingleton<ISignalProvider, CacheStorageSignalLoader>()
        .AddSingleton<ISignalCacheManager, CacheStorageSignalManager>()
        .AddSingleton<IZipOpener, BrowserZipOpener>();

}
