using Katie.UI.PhraseProviders;

namespace Katie.UI.Extensions;

public static class PhraseExtensions
{

    public static RawSourceSamplePhrase ToSamplePhrase(this WaveStreamPhrase phrase)
        => new(phrase.ToSampleProvider().ReadSamples(phrase.Duration), phrase.Text);

    public static async IAsyncEnumerable<SamplePhraseBase> ToSamplePhrases(this IReadOnlyCollection<SamplePhraseBase> phrases, IPhraseCacheSaver? saver)
    {
        foreach (var phrase in phrases)
        {
            if (phrase is not WaveStreamPhrase streamPhrase)
            {
                yield return phrase;
                continue;
            }

            var samplePhrase = await Task.Run(() => streamPhrase.ToSamplePhrase());
            streamPhrase.Dispose();
            if (saver != null)
                await saver.CacheAsync(samplePhrase);
            yield return samplePhrase;
        }
    }

}
