using Katie.NAudio;
using Katie.NAudio.Extensions;
using NAudio.Wave;

namespace Katie.UI.Audio;

public sealed class RestartingSampleProvider : ISampleProvider
{

    private readonly ISampleProvider _provider;

    private readonly WaveStream _stream;

    private bool _read;

    public RestartingSampleProvider(WaveStream stream, SimpleWaveFormat format)
    {
        WaveFormat = format.ToIeeeFloat();
        _stream = stream;
        _provider = stream.ToSampleProvider().EnsureFormat(format);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        if (!_read)
            _stream.Position = 0;
        _read = true;
        return _provider.Read(buffer, offset, count);
    }

}
