using Katie.NAudio.Phrases;

namespace Katie.NAudio.Extensions;

public static class PhraseExtensions
{

    public static RawSourceSamplePhrase ToSamplePhrase(this WaveStreamPhrase phrase)
    {
        var buffer = new float[(int) (phrase.Duration.TotalSeconds * PhraseChain.SampleRate)];
        var total = 0;
        var provider = phrase.ToSampleProvider();
        int read;
        while ((read = provider.Read(buffer, total, Math.Min(4800, buffer.Length - total))) != 0)
            total += read;
        return new RawSourceSamplePhrase(new RawSourceSampleProvider(PhraseChain.Format, buffer, total), phrase.Text);
    }

}
