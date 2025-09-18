using CommunityToolkit.Mvvm.Input;
using Katie.UI.PhraseProviders;
using Katie.UI.Services;
using Katie.UI.Signals;

namespace Katie.UI.ViewModels;

public sealed partial class CacheManagerViewModel : ViewModelBase
{

    public static string BrowserInfo => "Caching saves phrases and signals to the browser's cache storage, persisting them across sessions.";

    public static string DefaultInfo => "ℹ Caching reads all phrases into memory, releasing the file handles.";

    public PhraseManager Phrases { get; }

    public SignalManager Signals { get; }

    public IPhraseCacheManager? PhraseCache { get; }

    public ISignalCacheManager? SignalCache { get; }

    public CacheManagerViewModel(PhraseManager phraseManager, SignalManager signals, IPhraseCacheManager? phraseCacheManager = null, ISignalCacheManager? signalCacheManager = null)
    {
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

}
