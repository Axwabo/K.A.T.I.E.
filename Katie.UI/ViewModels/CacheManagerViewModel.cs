using CommunityToolkit.Mvvm.Input;
using Katie.UI.PhraseProviders;
using Katie.UI.Services;

namespace Katie.UI.ViewModels;

public sealed partial class CacheManagerViewModel : ViewModelBase
{

    public PhraseManager Phrases { get; }

    public IPhraseCacheManager? Cache { get; }

    public string Info { get; }

    public CacheManagerViewModel(PhraseManager phraseManager, IPhraseCacheManager? cacheManager = null)
    {
        Phrases = phraseManager;
        Cache = cacheManager;
        Info = cacheManager?.Info ?? "ℹ Caching reads all phrases into memory, releasing the file handles.";
    }

    public CacheManagerViewModel() : this(new PhraseManager())
    {
    }

    [RelayCommand]
    private async Task CacheEverything() => await Task.WhenAll(
        Phrases.Hungarian.CacheAll(Cache),
        Phrases.English.CacheAll(Cache),
        Phrases.Global.CacheAll(Cache)
    );

}
