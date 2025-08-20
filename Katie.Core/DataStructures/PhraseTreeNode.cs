using System.Diagnostics.CodeAnalysis;

namespace Katie.Core.DataStructures;

public sealed class PhraseTreeNode<T> where T : PhraseBase
{

    private readonly Dictionary<int, PhraseTreeNode<T>> _descendants = [];

    public string Key { get; }

    public T? Value { get; set; }

    public bool HasDescendants => _descendants.Count != 0;

    public PhraseTreeNode(string key, T? value = null)
    {
        Key = key;
        Value = value;
    }

    public PhraseTreeNode<T> Set(TreeKey key, T? value)
        => _descendants[key.GetHashCode()] = new PhraseTreeNode<T>(key.ToString(), value);

    public bool TryGetDescendant(TreeKey key, [NotNullWhen(true)] out PhraseTreeNode<T>? value)
        => _descendants.TryGetValue(key.GetHashCode(), out value);

}
