namespace Katie.UI.PhraseProviders;

public interface IPhraseCacheSaver
{

    Task CacheAsync(RawSourceSamplePhrase phrase);

}
