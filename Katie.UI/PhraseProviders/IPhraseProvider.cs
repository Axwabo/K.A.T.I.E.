namespace Katie.UI.PhraseProviders;

public interface IPhraseProvider
{

    IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync();

    public static Dictionary<string, IPhraseProvider> InitialProviders { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static Task LoadInitialPhrases(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global) => Task.WhenAll(
        Add("Hungarian", hungarian),
        Add("English", english),
        Add("Global", global)
    );

    private static Task Add(string key, PhrasePackViewModel pack)
        => InitialProviders.TryGetValue(key, out var provider)
            ? pack.AddPhrases(provider)
            : Task.CompletedTask;

}
