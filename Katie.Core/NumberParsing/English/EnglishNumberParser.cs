using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing.English;

public ref struct EnglishNumberParser<T> where T : PhraseBase
{

    private SequentialNumberParser<T> _parser;

    public bool IsActive => _parser.IsActive;

    public EnglishNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, bool ordinal, out UtteranceSegment<T> phrase, out int advanced)
        => _parser = new SequentialNumberParser<T>(text, tree, Map.Digits, ordinal, out phrase, out advanced);

    public bool Next(out UtteranceSegment<T> phrase, out int advanced) => _parser.Next(out phrase, out advanced);

}

file static class Map
{

    public static DigitMappers Digits { get; } = new(Ten, TenExact, TenOrdinal, OneExact, OneOrdinal);

    // TODO

    private static string Ten(char digit)
    {
        throw new NotImplementedException();
    }

    private static string TenExact(char arg)
    {
        throw new NotImplementedException();
    }

    private static string TenOrdinal(char arg)
    {
        throw new NotImplementedException();
    }

    private static string OneExact(char arg)
    {
        throw new NotImplementedException();
    }

    private static string OneOrdinal(char arg)
    {
        throw new NotImplementedException();
    }

}
