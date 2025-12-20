using Katie.Core.DataStructures;

namespace Katie.Core.Extensions;

public static class PhraseTreeExtensions
{

    extension<T>(PhraseTree<T> tree) where T : PhraseBase
    {

        public UtteranceSegment<T> RootPhrase(TreeKey key)
            => key.Length == 0 || !tree.TryGetRootValue(key, out var phrase)
                ? key.Length
                : phrase;

        public UtteranceSegment<T> Digit(char digit, Func<char, string> mapper) => tree.RootPhrase(mapper(digit));

    }

}
