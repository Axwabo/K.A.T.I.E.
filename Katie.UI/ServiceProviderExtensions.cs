using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI;

internal static class ServiceProviderExtensions
{

    public static IServiceCollection AddViewModels(this IServiceCollection collection)
        => collection.AddSingleton<MainViewModel>()
            .AddSingleton<SignalsViewModel>();

}
