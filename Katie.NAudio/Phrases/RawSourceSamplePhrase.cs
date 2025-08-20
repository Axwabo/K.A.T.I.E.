using NAudio.Wave;

namespace Katie.NAudio.Phrases;

public sealed class RawSourceSamplePhrase : SamplePhraseBase
{

    private readonly RawSourceSampleProvider _provider;

    public RawSourceSamplePhrase(RawSourceSampleProvider provider, string text)
    {
        if (!provider.WaveFormat.Equals(PhraseChain.Format))
            throw new ArgumentException("Wave format must match PhraseChain.Format");
        _provider = provider;
        Text = text;
        Duration = TimeSpan.FromSeconds(provider.Length * PhraseChain.SamplesToSeconds);
    }

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public override ISampleProvider ToSampleProvider() => _provider.Copy();

}
