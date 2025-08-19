using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing.Hungarian;

public ref struct HungarianNumericParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private HungarianNumberParser<T> _numberParser;

    private NumericTokenPart _previousPart;
    private NumericTokenShape _shape;
    private int _end;

    public bool IsActive => _previousPart != NumericTokenPart.None || _numberParser.IsActive;

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

        switch (_previousPart)
        {
            case NumericTokenPart.HourNumber:
                _previousPart = NumericTokenPart.Hour;
                index++;
                phrase = _tree.RootPhrase("óra");
                return true;
            case NumericTokenPart.Hour:
                _previousPart = NumericTokenPart.MinuteNumber;
                phrase = BeginNumber(ref index, 2);
                return true;
            case NumericTokenPart.MinuteNumber:
                _previousPart = NumericTokenPart.None;
                var suffix = _text[index.._end];
                index = _end;
                if (suffix.IsEmpty)
                {
                    phrase = _tree.RootPhrase("perc");
                    return true;
                }

                phrase = _tree.RootPhrase(new TreeKey
                {
                    First = "perc",
                    Second = suffix.TrimStart('-')
                });
                return true;
            default:
                phrase = default;
                return false;
        }
    }

    public bool Begin(ref int index, int tokenEnd, out UtteranceSegment<T> phrase)
    {
        if (!char.IsDigit(_text[index]))
        {
            phrase = default;
            return false;
        }

        var length = tokenEnd - index;
        _previousPart = NumericTokenPart.None;
        _shape = NumericShapeDetector.Identify(_text[index..], length);
        _end = tokenEnd;
        (_previousPart, phrase) = _shape switch
        {
            NumericTokenShape.Regular => (NumericTokenPart.None, BeginNumber(ref index, length)),
            NumericTokenShape.Ordinal => (NumericTokenPart.None, BeginNumber(ref index, length, true)),
            NumericTokenShape.Time => (NumericTokenPart.HourNumber, BeginNumber(ref index, 2)),
            _ => (NumericTokenPart.None, default)
        };
        return _shape != NumericTokenShape.None;
    }

    private UtteranceSegment<T> BeginNumber(ref int index, int length, bool ordinal = false)
    {
        _numberParser = new HungarianNumberParser<T>(_text[index..(index + length)], _tree, ordinal, out var advanced);
        index += advanced;
        _numberParser.Next(out var phrase, out advanced);
        index += advanced;
        if (ordinal)
            index++;
        return phrase;
    }

    private enum NumericTokenPart
    {

        None,
        HourNumber,
        Hour,
        MinuteNumber

    }

}
