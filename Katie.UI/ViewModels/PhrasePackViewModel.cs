using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    private FilePickerPhraseProvider? _fileProvider;

    public Control? Host { get; init; }

    public required string Language { get; set; }

    public string Content => $"Add {Language} phrases";

    public ObservableCollection<SamplePhraseBase> List { get; } = [];

    public event Action? PhrasesChanged;

    [RelayCommand]
    private async Task AddPhrases()
    {
        if (TopLevel.GetTopLevel(Host) is {StorageProvider: {CanOpen: true} storage})
            await AddPhrases(_fileProvider ??= new FilePickerPhraseProvider(storage));
    }

    public async Task AddPhrases(IPhraseProvider provider)
    {
        var any = false;
        await foreach (var phrase in provider.EnumeratePhrasesAsync())
        {
            List.Add(phrase);
            any = true;
        }

        if (any)
            PhrasesChanged?.InvokeOnUIThread();
    }

    public Task Cache() => Task.Run(() =>
    {
        var phrases = List.ToSamplePhrases();
        Dispatcher.UIThread.Post(() =>
        {
            for (var i = 0; i < List.Count; i++)
                List[i] = phrases[i];
            PhrasesChanged?.Invoke();
        });
    });

}
