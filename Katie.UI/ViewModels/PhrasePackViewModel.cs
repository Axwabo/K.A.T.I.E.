using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio.Extensions;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    private readonly ObservableCollection<WavePhraseBase> _list = [];

    public required string Language { get; set; }

    public IPhraseProvider? Picker { get; init; }

    public IPhraseCacheManager? Cache { get; init; }

    public IReadOnlyList<WavePhraseBase> List => _list;

    public CancellationToken Cancellation { get; set; }

    public event Action? PhrasesChanged;

    [ObservableProperty]
    private string? _blockingOperation;

    [RelayCommand]
    private Task AddPhrases() => Picker == null ? Task.CompletedTask : AddPhrases(Picker);

    public void Add(WavePhraseBase phrase)
    {
        _list.Add(phrase);
        PhrasesChanged?.Invoke();
    }

    public async Task AddPhrases(IPhraseProvider provider)
    {
        BlockingOperation = "Adding phrases...";
        try
        {
            var any = false;
            await foreach (var phrase in provider.EnumeratePhrasesAsync().WithCancellation(Cancellation))
            {
                _list.Add(phrase);
                any = true;
            }

            if (any)
                PhrasesChanged?.InvokeOnUIThread();
        }
        finally
        {
            CompleteOperation();
        }
    }

    public void ReplacePhrases(IReadOnlyCollection<WavePhraseBase> phrases)
    {
        _list.Clear();
        foreach (var phrase in phrases)
            _list.Add(phrase);
        PhrasesChanged?.Invoke();
    }

    [RelayCommand]
    public async Task CacheAll()
    {
        BlockingOperation = "Caching phrases...";
        await foreach (var task in Task.WhenEach(List.ToSamplePhrases(Language, Cache)).WithCancellation(Cancellation))
        {
            var (index, phrase) = task.Result;
            Dispatcher.UIThread.Post(() =>
            {
                var previous = _list[index];
                _list[index] = phrase;
                for (var i = 0; i < _list.Count; i++)
                    if (_list[i] is WavePhraseAlias {Original: var original, Text: var text} && original == previous)
                        _list[i] = WavePhraseAlias.Create(phrase, text);
            });
        }

        CompleteOperation();
        PhrasesChanged?.InvokeOnUIThread();
    }

    [RelayCommand]
    public void Remove(WavePhraseBase phrase)
    {
        if (phrase is WaveStreamPhrase wave)
            wave.Dispose();
        for (var i = _list.Count - 1; i >= 0; i--)
            if (_list[i] == phrase || _list[i].IsAliasOf(phrase))
                _list.RemoveAt(i);
        PhrasesChanged?.Invoke();
    }

    [RelayCommand]
    private void Clear()
    {
        foreach (var phrase in List)
            if (phrase is WaveStreamPhrase stream)
                stream.Dispose();
        _list.Clear();
        PhrasesChanged?.Invoke();
    }

    public void EditOrCreateAlias(WavePhraseBase phrase, string text)
    {
        var index = _list.IndexOf(phrase);
        if (index == -1)
            return;
        var alias = WavePhraseAlias.Create(phrase, text);
        if (phrase is WavePhraseAlias)
            _list[index] = alias;
        else
            _list.Add(alias);
        PhrasesChanged?.Invoke();
    }

    private void CompleteOperation()
    {
        if (Dispatcher.UIThread.CheckAccess())
            BlockingOperation = null; // avoid one frame delay
        else
            Dispatcher.UIThread.Post(() => BlockingOperation = null);
    }

}
