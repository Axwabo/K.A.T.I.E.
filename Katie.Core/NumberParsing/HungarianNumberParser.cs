using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing;

public ref struct HungarianNumberParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private int _index;
    private NumberPart _part;

    public bool IsActive => _part != NumberPart.None;

    public HungarianNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _text = text;
        _tree = tree;
        _part = text.Length switch
        {
            1 => NumberPart.Egyes,
            2 => NumberPart.Tízes,
            _ => throw new ArgumentException($"Cannot parse number of {text.Length} digits")
        };
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced)
    {
        switch (_part)
        {
            case NumberPart.Tízes:
                if (_text[^1] == 0)
                {
                    phrase = _tree.Digit(_text[_index], Map.Tíz);
                    advanced = 2;
                    _part = NumberPart.None;
                    return true;
                }

                phrase = _tree.Digit(_text[_index++], Map.Tizen);
                advanced = 1;
                _part = NumberPart.Egyes;
                return true;
            case NumberPart.Egyes:
                phrase = _tree.Digit(_text[_index], Map.Egy);
                advanced = 1;
                _part = NumberPart.None;
                return true;
            default:
                phrase = default;
                advanced = 0;
                return false;
        }
    }

    private enum NumberPart
    {

        None,
        Tízes,
        Egyes

    }

}

file static class Map
{

    public static string Tizen(char digit) => digit switch
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

    public static string Tíz(char digit) => digit switch
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

    public static string Egy(char digit) => digit switch
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

    public static UtteranceSegment<T> Digit<T>(this PhraseTree<T> tree, char digit, Func<char, string> mapper) where T : PhraseBase
    {
        var key = mapper(digit);
        return string.IsNullOrEmpty(key) || !tree.TryGetRootValue(key, out var phrase)
            ? 1
            : phrase;
    }

}
