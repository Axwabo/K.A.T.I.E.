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

    public bool Play(string text)
    {
        var span = text.AsSpan().Trim();
        if (span[0] != '[')
            return false;
        var end = span.IndexOf(']');
        if (end == -1)
            return false;
        var language = span[1..end];
        if (!PhraseCache.TryGetTree(language, out var tree))
            return false;
        Player.SampleProvider = PhraseChain.Parse(span[(end + 1)..].Trim(), tree, language);
        return true;
    }

}
