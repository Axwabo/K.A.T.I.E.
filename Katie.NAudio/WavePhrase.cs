using System;
using System.IO;
using Katie.Core;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.NAudio;

public sealed class WavePhrase : PhraseBase, IDisposable
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

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public ISampleProvider ToSampleProvider()
    {
        _stream.Position = 0;
        var provider = _stream.ToSampleProvider();
        if (provider.WaveFormat.SampleRate != PhraseChain.Format.SampleRate)
            provider = new WdlResamplingSampleProvider(provider, PhraseChain.Format.SampleRate);
        if (provider.WaveFormat.Channels != 1)
            provider = new StereoToMonoSampleProvider(provider);
        return provider;
    }

    public void Dispose()
    {
        _stream.Dispose();
        _source.Dispose();
    }

}
