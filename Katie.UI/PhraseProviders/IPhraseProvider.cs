using System;
using System.Collections.Generic;
using Katie.NAudio;
using Katie.UI.ViewModels;

namespace Katie.UI.PhraseProviders;

public interface IPhraseProvider
{

    IAsyncEnumerable<WavePhrase> EnumeratePhrasesAsync();

    public static Dictionary<string, IPhraseProvider> InitialProviders { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static void LoadInitialPhrases(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        Add("Hungarian", hungarian);
        Add("English", english);
        Add("Global", global);
    }

    private static void Add(string key, PhrasePackViewModel pack)
    {
        if (InitialProviders.TryGetValue(key, out var hungarianProvider))
            _ = pack.AddPhrases(hungarianProvider);
    }

}
