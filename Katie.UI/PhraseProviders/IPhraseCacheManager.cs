namespace Katie.UI.PhraseProviders;

public interface IPhraseCacheManager
{

    string Info { get; }

    Task CacheAsync(WaveStreamPhrase phrase, string language);

    Task ClearAsync();

}
