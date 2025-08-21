using NAudio.Wave;

namespace Katie.NAudio;

public readonly record struct SimpleWaveFormat(int SampleRate, int Channels)
{

    public static SimpleWaveFormat Default { get; } = new(48000, 1);

    public WaveFormat ToIeeeFloat() => WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

    public static implicit operator SimpleWaveFormat(WaveFormat format) => new(format.SampleRate, format.Channels);

    public static bool operator ==(SimpleWaveFormat simple, WaveFormat format)
        => simple.SampleRate == format.SampleRate && simple.Channels == format.Channels;

    public static bool operator !=(SimpleWaveFormat simple, WaveFormat format)
        => !(simple == format);

}
