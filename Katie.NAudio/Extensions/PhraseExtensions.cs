using Katie.NAudio.Phrases;

namespace Katie.NAudio.Extensions;

public static class PhraseExtensions
{

    public static SamplePhraseBase ToSamplePhrase(this WaveStreamPhrase phrase)
    {
        var buffer = new float[(int) (phrase.Duration.TotalSeconds * PhraseChain.SamplesToSeconds)];
        var length = phrase.ToSampleProvider().Read(buffer, 0, buffer.Length);
        return new RawSourceSamplePhrase(new RawSourceSampleProvider(PhraseChain.Format, buffer, length), phrase.Text);
    }

}
