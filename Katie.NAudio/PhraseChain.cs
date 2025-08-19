using System;
using System.Collections.Generic;
using System.Linq;
using Katie.Core;
using Katie.Core.DataStructures;
using Katie.NAudio.Phrases;
using NAudio.Wave;

namespace Katie.NAudio;

using LabeledClip = (ISampleProvider Provider, string Text);

public sealed class PhraseChain : ISampleProvider
{

    public const int SampleRate = 48000;
    public const double SamplesToSeconds = 1d / SampleRate;

    public static WaveFormat Format { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1);

    public static PhraseChain? Parse(ReadOnlySpan<char> text, PhraseTree<SamplePhraseBase> tree, ReadOnlySpan<char> language)
    {
        var segments = new Queue<UtteranceSegment<SamplePhraseBase>>();
        var parser = new PhraseParser<SamplePhraseBase>(text, tree, language);
        while (parser.Next(out var phrase))
            if (phrase.Duration != TimeSpan.Zero)
                segments.Enqueue(phrase);
        return segments.Count == 0 ? null : new PhraseChain(segments);
    }

    private readonly Queue<UtteranceSegment<SamplePhraseBase>> _remaining;

    private bool _ended;

    public LabeledClip Current { get; private set; }

    public TimeSpan CurrentTime { get; private set; }

    public TimeSpan TotalTime { get; }

    private PhraseChain(Queue<UtteranceSegment<SamplePhraseBase>> remaining)
    {
        _remaining = remaining;
        TotalTime = remaining.Aggregate(TimeSpan.Zero, (prev, curr) => prev + curr.Duration);
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
            CurrentTime += TimeSpan.FromSeconds(read * SamplesToSeconds);
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
