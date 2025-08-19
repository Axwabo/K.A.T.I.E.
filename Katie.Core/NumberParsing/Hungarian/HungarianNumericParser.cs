using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing.Hungarian;

public ref struct HungarianNumericParser<T> where T : PhraseBase
{

    private static readonly UtteranceSegment<T> Pause = 0.2;

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private HungarianNumberParser<T> _numberParser;

    private NumericTokenPart _part;
    private NumericTokenShape _shape;
    private int _end;
    private int _suffixStart;

    public bool IsActive => _part != NumericTokenPart.None || _numberParser.IsActive;

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
            case NumericTokenPart.HourNumber when _shape == NumericTokenShape.TimeHourOnly:
                _part = NumericTokenPart.None;
                index += 3;
                phrase = EndWithSuffix(ref index, "óra");
                return true;
            case NumericTokenPart.HourNumber:
                _part = NumericTokenPart.Hour;
                phrase = _tree.RootPhrase("óra");
                return true;
            case NumericTokenPart.Hour:
                _part = NumericTokenPart.Minute;
                phrase = Pause;
                index++;
                return true;
            case NumericTokenPart.Minute:
                _part = NumericTokenPart.MinuteNumber;
                phrase = BeginNumber(ref index, 2);
                return true;
            case NumericTokenPart.MinuteNumber:
                _part = NumericTokenPart.None;
                phrase = EndWithSuffix(ref index, "perc");
                return true;
            case NumericTokenPart.BeforeRegularSuffix:
                _part = NumericTokenPart.None;
                phrase = EndWithSuffix(ref index, ReadOnlySpan<char>.Empty);
                return true;
            default:
                phrase = default;
                return false;
        }
    }

    private UtteranceSegment<T> EndWithSuffix(ref int index, ReadOnlySpan<char> main)
    {
        var suffix = _text[index.._end];
        index = _end;
        return _tree.RootPhrase(
            suffix.IsEmpty
                ? main
                : main.IsEmpty
                    ? suffix
                    : new TreeKey
                    {
                        First = main,
                        Second = suffix.TrimStart('-')
                    }
        );
    }

    public bool Begin(ref int index, int tokenEnd, out UtteranceSegment<T> phrase)
    {
        if (!char.IsDigit(_text[index]))
        {
            phrase = default;
            return false;
        }

        var length = tokenEnd - index;
        _part = NumericTokenPart.None;
        _shape = NumericShapeDetector.Identify(_text[index..], length);
        _end = tokenEnd;
        (_part, phrase) = _shape switch
        {
            NumericTokenShape.Regular => (NumericTokenPart.None, BeginNumber(ref index, length)),
            NumericTokenShape.RegularSuffixed => (NumericTokenPart.BeforeRegularSuffix, BeginSuffixed(ref index)),
            NumericTokenShape.Ordinal => (NumericTokenPart.None, BeginNumber(ref index, length, true)),
            NumericTokenShape.TimeHourMinute or NumericTokenShape.TimeHourOnly => (NumericTokenPart.HourNumber, BeginNumber(ref index, 2)),
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

    private UtteranceSegment<T> BeginSuffixed(ref int index)
    {
        _suffixStart = index + _text[index..].IndexOf('-');
        return BeginNumber(ref index, _suffixStart - index);
    }

    private enum NumericTokenPart
    {

        None,
        HourNumber,
        Hour,
        Minute,
        MinuteNumber,
        BeforeRegularSuffix

    }

}
