using Katie.UI.PhraseProviders;
using Katie.UI.Services;

namespace Katie.UI.ViewModels;

public sealed class CacheManagerViewModel : ViewModelBase
{

    private readonly PhraseManager? _phraseManager;

    private readonly IPhraseCacheManager? _cacheManager;

    public string Info { get; }

    public CacheManagerViewModel(PhraseManager? phraseManager, IPhraseCacheManager? cacheManager = null)
    {
        _phraseManager = phraseManager;
        _cacheManager = cacheManager;
        Info = cacheManager?.Info ?? "ℹ Caching reads all phrases into memory, releasing the file handles.";
    }

    public CacheManagerViewModel() : this(null)
    {
    }

}
