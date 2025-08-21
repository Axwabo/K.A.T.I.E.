namespace Katie.UI.Extensions;

public static class PhraseExtensions
{

    public static RawSourceSamplePhrase ToSamplePhrase(this WaveStreamPhrase phrase)
    {
        var total = 0;
        var provider = phrase.ToSampleProvider();
        var buffer = new float[(int) (phrase.Duration.TotalSeconds * provider.WaveFormat.AverageBytesPerSecond / 4)];
        int read;
        while ((read = provider.Read(buffer, total, Math.Min(4800, buffer.Length - total))) != 0)
            total += read;
        return new RawSourceSamplePhrase(new RawSourceSampleProvider(provider.WaveFormat, buffer, total), phrase.Text);
    }

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
