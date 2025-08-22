using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

internal static class Program
{

    private static async Task Main()
    {
        await JSHost.ImportAsync(WebAudioFunctions.Module, $"/{WebAudioFunctions.Module}.js");

        IPhraseProvider.IsBrowser = true;
        IAudioPlayer.Factory = provider =>
        {
            WebAudioPlayer.Provider = provider;
            return WebAudioPlayer.Instance;
        };

        await BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

}
