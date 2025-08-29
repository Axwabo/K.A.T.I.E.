using Katie.UI.PhraseProviders;

namespace Katie.UI.Extensions;

using IndexedPhrase = (int Index, SamplePhraseBase Phrase);

public static class PhraseExtensions
{

    public static RawSourceSamplePhrase ToSamplePhrase(this WaveStreamPhrase phrase)
        => new(phrase.ToSampleProvider().ReadSamples(phrase.Duration), phrase.Text);

    public static IEnumerable<Task<IndexedPhrase>> ToSamplePhrases(this IReadOnlyList<SamplePhraseBase> phrases, string language, IPhraseCacheManager? saver)
    {
        for (var i = 0; i < phrases.Count; i++)
        {
            var phrase = phrases[i];
            if (phrase is WaveStreamPhrase streamPhrase)
                yield return CacheAndConvert(i, streamPhrase, language, saver);
        }
    }

    private static async Task<IndexedPhrase> CacheAndConvert(int index, WaveStreamPhrase streamPhrase, string language, IPhraseCacheManager? saver)
    {
        if (saver != null)
            await saver.CacheAsync(streamPhrase, language);
        var samplePhrase = await Task.Run(streamPhrase.ToSamplePhrase);
        streamPhrase.Dispose();
        return (index, samplePhrase);
    }

}
