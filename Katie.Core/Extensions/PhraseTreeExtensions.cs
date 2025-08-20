using Katie.Core.DataStructures;

namespace Katie.Core.Extensions;

public static class PhraseTreeExtensions
{

    public static UtteranceSegment<T> RootPhrase<T>(this PhraseTree<T> tree, TreeKey key) where T : PhraseBase
        => key.Length == 0 || !tree.TryGetRootValue(key, out var phrase)
            ? key.Length
            : phrase;

    public static UtteranceSegment<T> Digit<T>(this PhraseTree<T> tree, char digit, Func<char, string> mapper) where T : PhraseBase
        => tree.RootPhrase(mapper(digit));

}
