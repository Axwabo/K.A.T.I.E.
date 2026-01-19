using Katie.Core.DataStructures;
using UnityEngine;

namespace Katie.Unity;

[CreateAssetMenu(fileName = "Phrase Pack", menuName = "K.A.T.I.E./Phrase Pack", order = 0)]
public sealed class PhrasePack : ScriptableObject
{

#nullable disable
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private string language;

    public PhraseTree<AudioClipPhrase> Tree { get; private set; }
#nullable restore

    private void OnEnable() => Rebuild();

    private void OnValidate() => Rebuild();

    private void Rebuild()
    {
        Tree = new PhraseTree<AudioClipPhrase>(language);
        Tree.Rebuild(clips.Where(static e => e != null).Select(static e => new AudioClipPhrase(e)));
    }

}
