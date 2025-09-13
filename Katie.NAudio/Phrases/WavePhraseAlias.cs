using NAudio.Wave;

namespace Katie.NAudio.Phrases;

public sealed class WavePhraseAlias : WavePhraseBase
{

    public static WavePhraseAlias Create(WavePhraseBase phrase, string text)
        => phrase is WavePhraseAlias alias
            ? new WavePhraseAlias(alias.Original, text)
            : new WavePhraseAlias(phrase, text);

    private WavePhraseAlias(WavePhraseBase original, string text)
    {
        Original = original;
        Text = text;
    }

    public WavePhraseBase Original { get; }

    public override string Text { get; }

    public override TimeSpan Duration => Original.Duration;

    public override SimpleWaveFormat WaveFormat => Original.WaveFormat;

    public override ISampleProvider ToSampleProvider() => Original.ToSampleProvider();

}
