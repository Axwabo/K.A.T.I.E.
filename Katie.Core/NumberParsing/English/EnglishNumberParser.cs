using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing.English;

public ref struct EnglishNumberParser<T> where T : PhraseBase
{

    private readonly PhraseTree<T> _tree;
    private readonly bool _leadingZero;

    private SequentialNumberParser<T> _parser;
    private int _index;

    public bool IsActive => _parser.IsActive;

    public EnglishNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, bool ordinal)
    {
        _tree = tree;
        _parser = new SequentialNumberParser<T>(text, tree, Map.Digits, ordinal);
        _leadingZero = text[0] == '0';
    }

    private EnglishNumberParser(PhraseTree<T> tree, SequentialNumberParser<T> parser)
    {
        _tree = tree;
        _parser = parser;
        _leadingZero = false;
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced)
    {
        switch (_parser.PositionalIndex, _parser.Digit)
        {
            case (not 0, '0') when _leadingZero:
                phrase = _tree.RootPhrase("oh", _index);
                return Advance(out _, out advanced);
            case (1, '1'):
                Advance(out phrase, out advanced);
                if (advanced == 2)
                    return true;
                advanced++;
                _index++;
                phrase = _tree.RootPhrase(Map.TenTy(_parser.Digit), _index);
                Advance(out _, out _);
                return true;
            default:
                return Advance(out phrase, out advanced);
        }
    }

    private bool Advance(out UtteranceSegment<T> phrase, out int advanced)
    {
        var success = _parser.Next(out phrase, out advanced);
        _index += advanced;
        return success;
    }

    public static EnglishNumberParser<T> CreateTrimmed(ReadOnlySpan<char> text, PhraseTree<T> tree, bool ordinal, out int advanced)
        => new(tree, SequentialNumberParser<T>.CreateTrimmed(text, tree, Map.Digits, ordinal, out advanced));

}

file static class Map
{

    public static DigitMappers Digits { get; } = new(Ten, Ten, Ten, OneExact, OneOrdinal);

    public static string TenTy(char one) => one switch
    {
        '0' => "ten",
        '1' => "eleven",
        '2' => "twelve",
        '3' => "thirteen",
        '4' => "fourteen",
        '5' => "fifteen",
        '6' => "sixteen",
        '7' => "seventeen",
        '8' => "eighteen",
        '9' => "nineteen",
        _ => ""
    };

    private static string Ten(char digit) => digit switch
    {
        '1' => "ten",
        '2' => "twenty",
        '3' => "thirty",
        '4' => "forty",
        '5' => "fifty",
        '6' => "sixty",
        '7' => "seventy",
        '8' => "eighty",
        '9' => "ninety",
        _ => ""
    };

    private static string OneExact(char digit) => digit switch
    {
        '0' => "zero",
        '1' => "one",
        '2' => "two",
        '3' => "three",
        '4' => "four",
        '5' => "five",
        '6' => "six",
        '7' => "seven",
        '8' => "eight",
        '9' => "nine",
        _ => ""
    };

    private static string OneOrdinal(char digit) => digit switch
    {
        '0' => "zeroth",
        '1' => "first",
        '2' => "second",
        '3' => "third",
        '4' => "fourth",
        '5' => "fifth",
        '6' => "sixth",
        '7' => "seventh",
        '8' => "eighth",
        '9' => "ninth",
        _ => ""
    };

}
