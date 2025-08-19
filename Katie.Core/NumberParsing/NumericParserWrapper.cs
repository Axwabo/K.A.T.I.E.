using System;
using Katie.Core.DataStructures;

namespace Katie.Core.NumberParsing;

public ref struct NumericParserWrapper<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;

    // TODO: language support

    private HungarianNumericParser<T> _hungarian;

    public NumericParserWrapper(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _text = text;
        _tree = tree;
    }

    public bool Begin(ref int index, int primaryEnd, out UtteranceSegment<T> phrase)
    {
        if (!char.IsDigit(_text[index]))
        {
            phrase = default;
            return false;
        }

        _hungarian = new HungarianNumericParser<T>(_text, _tree);
        return _hungarian.Begin(ref index, primaryEnd, out phrase);
    }

    public bool Next(ref int index, out UtteranceSegment<T> phrase)
    {
        if (_hungarian.IsActive)
            return _hungarian.Next(ref index, out phrase);
        phrase = default;
        return false;
    }

}
