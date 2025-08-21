using Katie.NAudio;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Pools;
using UnityEngine;

namespace Katie.SecretLab;

internal sealed class AnnouncementManager : MonoBehaviour
{

    private static readonly SpeakerSettings Settings = new()
    {
        IsSpatial = false,
        MaxDistance = 10000,
        Volume = 1
    };

    public static AnnouncementManager Instance { get; private set; } = null!;

    public static void Initialize()
    {
        var player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, Settings);
        Instance = player.gameObject.AddComponent<AnnouncementManager>();
        Instance.Player = player;
    }

    public AudioPlayer Player { get; private set; } = null!;

    private void Awake() => Instance = this;

    public void Play(string text)
    {
        var chain = PhraseChain.Parse(text, PhraseCache.Tree, "Hungarian");
        if (chain != null)
            Player.SampleProvider = chain;
    }

}
