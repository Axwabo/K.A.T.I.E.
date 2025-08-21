using Katie.NAudio;
using Katie.NAudio.Phrases;
using NAudio.Wave;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Providers;

namespace Katie.SecretLab;

public sealed class RawSourcePhrase : SamplePhraseBase
{

    private readonly RawSourceSampleProvider _provider;

    public RawSourcePhrase(RawSourceSampleProvider provider, string text)
    {
        if (!provider.WaveFormat.Equals(AudioPlayer.SupportedFormat))
            throw new ArgumentException("Wave format must match AudioPlayer.SupportedFormat");
        _provider = provider;
        Text = text;
        Duration = provider.TotalTime;
    }

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public override SimpleWaveFormat WaveFormat => _provider.WaveFormat;

    public override ISampleProvider ToSampleProvider() => _provider.Copy();

}
