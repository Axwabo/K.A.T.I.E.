using Katie.Core;
using UnityEngine;

namespace Katie.Unity;

public sealed class AudioClipPhrase : PhraseBase
{

    public AudioClip Clip { get; }

    public override string Text { get; }

    public override TimeSpan Duration { get; }

    public AudioClipPhrase(AudioClip clip)
    {
        Clip = clip;
        Text = clip.name;
        Duration = TimeSpan.FromSeconds(clip.length);
    }

}
