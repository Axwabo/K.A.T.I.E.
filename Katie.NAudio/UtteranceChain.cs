using Katie.Core;
using Katie.Core.DataStructures;
using Katie.NAudio.Extensions;
using Katie.NAudio.Phrases;
using NAudio.Wave;

namespace Katie.NAudio;

using PhraseClip = (ISampleProvider Provider, UtteranceSegment<WavePhraseBase> Segment);

public sealed class UtteranceChain : ISampleProvider
{

    public static UtteranceChain? From(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, SimpleWaveFormat? format = null)
    {
        var segments = new Queue<UtteranceSegment<WavePhraseBase>>();
        var parser = new PhraseParser<WavePhraseBase>(text, tree);
        while (parser.Next(out var phrase))
        {
            if (phrase.Duration == TimeSpan.Zero)
                continue;
            format ??= phrase.Phrase?.WaveFormat;
            segments.Enqueue(phrase);
        }

        format ??= SimpleWaveFormat.Default;
        return segments.Count == 0 ? null : new UtteranceChain(segments, format.Value);
    }

    private bool _ended;

    private bool _started;

    public Queue<UtteranceSegment<WavePhraseBase>> Remaining { get; }

    public PhraseClip Current { get; private set; }

    public TimeSpan CurrentTime { get; private set; }

    public TimeSpan TotalTime { get; }

    // TODO: require initial & ensure format
    public UtteranceChain(Queue<UtteranceSegment<WavePhraseBase>> remaining, SimpleWaveFormat waveFormat)
    {
        Remaining = remaining;
        WaveFormat = waveFormat.ToIeeeFloat();
        TotalTime = remaining.Aggregate(TimeSpan.Zero, static (prev, curr) => prev + curr.Duration);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        if (_ended)
            return 0;
        var total = 0;
        while (total < count)
        {
            var read = _started ? Current.Provider.Read(buffer, total, Math.Min(count, count - total)) : 0;
            total += read;
            CurrentTime += this.SamplesToTime(read);
            if (read > 0)
                continue;
            if (TryDequeue(out var result))
            {
                Current = result;
                _started = true;
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

    private PhraseClip ToClip(UtteranceSegment<WavePhraseBase> segment) => segment is {Phrase: { } clip}
        ? (clip.ToSampleProvider().EnsureFormat(WaveFormat), segment)
        : (new DurationSilenceSampleProvider(WaveFormat, segment.Duration.TotalSeconds), segment);

}
