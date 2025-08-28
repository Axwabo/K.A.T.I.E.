namespace Katie.UI.PhraseProviders;

public interface IPhraseCacheManager
{

    Task CacheAsync(WaveStreamPhrase phrase, string language);

    Task DeleteAsync(WaveStreamPhrase phrase, string language);

}
