using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing;

public ref struct HungarianNumericParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private HungarianNumberParser<T> _numberParser;

    private NumericTokenPart _part;

    public bool IsActive => _part != NumericTokenPart.None;

    public HungarianNumericParser(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _text = text;
        _tree = tree;
    }

    public bool Next(ref int index, out UtteranceSegment<T> phrase)
    {
        if (_numberParser.IsActive && _numberParser.Next(out phrase, out var advanced))
        {
            index += advanced;
            return true;
        }

        phrase = default;
        return false;
    }

    public bool Begin(ref int index, int tokenEnd, out UtteranceSegment<T> phrase)
    {
        _part = NumericTokenPart.None;
        var length = tokenEnd - index;
        if (length == 5 && char.IsDigit(_text[index + 1]) && _text[index + 2] == ':')
        {
            _part = NumericTokenPart.HourNumber;
            _numberParser = new HungarianNumberParser<T>(_text[index..(index + 2)], _tree);
            _numberParser.Next(out phrase, out var advanced);
            index += advanced;
            return true;
        }

        index = tokenEnd;
        phrase = default;
        return false;
    }

    private enum NumericTokenPart
    {

        None,
        HourNumber,
        Hour,
        MinuteNumber,
        Minute

    }

}
