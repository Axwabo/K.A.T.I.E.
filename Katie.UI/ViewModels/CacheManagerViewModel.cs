using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed class CacheManagerViewModel : ViewModelBase
{

    private readonly IPhraseCacheManager? _cacheManager;

    public CacheManagerViewModel(IPhraseCacheManager? cacheManager = null) => _cacheManager = cacheManager;

}
