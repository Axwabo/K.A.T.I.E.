namespace Katie.UI.PhraseProviders;

public interface IPhraseCacheSaver
{

    Task CacheAsync(WaveStreamPhrase phrase, string language);

}
