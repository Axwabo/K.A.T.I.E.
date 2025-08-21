namespace Katie.UI.Extensions;

public static class PhraseExtensions
{

    public static RawSourceSamplePhrase ToSamplePhrase(this WaveStreamPhrase phrase)
        => new(phrase.ToSampleProvider().ReadSamples(phrase.Duration), phrase.Text);

    public static List<SamplePhraseBase> ToSamplePhrases(this IReadOnlyCollection<SamplePhraseBase> phrases)
    {
        var list = new List<SamplePhraseBase>(phrases.Count);
        foreach (var phrase in phrases)
        {
            if (phrase is not WaveStreamPhrase streamPhrase)
            {
                list.Add(phrase);
                continue;
            }

            var samplePhrase = streamPhrase.ToSamplePhrase();
            streamPhrase.Dispose();
            list.Add(samplePhrase);
        }

        return list;
    }

}
