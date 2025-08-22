namespace Katie.Core;

public readonly record struct UtteranceSegment<T>(TimeSpan Duration, int EndIndex, T? Phrase = null) where T : PhraseBase
{

    public UtteranceSegment<T> WithOffset(int amount) => this with {EndIndex = EndIndex + amount};

    public static implicit operator UtteranceSegment<T>((int Characters, int End) tuple) => new(TimeSpan.FromMilliseconds(tuple.Characters * 100), tuple.End);

    public static implicit operator UtteranceSegment<T>((double Seconds, int End) tuple) => new(TimeSpan.FromSeconds(tuple.Seconds), tuple.End);

    public static implicit operator UtteranceSegment<T>((T? Phrase, int End) tuple)
        => tuple.Phrase == null
            ? new UtteranceSegment<T>(TimeSpan.Zero, tuple.End)
            : new UtteranceSegment<T>(tuple.Phrase.Duration, tuple.End, tuple.Phrase);

}
