using Katie.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Extensions;

internal static class ServiceProviderExtensions
{

    public static IServiceCollection AddViewModels(this IServiceCollection collection)
        => collection.AddSingleton<MainViewModel>()
            .AddSingleton<PhrasesPageViewModel>()
            .AddSingleton<CacheManagerViewModel>()
            .AddTransient<SignalsViewModel>();

    private static IServiceCollection AddView<TModel, TView>(this IServiceCollection collection)
        where TModel : ViewModelBase
        where TView : UserControl, new()
        => collection.AddSingleton<TModel>()
            .AddSingleton<IViewFactory>(new GenericViewFactory<TView>());

    private static IServiceCollection AddTransientView<TModel, TView>(this IServiceCollection collection)
        where TModel : ViewModelBase
        where TView : UserControl, new()
        => collection.AddTransient<TModel>()
            .AddSingleton<IViewFactory>(new GenericViewFactory<TView>());

}

file sealed class GenericViewFactory<T> : IViewFactory where T : UserControl, new()
{

    public Type ViewModelType { get; } = typeof(T);

    public Control Build(ViewModelBase viewModel) => new T {DataContext = viewModel};

}
