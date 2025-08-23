namespace Katie.UI.PhraseProviders;

public interface IPhraseProvider
{

    IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync();

}
