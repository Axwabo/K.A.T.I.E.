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

        switch (_part)
        {
            case NumericTokenPart.HourNumber:
                index++;
                phrase = _tree.TryGetRootValue("óra", out var hour) ? hour : 3;
                _part = NumericTokenPart.Hour;
                return true;
            case NumericTokenPart.Hour:
                _numberParser = new HungarianNumberParser<T>(_text[index..(index + 2)], _tree);
                _numberParser.Next(out phrase, out var advancedHour);
                _part = NumericTokenPart.MinuteNumber;
                index += advancedHour;
                return true;
            case NumericTokenPart.MinuteNumber:
                index++;
                phrase = _tree.TryGetRootValue("perc", out var minute) ? minute : 4;
                _part = NumericTokenPart.None;
                return true;
            default:
                phrase = default;
                return false;
        }
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
        MinuteNumber

    }

}
