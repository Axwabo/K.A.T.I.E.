using System;

namespace Katie.Core;

public readonly record struct UtteranceSegment<T>(TimeSpan Duration, T? Phrase = null) where T : PhraseBase
{

    public static implicit operator UtteranceSegment<T>(TimeSpan duration) => new(duration);

    public static implicit operator UtteranceSegment<T>(T? phrase)
        => phrase == null
            ? default
            : new UtteranceSegment<T>(phrase.Duration, phrase);

}
