using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing;

public ref struct HungarianNumericParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    private HungarianNumberParser<T> _numberParser;

    private NumericTokenPart _previousPart;

    public bool IsActive => _previousPart != NumericTokenPart.None;

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
                phrase = Phrase("óra");
                return true;
            case NumericTokenPart.Hour:
                _previousPart = NumericTokenPart.MinuteNumber;
                BeginNumber(ref index, 2, out phrase);
                return true;
            case NumericTokenPart.MinuteNumber:
                _previousPart = NumericTokenPart.None;
                index++;
                phrase = Phrase("perc");
                return true;
            default:
                phrase = default;
                return false;
        }
    }

    public bool Begin(ref int index, int tokenEnd, out UtteranceSegment<T> phrase)
    {
        _previousPart = NumericTokenPart.None;
        var length = tokenEnd - index;
        if (length == 5 && char.IsDigit(_text[index + 1]) && _text[index + 2] == ':')
        {
            _previousPart = NumericTokenPart.HourNumber;
            BeginNumber(ref index, 2, out phrase);
            return true;
        }

        index = tokenEnd;
        phrase = default;
        return false;
    }

    private UtteranceSegment<T> Phrase(ReadOnlySpan<char> token)
        => _tree.TryGetRootValue(token, out var value) ? value : token.Length;

    private void BeginNumber(ref int index, int length, out UtteranceSegment<T> phrase)
    {
        _numberParser = new HungarianNumberParser<T>(_text[index..(index + length)], _tree);
        _numberParser.Next(out phrase, out var advanced);
        index += advanced;
    }

    private enum NumericTokenPart
    {

        None,
        HourNumber,
        Hour,
        MinuteNumber

    }

}
