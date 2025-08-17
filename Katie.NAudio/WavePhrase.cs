using System;
using System.IO;
using Katie.Core;
using NAudio.Wave;

namespace Katie.NAudio;

public sealed class WavePhrase : IPhrase, IDisposable
{

    private readonly Stream _source;
    private readonly WaveStream _stream;

    public WavePhrase(Stream source, string name)
    {
        _source = source;
        _stream = new WaveFileReader(source);
        Text = name;
        Duration = _stream.TotalTime;
    }

    public string Text { get; }

    public TimeSpan Duration { get; }

    public void Dispose()
    {
        _stream.Dispose();
        _source.Dispose();
    }

}
