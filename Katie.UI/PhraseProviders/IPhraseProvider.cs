namespace Katie.UI.PhraseProviders;

public interface IPhraseProvider
{

    string Language { get; }

    IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync();

}
