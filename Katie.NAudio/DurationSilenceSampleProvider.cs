using System;
using NAudio.Wave;

namespace Katie.NAudio;

public sealed class DurationSilenceSampleProvider : ISampleProvider
{

    private int _remaining;

    public DurationSilenceSampleProvider(WaveFormat waveFormat, double seconds)
    {
        WaveFormat = waveFormat;
        _remaining = (int) (seconds * waveFormat.SampleRate * waveFormat.Channels);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        var read = Math.Min(count, _remaining);
        _remaining = Math.Max(0, _remaining - count);
        Array.Clear(buffer, offset, read);
        return read;
    }

}
