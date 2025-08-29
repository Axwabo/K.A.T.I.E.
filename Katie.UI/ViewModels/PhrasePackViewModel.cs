using System.Collections.ObjectModel;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    public required IPhraseProvider? PhraseProvider { get; init; }

    public IPhraseCacheManager? Cache { get; init; }

    public required string Language { get; set; }

    public ObservableCollection<SamplePhraseBase> List { get; } = [];

    public CancellationToken Cancellation { get; set; }

    public event Action? PhrasesChanged;

    [ObservableProperty]
    private string? _blockingOperation;

    [RelayCommand]
    private Task AddPhrases() => PhraseProvider == null ? Task.CompletedTask : AddPhrases(PhraseProvider);

    public async Task AddPhrases(IPhraseProvider provider)
    {
        BlockingOperation = "Adding phrases...";
        try
        {
            var any = false;
            await foreach (var phrase in provider.EnumeratePhrasesAsync().WithCancellation(Cancellation))
            {
                List.Add(phrase);
                any = true;
            }

            if (any)
                PhrasesChanged?.InvokeOnUIThread();
        }
        finally
        {
            Dispatcher.UIThread.Post(() => BlockingOperation = null);
        }
    }

    public void ReplacePhrases(IReadOnlyCollection<SamplePhraseBase> phrases)
    {
        List.Clear();
        foreach (var phrase in phrases)
            List.Add(phrase);
        PhrasesChanged?.Invoke();
    }

    [RelayCommand]
    public async Task CacheAll()
    {
        BlockingOperation = "Caching phrases...";
        var any = false;
        await foreach (var task in Task.WhenEach(List.ToSamplePhrases(Language, Cache)).WithCancellation(Cancellation))
        {
            any = true;
            var (index, phrase) = task.Result;
            Dispatcher.UIThread.Post(() => List[index] = phrase);
        }

        if (any)
            Dispatcher.UIThread.Post(() => BlockingOperation = null);
        else
            BlockingOperation = null; // avoid one frame delay
    }

    [RelayCommand]
    private void RemovePhrase(SamplePhraseBase phrase)
    {
        if (phrase is not WaveStreamPhrase wave)
        {
            List.Remove(phrase);
            return;
        }

        wave.Dispose();
        List.Remove(wave);
        PhrasesChanged?.Invoke();
    }

    [RelayCommand]
    private void Clear()
    {
        foreach (var phrase in List)
            if (phrase is WaveStreamPhrase stream)
                stream.Dispose();
        List.Clear();
        PhrasesChanged?.Invoke();
    }

}
