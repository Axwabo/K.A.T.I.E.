using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing.Hungarian;

public ref struct HungarianNumberParser<T> where T : PhraseBase
{

    private SequentialNumberParser<T> _parser;

    public bool IsActive => _parser.IsActive;

    public HungarianNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, bool ordinal, out UtteranceSegment<T> phrase, out int advanced)
    {
        var trimmed = text.TrimStart('0');
        _parser = new SequentialNumberParser<T>(trimmed, tree, Map.Digits, ordinal, out phrase, out advanced);
        advanced += text.Length - trimmed.Length;
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced) => _parser.Next(out phrase, out advanced);

}

file static class Map
{

    public static DigitMappers Digits { get; } = new(Ten, TenExact, TenOrdinal, One, Ordinal);

    private static string Ten(char digit) => digit switch
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

    private static string TenExact(char digit) => digit switch
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

    private static string TenOrdinal(char digit) => digit switch
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

    private static string One(char digit) => digit switch
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

    private static string Ordinal(char digit) => digit switch
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
