using System.IO;
using NAudio.Wave;

namespace Katie.NAudio.Phrases;

public sealed class WaveStreamPhrase : SamplePhraseBase, IDisposable
{

    private readonly Stream _source;
    private readonly WaveStream _stream;

    public WaveStreamPhrase(Stream source, string name)
    {
        _source = source;
        _stream = new WaveFileReader(source);
        Text = name;
        Duration = _stream.TotalTime;
    }

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public override SimpleWaveFormat WaveFormat => _stream.WaveFormat;

    public override ISampleProvider ToSampleProvider()
    {
        _stream.Position = 0;
        return _stream.ToSampleProvider();
    }

    public void Dispose()
    {
        _stream.Dispose();
        _source.Dispose();
    }

}
