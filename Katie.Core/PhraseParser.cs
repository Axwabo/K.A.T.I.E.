using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;
using Katie.Core.NumberParsing;

namespace Katie.Core;

public ref struct PhraseParser<T> where T : PhraseBase
{

    private readonly PhraseTree<T> _tree;
    private readonly ReadOnlySpan<char> _text;
    private NumericParserWrapper<T> _numericParser;

    private int _index;

    public PhraseParser(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _tree = tree;
        _text = text;
        _numericParser = new NumericParserWrapper<T>(text, tree);
    }

    public bool Next(out UtteranceSegment<T> phrase)
    {
        if (_numericParser.Next(ref _index, out phrase))
            return true;

        if (!SkipWhitespaces())
        {
            phrase = default;
            return false;
        }

        if (TryParseSilence(out phrase))
            return true;

        if (!TryLookAhead(out var primaryToken, out var primaryEnd))
        {
            phrase = default;
            return false;
        }

        if (_numericParser.Begin(ref _index, primaryEnd, out phrase)
            || TryParsePhrase(primaryToken, out phrase))
            return true;

        if (primaryToken.Length > 0)
        {
            Commit(primaryToken);
            phrase = primaryToken.Length;
            return true;
        }

        phrase = default;
        return false;
    }

    private bool SkipWhitespaces()
    {
        if (_index == -1)
            return false;
        while (_index < _text.Length - 1)
        {
            if (!char.IsWhiteSpace(_text[_index]))
                return true;
            _index++;
        }

        _index = -1;
        return false;
    }

    private bool TryParseSilence(out UtteranceSegment<T> phrase)
    {
        var duration = _text[_index] switch
        {
            '.' => 0.5,
            ',' => 0.3,
            _ => 0
        };
        if (duration is 0)
        {
            phrase = default;
            return false;
        }

        Commit(1);
        phrase = duration;
        return true;
    }

    private bool TryLookAhead(out ReadOnlySpan<char> token, out int endIndex)
    {
        if (_index == -1)
        {
            token = default;
            endIndex = -1;
            return false;
        }

        var delimiter = _text.IndexOfWordDelimiter(_index);
        endIndex = delimiter == -1 ? _text.Length : delimiter;
        var length = endIndex - _index;
        token = _text[_index..endIndex];
        return length > 0;
    }

    private bool TryParsePhrase(ReadOnlySpan<char> primaryToken, out UtteranceSegment<T> phrase)
    {
        if (!_tree.TryGetRootNode(primaryToken, out var lastNode))
        {
            phrase = default;
            return false;
        }

        var start = _index;
        Commit(primaryToken);
        var lastSuccessful = lastNode.Value != null ? lastNode : null;
        while (lastNode is {HasDescendants: true})
        {
            SkipWhitespaces();
            if (!TryLookAhead(out var token, out var endIndex)
                || !lastNode.TryGetDescendant(token, out lastNode))
                break;
            lastSuccessful = lastNode.Value != null ? lastNode : lastSuccessful;
            _index = endIndex;
        }

        if (lastSuccessful != null)
        {
            phrase = lastSuccessful.Value;
            return true;
        }

        _index = start;
        phrase = default;
        return false;
    }

    private void Commit(ReadOnlySpan<char> phrase) => Commit(phrase.Length);

    private void Commit(int characters)
    {
        _index += characters;
        if (_index >= _text.Length - 1)
            _index = -1;
    }

}
