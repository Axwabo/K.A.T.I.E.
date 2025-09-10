namespace Katie.UI.PhraseProviders;

public interface IPhraseProvider
{

    IAsyncEnumerable<WavePhraseBase> EnumeratePhrasesAsync();

}
