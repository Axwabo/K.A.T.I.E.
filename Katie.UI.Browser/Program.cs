using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

internal static class Program
{

    private static Task Main(string[] args)
    {
        IPhraseProvider.IsBrowser = true;
        return BuildAvaloniaApp()
            .WithInterFont()
            .StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>();

}
