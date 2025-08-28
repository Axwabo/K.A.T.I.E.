using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    public required IPhraseProvider? PhraseProvider { get; init; }

    public IPhraseCacheManager? Cache { get; init; }

    public required string Language { get; set; }

    public string Content => $"Add {Language} phrases";

    public ObservableCollection<SamplePhraseBase> List { get; } = [];

    public event Action? PhrasesChanged;

    [RelayCommand]
    private Task AddPhrases() => PhraseProvider == null ? Task.CompletedTask : AddPhrases(PhraseProvider);

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

    public void ReplacePhrases(IReadOnlyCollection<SamplePhraseBase> phrases)
    {
        List.Clear();
        foreach (var phrase in phrases)
            List.Add(phrase);
        PhrasesChanged?.Invoke();
    }

    public async Task CacheAll()
    {
        var list = new List<SamplePhraseBase>(List.Count);
        await foreach (var phrase in List.ToSamplePhrases(Language, Cache))
            list.Add(phrase);
        Dispatcher.UIThread.Post(() => ReplacePhrases(list));
    }

    [RelayCommand]
    private async Task RemovePhrase(SamplePhraseBase phrase)
    {
        if (phrase is not WaveStreamPhrase wave)
        {
            List.Remove(phrase);
            return;
        }

        if (Cache != null)
            await Cache.DeleteAsync(wave, Language);
        wave.Dispose();
        List.Remove(wave);
        PhrasesChanged?.InvokeOnUIThread();
    }

}
