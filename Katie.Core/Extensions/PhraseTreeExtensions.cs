using System;
using Katie.Core.DataStructures;

namespace Katie.Core.Extensions;

public static class PhraseTreeExtensions
{

    public static UtteranceSegment<T> Digit<T>(this PhraseTree<T> tree, char digit, Func<char, string> mapper) where T : PhraseBase
    {
        var key = mapper(digit);
        return string.IsNullOrEmpty(key) || !tree.TryGetRootValue(key, out var phrase)
            ? 1
            : phrase;
    }

}
