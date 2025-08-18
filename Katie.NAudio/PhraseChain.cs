using System;
using System.Collections.Generic;
using Katie.Core;
using Katie.Core.DataStructures;
using NAudio.Wave;

namespace Katie.NAudio;

using Clip = (ISampleProvider Provider, string Text);

public sealed class PhraseChain : ISampleProvider
{

    public static WaveFormat Format { get; } = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1);

    public static PhraseChain Parse(ReadOnlySpan<char> text, PhraseTree<WavePhrase> tree)
    {
        var providers = new Queue<Clip>();
        var parser = new PhraseParser<WavePhrase>(text, tree);
        while (parser.Next(out var phrase))
            switch (phrase)
            {
                case {Phrase: { } clip}:
                    providers.Enqueue((clip.ToSampleProvider(), clip.Text));
                    break;
                case {Duration.TotalSeconds: not 0 and var seconds}:
                    providers.Enqueue((new DurationSilenceSampleProvider(Format, seconds), ""));
                    break;
            }

        return new PhraseChain(providers);
    }

    private readonly Queue<Clip> _remaining;

    private bool _ended;

    public Clip Current { get; private set; }

    public PhraseChain(Queue<Clip> remaining)
    {
        _remaining = remaining;
        _ended = _remaining.Count == 0;
        Current = _ended ? (new DurationSilenceSampleProvider(Format, 0), "") : remaining.Dequeue();
    }

    public WaveFormat WaveFormat => Format;

    public int Read(float[] buffer, int offset, int count)
    {
        if (_ended)
            return 0;
        var total = 0;
        while (total < count)
        {
            var read = Current.Provider.Read(buffer, total, Math.Min(count, total - count));
            total += read;
            if (read > 0)
                continue;
            if (_remaining.TryDequeue(out var result))
            {
                Current = result;
                continue;
            }

            _ended = true;
            break;
        }

        return total;
    }

}
