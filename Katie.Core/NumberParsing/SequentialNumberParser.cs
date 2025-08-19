using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core.NumberParsing;

public ref struct SequentialNumberParser<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;
    private readonly DigitMappers _mappers;
    private readonly bool _isOrdinal;

    private int _positionalIndex;

    private char Digit => _text[_text.Length - _positionalIndex - 1];

    public bool IsActive => _positionalIndex != -1 && !_text.IsEmpty;

    public SequentialNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, DigitMappers mappers, bool isOrdinal, out UtteranceSegment<T> phrase, out int advanced)
    {
        if (text.IsEmpty)
            throw new ArgumentException("Text cannot be empty", nameof(text));
        _text = text.TrimStart('0');
        _tree = tree;
        _mappers = mappers;
        _isOrdinal = isOrdinal;
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
                    phrase = _tree.Digit(_text[^2], _isOrdinal ? _mappers.TenOrdinal : _mappers.TenExact);
                    advanced = 2;
                    _positionalIndex = -1;
                    return true;
                }

                phrase = _tree.Digit(Digit, _mappers.Ten);
                advanced = 1;
                _positionalIndex = 0;
                return true;
            case 0:
                phrase = _tree.Digit(Digit, _isOrdinal ? _mappers.OneOrdinal : _mappers.OneExact);
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
