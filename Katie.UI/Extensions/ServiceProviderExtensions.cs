using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Extensions;

internal static class ServiceProviderExtensions
{

    public static IServiceCollection AddViewModels(this IServiceCollection collection)
        => collection.AddSingleton<MainViewModel>()
            .AddSingleton<PhrasesPageViewModel>()
            .AddSingleton<CacheManagerViewModel>()
            .AddSingleton<SignalsViewModel>()
            .AddSingleton<InspectPageViewModel>()
            .AddSingleton<QueuePageViewModel>()
            .AddSingleton<ExportPageViewModel>();

}
