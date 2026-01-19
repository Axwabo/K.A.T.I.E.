using Katie.Core.DataStructures;
using UnityEngine;

namespace Katie.Unity;

public static class PhraseTreeBuilder
{

    public static PhraseTree<AudioClipPhrase> Create(string language, IEnumerable<AudioClip> clips)
    {
        var tree = new PhraseTree<AudioClipPhrase>(language);
        tree.Rebuild(clips.Select(static e => new AudioClipPhrase(e)));
        return tree;
    }

}
