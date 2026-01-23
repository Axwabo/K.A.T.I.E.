using NAudio.Wave;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Metadata.Models;

namespace Katie.UI.Desktop;

public sealed class NAudioDataProvider : ISoundDataProvider
{

    private readonly ISampleProvider _source;
    private readonly float[] _buffer;

    public NAudioDataProvider(ISampleProvider source)
    {
        _source = source;
        _buffer = new float[480];
        SampleFormat = SampleFormat.F32;
        SampleRate = source.WaveFormat.SampleRate;
    }

    public int Position { get; private set; }
    public int Length => int.MaxValue;
    public bool CanSeek => false;
    public SampleFormat SampleFormat { get; }
    public int SampleRate { get; }

    public bool IsDisposed => false;

    public SoundFormatInfo? FormatInfo => null;

    public event EventHandler<EventArgs>? EndOfStreamReached;

    public event EventHandler<PositionChangedEventArgs>? PositionChanged;

    public int ReadBytes(Span<float> buffer)
    {
        var total = 0;
        int read;
        while ((read = _source.Read(_buffer, 0, Math.Min(_buffer.Length, buffer.Length - total))) != 0)
        {
            _buffer.AsSpan()[..read].CopyTo(buffer[total..]);
            total += read;
            Position += read;
            PositionChanged?.Invoke(this, new PositionChangedEventArgs(Position));
        }

        if (total == 0)
            EndOfStreamReached?.Invoke(this, EventArgs.Empty);
        return total;
    }

    public void Seek(int offset) => throw new NotSupportedException();

    public void Dispose()
    {
    }

}
