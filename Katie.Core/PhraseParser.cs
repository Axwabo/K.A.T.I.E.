using System;
using System.Collections.Generic;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;

namespace Katie.Core;

// TODO: to struct
public static class PhraseParser<T> where T : PhraseBase
{

    public static IEnumerable<T> Parse(ReadOnlySpan<char> text, PhraseTree<T> tree)
    {
        var list = new List<T>();
        var index = 0;
        while (true)
        {
            var end = text.IndexOfWordDelimiter(index);
            if (end++ == -1)
                break;
            Add(tree, list, text[index..end]);
            index = end;
        }

        Add(tree, list, text[index..]);
        return list;
    }

    private static void Add(PhraseTree<T> tree, List<T> list, ReadOnlySpan<char> token)
    {
        if (!token.IsWhiteSpace() && tree.TryGetRootValue(token.Trim().TrimDelimeters(), out var value))
            list.Add(value);
    }

}
