using Katie.NAudio;
using Katie.NAudio.Extensions;
using NAudio.Wave;

namespace Katie.UI;

public sealed class RawSourceSamplePhrase : WavePhraseBase
{

    private readonly RawSourceSampleProvider _provider;

    public RawSourceSamplePhrase(RawSourceSampleProvider provider, string text)
    {
        _provider = provider;
        Text = text;
        Duration = provider.SamplesToTime(provider.Length);
    }

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public override SimpleWaveFormat WaveFormat => _provider.WaveFormat;

    public override ISampleProvider ToSampleProvider() => _provider.Copy();

}
