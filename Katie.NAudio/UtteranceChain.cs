using System.Diagnostics.CodeAnalysis;
using Katie.Core;
using Katie.Core.DataStructures;
using Katie.NAudio.Extensions;
using Katie.NAudio.Phrases;
using NAudio.Wave;
using WaveSegment = Katie.Core.UtteranceSegment<Katie.NAudio.Phrases.WavePhraseBase>;

namespace Katie.NAudio;

using PhraseClip = (ISampleProvider Provider, WaveSegment Segment);

public sealed class UtteranceChain : ISampleProvider
{

    public static UtteranceChain? From(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, SimpleWaveFormat? format = null)
    {
        var segments = ParseToQueue(text, tree, ref format);
        return segments.Count == 0 ? null : new UtteranceChain(segments.Dequeue(), segments, format.Value);
    }

    public static Queue<WaveSegment> ParseToQueue(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, [NotNull] ref SimpleWaveFormat? format)
    {
        var segments = new Queue<WaveSegment>();
        var parser = new PhraseParser<WavePhraseBase>(text, tree);
        while (parser.Next(out var phrase))
        {
            if (phrase.Duration == TimeSpan.Zero)
                continue;
            format ??= phrase.Phrase?.WaveFormat;
            segments.Enqueue(phrase);
        }

        format ??= SimpleWaveFormat.Default;
        return segments;
    }

    private bool _ended;

    public Queue<WaveSegment> Remaining { get; }

    public PhraseClip Current { get; private set; }

    public TimeSpan CurrentTime { get; private set; }

    public TimeSpan TotalTime { get; }

    public UtteranceChain(WaveSegment initial, Queue<WaveSegment> remaining, SimpleWaveFormat waveFormat)
    {
        Remaining = remaining;
        WaveFormat = waveFormat.ToIeeeFloat();
        Current = ToClip(initial);
        TotalTime = remaining.Aggregate(initial.Duration, static (prev, curr) => prev + curr.Duration);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        if (_ended)
            return 0;
        var total = 0;
        while (total < count)
        {
            var read = Current.Provider.Read(buffer, total, Math.Min(count, count - total));
            total += read;
            CurrentTime += this.SamplesToTime(read);
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

    private bool TryDequeue(out PhraseClip result)
    {
        if (!Remaining.TryDequeue(out var segment))
        {
            result = default;
            return false;
        }

        result = ToClip(segment);
        return true;
    }

    private PhraseClip ToClip(WaveSegment segment) => segment is {Phrase: { } clip}
        ? (clip.ToSampleProvider().EnsureFormat(WaveFormat), segment)
        : (new DurationSilenceSampleProvider(WaveFormat, segment.Duration.TotalSeconds), segment);

}
