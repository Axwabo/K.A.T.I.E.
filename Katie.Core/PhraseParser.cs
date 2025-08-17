using System;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core;

// TODO: to struct
public ref struct PhraseParser<T> where T : PhraseBase
{

    private readonly PhraseTree<T> _tree;
    private readonly ReadOnlySpan<char> _text;

    private int _index;

    public PhraseParser(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        _tree = tree;
        _text = text;
    }

    public bool Next(out UtteranceSegment<T> phrase)
    {
        if (!SkipDelimiters() || !TryLookAhead(out var primaryToken, out var primaryEnd))
        {
            phrase = default;
            return false;
        }

        _index = primaryEnd;
        // TODO: parse multiple tokens
        var trimmed = primaryToken.Trim().TrimDelimeters();
        if (!_tree.TryGetRootNode(trimmed, out var node))
        {
            phrase = TimeSpan.FromMilliseconds(trimmed.Length * 30);
            return true;
        }

        phrase = node.Value;
        return true;
    }

    private bool SkipDelimiters()
    {
        if (_index == -1)
            return false;
        while (_index < _text.Length - 1)
        {
            if (!SpanExtensions.Delimiters.Contains(_text[_index]))
                return true;
            _index++;
        }

        _index = -1;
        return false;
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

    private void Commit(ReadOnlySpan<char> phrase) => Commit(phrase.Length);

    private void Commit(int characters)
    {
        _index += characters;
        if (_index >= _text.Length - 1)
            _index = -1;
    }

}
