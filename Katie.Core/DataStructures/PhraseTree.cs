using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Katie.Core.Extensions;

namespace Katie.Core.DataStructures;

public sealed class PhraseTree<T> where T : PhraseBase
{

    private readonly PhraseTreeNode<T> _rootNode = new("");

    public PhraseTree(IEnumerable<T> values)
    {
        foreach (var phrase in values)
            Add(phrase);
    }

    private void Add(T phrase)
    {
        var key = phrase.Text;
        var span = key.AsSpan();
        var index = span.IndexOfWordDelimiter(0);
        var firstToken = index == -1 ? span : span[..index];

        if (!_rootNode.TryGetDescendant(firstToken, out var node))
            node = _rootNode.Set(firstToken, null);

        if (index == -1)
        {
            node.Value = phrase;
            return;
        }

        var previousIndex = index;
        index = span.IndexOfWordDelimiter(index + 1);
        while (index >= 0)
        {
            var currentToken = span[previousIndex..index];
            node = node.TryGetDescendant(currentToken, out var nextNode)
                ? nextNode
                : node.Set(currentToken.Trim(), null);
            previousIndex = index;
            index = span.IndexOfWordDelimiter(index + 1);
        }

        if (node.Key == span)
            node.Value = phrase;
        else
            node.Set(span[previousIndex..].Trim(), phrase);
    }

    public bool TryGetRootNode(ReadOnlySpan<char> token, [NotNullWhen(true)] out PhraseTreeNode<T>? node)
        => _rootNode.TryGetDescendant(token, out node);

    public bool TryGetRootValue(ReadOnlySpan<char> token, [NotNullWhen(true)] out T? value)
    {
        if (_rootNode.TryGetDescendant(token, out var node))
        {
            value = node.Value;
            return value != null;
        }

        value = null;
        return false;
    }

}
