using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing.English;

public ref struct EnglishNumericParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private EnglishNumberParser<T> _numberParser;

    private NumericTokenPart _part;
    private NumericTokenShape _shape;

    public bool IsActive => _part != NumericTokenPart.None || _numberParser.IsActive;

    public EnglishNumericParser(ReadOnlySpan<char> text, PhraseTree<T> tree)
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
                phrase = _tree.RootPhrase("o'clock");
                index += 3;
                return true;
            case NumericTokenPart.HourNumber:
                _part = NumericTokenPart.Minute;
                phrase = 0.2;
                index++;
                return true;
            case NumericTokenPart.Minute:
                _part = NumericTokenPart.None;
                phrase = BeginNumber(ref index, 2, false);
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
        _part = NumericTokenPart.None;
        _shape = NumericShapeDetector.Identify(_text[index..], length);
        (_part, phrase) = _shape switch
        {
            NumericTokenShape.Regular => (NumericTokenPart.None, BeginNumber(
                ref index,
                length,
                true,
                length > 3 ? NumberInterpretation.SeparateDigits : NumberInterpretation.Regular
            )),
            NumericTokenShape.Ordinal => (NumericTokenPart.None, BeginNumber(ref index, length, true, NumberInterpretation.Ordinal)),
            NumericTokenShape.TimeHourMinute or NumericTokenShape.TimeHourOnly => (NumericTokenPart.HourNumber, BeginNumber(ref index, 2, false)),
            _ => (NumericTokenPart.None, default)
        };
        return _shape is not (NumericTokenShape.None or NumericTokenShape.RegularSuffixed);
    }

    private UtteranceSegment<T> BeginNumber(ref int index, int length, bool trim, NumberInterpretation interpretation = NumberInterpretation.Regular)
    {
        var span = _text[index..(index + length)];
        var advanced = 0;
        if (trim)
        {
            _numberParser = EnglishNumberParser<T>.CreateTrimmed(span, _tree, interpretation, out advanced);
            index += advanced;
        }
        else
            _numberParser = new EnglishNumberParser<T>(span, _tree, interpretation);

        _numberParser.Next(out var phrase, out advanced);
        index += advanced;
        if (interpretation == NumberInterpretation.Ordinal)
            index++;
        return phrase;
    }

    private enum NumericTokenPart
    {

        None,
        HourNumber,
        Minute

    }

}
