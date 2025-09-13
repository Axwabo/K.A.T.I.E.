using NAudio.Wave;

namespace Katie.UI.Audio;

public sealed class RawSourceSampleProvider : ISampleProvider
{

    private readonly float[] _source;

    public int Length { get; }

    public int Position { get; set; }

    public RawSourceSampleProvider(WaveFormat waveFormat, float[] source, int length)
    {
        _source = source;
        WaveFormat = waveFormat;
        Length = length;
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        var remaining = Math.Max(0, Math.Min(count, Length - Position));
        if (remaining == 0)
            return 0;
        Array.Copy(_source, Position, buffer, offset, remaining);
        Position += remaining;
        return remaining;
    }

    public RawSourceSampleProvider Copy() => new(WaveFormat, _source, Length);

    public ArraySegment<float> ToArraySegment() => new(_source, 0, Length);

}
