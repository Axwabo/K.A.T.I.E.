using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing.English;

public ref struct EnglishNumericParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private EnglishNumberParser<T> _numberParser;

    private bool _wasHour;
    private NumericTokenShape _shape;

    public bool IsActive => _wasHour || _numberParser.IsActive;

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

        if (_wasHour)
        {
            _wasHour = false;
            phrase = BeginNumber(ref index, 2);
            index++;
            return true;
        }

        phrase = default;
        return false;
    }

    public bool Begin(ref int index, int tokenEnd, out UtteranceSegment<T> phrase)
    {
        if (!char.IsDigit(_text[index]))
        {
            phrase = default;
            return false;
        }

        var length = tokenEnd - index;
        _wasHour = false;
        _shape = NumericShapeDetector.Identify(_text[index..], length);
        if (_shape == NumericTokenShape.None)
        {
            phrase = default;
            return false;
        }

        (_wasHour, phrase) = _shape switch
        {
            NumericTokenShape.Regular => (false, BeginNumber(ref index, length)),
            NumericTokenShape.Ordinal => (false, BeginNumber(ref index, length, true)),
            NumericTokenShape.Time => (true, BeginNumber(ref index, 2)),
            _ => (false, default)
        };
        return true;
    }

    private UtteranceSegment<T> BeginNumber(ref int index, int length, bool ordinal = false)
    {
        _numberParser = new EnglishNumberParser<T>(_text[index..(index + length)], _tree, ordinal, out var phrase, out var advanced);
        index += advanced;
        if (ordinal)
            index++;
        return phrase;
    }

}
