using System;
using System.Collections.Generic;
using Katie.Core;
using Katie.Core.DataStructures;
using NAudio.Wave;

namespace Katie.NAudio;

using LabeledClip = (ISampleProvider Provider, string Text);

public sealed class PhraseChain : ISampleProvider
{

    public static WaveFormat Format { get; } = WaveFormat.CreateIeeeFloatWaveFormat(48000, 1);

    public static PhraseChain? Parse(ReadOnlySpan<char> text, PhraseTree<WavePhrase> tree)
    {
        var segments = new Queue<UtteranceSegment<WavePhrase>>();
        var parser = new PhraseParser<WavePhrase>(text, tree);
        while (parser.Next(out var phrase))
            if (phrase.Duration != TimeSpan.Zero)
                segments.Enqueue(phrase);
        return segments.Count == 0 ? null : new PhraseChain(segments);
    }

    private readonly Queue<UtteranceSegment<WavePhrase>> _remaining;

    private bool _ended;

    public LabeledClip Current { get; private set; }

    private PhraseChain(Queue<UtteranceSegment<WavePhrase>> remaining)
    {
        _remaining = remaining;
        TryDequeue(out var current);
        Current = current;
    }

    public WaveFormat WaveFormat => Format;

    public int Read(float[] buffer, int offset, int count)
    {
        if (_ended)
            return 0;
        var total = 0;
        while (total < count)
        {
            var read = Current.Provider.Read(buffer, total, Math.Min(count, count - total));
            total += read;
            if (read > 0)
                continue;
            if (TryDequeue(out var result))
            {
                Current = result;
                continue;
            }

            _ended = true;
            break;
        }

        return total;
    }

    private bool TryDequeue(out LabeledClip result)
    {
        if (!_remaining.TryDequeue(out var segment))
        {
            result = default;
            return false;
        }

        result = segment is {Phrase: { } clip}
            ? (clip.ToSampleProvider(), clip.Text)
            : (new DurationSilenceSampleProvider(Format, segment.Duration.TotalSeconds), "");
        return true;
    }

}
