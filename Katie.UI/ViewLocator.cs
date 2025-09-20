using Avalonia.Controls.Templates;

namespace Katie.UI;

public sealed class ViewLocator : IDataTemplate
{

    public Control? Build(object? param) => param switch
    {
        null => null,
        MainViewModel main => Create<MainView>(main),
        PhrasesPageViewModel phrases => Create<PhrasesPageView>(phrases),
        PhrasePackViewModel pack => Create<PhrasePackView>(pack),
        SignalsViewModel signals => Create<SignalsView>(signals),
        CacheManagerViewModel cache => Create<CacheManagerView>(cache),
        InspectPageViewModel inspect => Create<InspectPageView>(inspect),
        QueuePageViewModel queue => Create<QueuePageView>(queue),
        ExportPageViewModel export => Create<ExportPageView>(export),
        _ => new TextBlock {Text = "Not found: " + param}
    };

    public bool Match(object? data) => data is ViewModelBase;

    private static T Create<T>(object context) where T : Control, new()
        => new() {DataContext = context};

}
