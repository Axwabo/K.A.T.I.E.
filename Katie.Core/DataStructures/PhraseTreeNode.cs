using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Katie.Core.Extensions;

namespace Katie.Core.DataStructures;

public sealed class PhraseTreeNode<T> where T : PhraseBase
{

    private readonly Dictionary<int, PhraseTreeNode<T>> _descendants = [];

    public string Key { get; }

    public T? Value { get; set; }

    public PhraseTreeNode(string key, T? value = null)
    {
        Key = key;
        Value = value;
    }

    public PhraseTreeNode<T> Set(ReadOnlySpan<char> key, T? value)
        => _descendants[key.LowercaseHashCode()] = new PhraseTreeNode<T>(key.ToString(), value);

    public bool TryGetDescendant(ReadOnlySpan<char> key, [NotNullWhen(true)] out PhraseTreeNode<T>? value)
        => _descendants.TryGetValue(key.LowercaseHashCode(), out value);

}
