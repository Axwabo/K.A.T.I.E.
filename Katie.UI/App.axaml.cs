using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Katie.UI.PhraseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI;

public sealed class App : Application
{

    public required IServiceCollection Services { get; init; }

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        Services.AddViewModels()
            .AddSingleton<StorageWrapper>()
            .AddKeyedSingleton<IPhraseProvider, FilePickerPhraseProvider>(nameof(FilePickerPhraseProvider));

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var window = new MainWindow();
            Services.AddSingleton(new HostControl(window));
            Services.AddSingleton<MainWindowViewModel>();

            var provider = Services.BuildServiceProvider();

            window.DataContext = provider.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = window;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            var view = new MainView();
            Services.AddSingleton(new HostControl(view));

            var provider = Services.BuildServiceProvider();

            view.DataContext = provider.GetRequiredService<MainViewModel>();
            singleViewPlatform.MainView = view;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

}
