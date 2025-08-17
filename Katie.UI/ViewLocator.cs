using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Katie.UI.ViewModels;
using Katie.UI.Views;

namespace Katie.UI;

public sealed class ViewLocator : IDataTemplate
{

    public Control? Build(object? param) => param switch
    {
        null => null,
        MainViewModel main => Create<MainView>(main),
        PhrasePackViewModel pack => Create<PhrasePackView>(pack),
        _ => new TextBox {Text = "Not found: " + param}
    };

    public bool Match(object? data) => data is ViewModelBase;

    private static T Create<T>(object context) where T : Control, new()
        => new() {DataContext = context};

}
