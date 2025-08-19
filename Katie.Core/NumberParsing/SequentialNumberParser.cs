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

    public int PositionalIndex { get; private set; }

    public char Digit => IsActive
        ? _text[_text.Length - PositionalIndex - 1]
        : throw new InvalidOperationException("Cannot access the digit on an inactive parser");

    public bool IsActive => PositionalIndex != -1 && !_text.IsEmpty;

    public SequentialNumberParser(ReadOnlySpan<char> text, PhraseTree<T> tree, DigitMappers mappers, bool isOrdinal, out UtteranceSegment<T> phrase, out int advanced)
    {
        if (text.IsEmpty)
            throw new ArgumentException("Text cannot be empty", nameof(text));
        if (text.Length > 2)
            throw new ArgumentException($"Cannot parse a number of {text.Length} digits", nameof(text));
        _text = text;
        _tree = tree;
        _mappers = mappers;
        _isOrdinal = isOrdinal;
        PositionalIndex = _text.Length - 1;
        Next(out phrase, out advanced);
    }

    public bool Next(out UtteranceSegment<T> phrase, out int advanced)
    {
        switch (PositionalIndex)
        {
            case 1:
                if (_text[^1] == '0')
                {
                    phrase = _tree.Digit(_text[^2], _isOrdinal ? _mappers.TenOrdinal : _mappers.TenExact);
                    advanced = 2;
                    PositionalIndex = -1;
                    return true;
                }

                phrase = _tree.Digit(Digit, _mappers.Ten);
                advanced = 1;
                PositionalIndex = 0;
                return true;
            case 0:
                phrase = _tree.Digit(Digit, _isOrdinal ? _mappers.OneOrdinal : _mappers.OneExact);
                advanced = 1;
                PositionalIndex = -1;
                return true;
            default:
                phrase = default;
                advanced = 0;
                return false;
        }
    }

}
