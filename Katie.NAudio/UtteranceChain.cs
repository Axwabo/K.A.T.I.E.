using Katie.Core;
using Katie.Core.DataStructures;
using Katie.NAudio.Extensions;
using Katie.NAudio.Phrases;
using NAudio.Wave;

namespace Katie.NAudio;

using LabeledClip = (ISampleProvider Provider, string Text);

public sealed class UtteranceChain : ISampleProvider
{

    public static UtteranceChain? Parse(ReadOnlySpan<char> text, PhraseTree<SamplePhraseBase> tree, ReadOnlySpan<char> language)
    {
        var segments = new Queue<UtteranceSegment<SamplePhraseBase>>();
        var parser = new PhraseParser<SamplePhraseBase>(text, tree, language);
        SimpleWaveFormat? format = null;
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

    private readonly Queue<UtteranceSegment<SamplePhraseBase>> _remaining;

    private bool _ended;

    public LabeledClip Current { get; private set; }

    public TimeSpan CurrentTime { get; private set; }

    public TimeSpan TotalTime { get; }

    public IEnumerable<UtteranceSegment<PhraseBase>> Remaining
        => _remaining.Select(e => new UtteranceSegment<PhraseBase>(e.Duration, e.EndIndex, e.Phrase));

    private UtteranceChain(Queue<UtteranceSegment<SamplePhraseBase>> remaining, SimpleWaveFormat waveFormat)
    {
        _remaining = remaining;
        WaveFormat = waveFormat.ToIeeeFloat();
        TotalTime = remaining.Aggregate(TimeSpan.Zero, (prev, curr) => prev + curr.Duration);
        TryDequeue(out var current);
        Current = current;
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

    private bool TryDequeue(out LabeledClip result)
    {
        if (!_remaining.TryDequeue(out var segment))
        {
            result = default;
            return false;
        }

        result = segment is {Phrase: { } clip}
            ? (clip.ToSampleProvider().EnsureFormat(WaveFormat), clip.Text)
            : (new DurationSilenceSampleProvider(WaveFormat, segment.Duration.TotalSeconds), "");
        return true;
    }

}
