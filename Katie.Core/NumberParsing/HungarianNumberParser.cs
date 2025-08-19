using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing;

public ref struct HungarianNumberParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;
    private readonly bool _ordinal;

    private int _positionalIndex;

    private char Digit => _text[_text.Length - _positionalIndex - 1];

    public bool IsActive => _positionalIndex != -1 && !_text.IsEmpty;

    public HungarianNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, bool ordinal, out UtteranceSegment<T> phrase, out int advanced)
    {
        if (text.IsEmpty)
            throw new ArgumentException("Text cannot be empty", nameof(text));
        _text = text.TrimStart('0');
        _tree = tree;
        _ordinal = ordinal;
        if (_text.Length > 2)
            throw new ArgumentException($"Cannot parse a number of {_text.Length} digits", nameof(text));
        _positionalIndex = _text.Length - 1;
        Next(out phrase, out advanced);
        advanced += text.Length - _text.Length;
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced)
    {
        switch (_positionalIndex)
        {
            case 1:
                if (_text[^1] == '0')
                {
                    phrase = _tree.Digit(_text[^2], _ordinal ? Map.TenOrdinal : Map.TenExact);
                    advanced = 2;
                    _positionalIndex = -1;
                    return true;
                }

                phrase = _tree.Digit(Digit, Map.Ten);
                advanced = 1;
                _positionalIndex = 0;
                return true;
            case 0:
                phrase = _tree.Digit(Digit, _ordinal ? Map.Ordinal : Map.One);
                advanced = 1;
                _positionalIndex = -1;
                return true;
            default:
                phrase = default;
                advanced = 0;
                return false;
        }
    }

}

file static class Map
{

    public static string Ten(char digit) => digit switch
    {
        '1' => "tizen",
        '2' => "huszon",
        '3' => "harminc",
        '4' => "negyven",
        '5' => "ötven",
        '6' => "hatvan",
        '7' => "hetven",
        '8' => "nyolcvan",
        '9' => "kilencven",
        _ => ""
    };

    public static string TenExact(char digit) => digit switch
    {
        '1' => "tíz",
        '2' => "húsz",
        '3' => "harminc",
        '4' => "negyven",
        '5' => "ötven",
        '6' => "hatvan",
        '7' => "hetven",
        '8' => "nyolcvan",
        '9' => "kilencven",
        _ => ""
    };

    public static string TenOrdinal(char digit) => digit switch
    {
        '1' => "tizedik",
        '2' => "huszadik",
        '3' => "harmincadik",
        '4' => "negyvenedik",
        '5' => "ötvenedik",
        '6' => "hatvanadik",
        '7' => "hetvenedik",
        '8' => "nyolcvanadik",
        '9' => "kilencedik",
        _ => ""
    };

    public static string One(char digit) => digit switch
    {
        '0' => "nulla",
        '1' => "egy",
        '2' => "két",
        '3' => "három",
        '4' => "négy",
        '5' => "öt",
        '6' => "hat",
        '7' => "hét",
        '8' => "nyolc",
        '9' => "kilenc",
        _ => ""
    };

    public static string Ordinal(char digit) => digit switch
    {
        '0' => "nulladik",
        '1' => "első",
        '2' => "második",
        '3' => "harmadik",
        '4' => "negyedik",
        '5' => "ötödik",
        '6' => "hatodik",
        '7' => "hetedik",
        '8' => "nyolcadik",
        '9' => "kilencedik",
        _ => ""
    };

}
