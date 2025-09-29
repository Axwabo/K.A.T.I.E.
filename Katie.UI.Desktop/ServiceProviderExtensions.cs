using System.IO;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Desktop;

public static class ServiceProviderExtensions
{

    private const string Phrases = "Phrases";
    private const string Signals = "Signals";

    public static IServiceCollection AddInitialProviders(this IServiceCollection builder)
    {
        if (Directory.Exists(Phrases))
            builder.AddSingleton<IInitialPhraseLoader>(new DirectoryPhraseLoader {Root = Phrases});
        if (Directory.Exists(Signals))
            builder.AddSingleton<ISignalProvider>(new DirectorySignalProvider(Signals));
        return builder;
    }

}
