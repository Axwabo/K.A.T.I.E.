using System.IO;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Desktop;

public static class ServiceProviderExtensions
{

    private const string Samples = "samples";
    private const string Signals = "signals";

    public static IServiceCollection AddInitialProviders(this IServiceCollection builder)
    {
        if (Directory.Exists(Samples))
            foreach (var directory in Directory.EnumerateDirectories(Samples))
                builder.AddSingleton<IPhraseProvider>(new DirectoryPhraseProvider(directory));
        if (Directory.Exists(Signals))
            builder.AddSingleton<ISignalProvider>(new DirectorySignalProvider(Signals));
        return builder;
    }

}
