using Katie.Core.DataStructures;
using Katie.Core.NumberParsing.English;
using Katie.Core.NumberParsing.Hungarian;

namespace Katie.Core.NumberParsing;

public ref struct NumericParserWrapper<T> where T : PhraseBase
{

    private readonly ReadOnlySpan<char> _text;
    private readonly PhraseTree<T> _tree;
    private readonly bool _isEnglish;

    private HungarianNumericParser<T> _hungarian;
    private EnglishNumericParser<T> _english;

    public NumericParserWrapper(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _text = text;
        _tree = tree;
        _isEnglish = tree.Language.Equals("English", StringComparison.OrdinalIgnoreCase);
    }

    public bool Begin(ref int index, int primaryEnd, out UtteranceSegment<T> phrase)
    {
        if (!char.IsDigit(_text[index]))
        {
            phrase = default;
            return false;
        }

        if (_isEnglish)
        {
            _english = new EnglishNumericParser<T>(_text, _tree);
            return _english.Begin(ref index, primaryEnd, out phrase);
        }

        _hungarian = new HungarianNumericParser<T>(_text, _tree);
        return _hungarian.Begin(ref index, primaryEnd, out phrase);
    }

    public bool Next(ref int index, out UtteranceSegment<T> phrase)
    {
        if (_hungarian.IsActive)
            return _hungarian.Next(ref index, out phrase);
        if (_english.IsActive)
            return _english.Next(ref index, out phrase);
        phrase = default;
        return false;
    }

}
