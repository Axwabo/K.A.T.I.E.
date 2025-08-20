namespace Katie.Core;

public readonly record struct UtteranceSegment<T>(TimeSpan Duration, T? Phrase = null) where T : PhraseBase
{

    public static implicit operator UtteranceSegment<T>(int characters) => new(TimeSpan.FromMilliseconds(characters * 100));

    public static implicit operator UtteranceSegment<T>(double seconds) => new(TimeSpan.FromSeconds(seconds));

    public static implicit operator UtteranceSegment<T>(T? phrase)
        => phrase == null
            ? default
            : new UtteranceSegment<T>(phrase.Duration, phrase);

}
