using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.PhraseProviders;
using Katie.UI.Services;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class CacheManagerViewModel : ViewModelBase
{

    private readonly IInitialPhraseLoader? _zipLoader;

    public static string BrowserInfo => "Caching saves phrases and signals to the browser's cache storage, persisting them across sessions.";

    public static string DefaultInfo => "ℹ Caching reads all phrases into memory, releasing the file handles.";

    public PhraseManager Phrases { get; }

    public SignalManager Signals { get; }

    public IPhraseCacheManager? PhraseCache { get; }

    public ISignalCacheManager? SignalCache { get; }

    public CacheManagerViewModel(
        PhraseManager phraseManager,
        SignalManager signals,
        IPhraseCacheManager? phraseCacheManager = null,
        ISignalCacheManager? signalCacheManager = null,
        [FromKeyedServices(nameof(ZipPhraseLoader))]
        IInitialPhraseLoader? zipLoader = null
    )
    {
        _zipLoader = zipLoader;
        Phrases = phraseManager;
        Signals = signals;
        PhraseCache = phraseCacheManager;
        SignalCache = signalCacheManager;
    }

    public CacheManagerViewModel() : this(new PhraseManager(), new SignalManager())
    {
    }

    [RelayCommand]
    private Task Cache(PhrasePackViewModel pack) => pack.CacheAll(PhraseCache);

    [RelayCommand]
    private Task CacheSignals() => Signals.CacheAll(SignalCache);

    [RelayCommand]
    private async Task CacheEverything() => await Task.WhenAll(
        Phrases.Hungarian.CacheAll(PhraseCache),
        Phrases.English.CacheAll(PhraseCache),
        Phrases.Global.CacheAll(PhraseCache),
        Signals.CacheAll(SignalCache)
    );

    [RelayCommand]
    private Task LoadArchive() => _zipLoader == null
        ? Task.CompletedTask
        : _zipLoader.LoadPhrasesAsync(Phrases.Hungarian, Phrases.English, Phrases.Global);

    [RelayCommand]
    private async Task ClearPhraseCache()
    {
        if (PhraseCache == null)
            return;
        await PhraseCache.ClearAsync();
        Phrases.Hungarian.UnCache();
        Phrases.English.UnCache();
        Phrases.Global.UnCache();
    }

    [RelayCommand]
    private Task ClearSignalCache() => SignalCache?.ClearAsync() ?? Task.CompletedTask;

}

file sealed class NotCachedPhrase : WavePhraseBase
{

    private readonly WavePhraseBase _phrase;

    public NotCachedPhrase(WavePhraseBase phrase) => _phrase = phrase;

    public override string Text => _phrase.Text;
    public override TimeSpan Duration => _phrase.Duration;
    public override SimpleWaveFormat WaveFormat => _phrase.WaveFormat;
    public override ISampleProvider ToSampleProvider() => _phrase.ToSampleProvider();

}

file static class Extensions
{

    public static void UnCache(this PhrasePackViewModel phrases)
    {
        var list = new List<WavePhraseBase>(phrases.List.Count);
        foreach (var phrase in phrases.List)
            list.Add(phrase as NotCachedPhrase ?? new NotCachedPhrase(phrase));
        phrases.ReplacePhrases(list);
    }

}
