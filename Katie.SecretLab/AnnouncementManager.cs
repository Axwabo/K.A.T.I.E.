using Cassie;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
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
        var player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, Settings).WithMasterAmplification(2).UnsetProviderOnEnd();
        Instance = player.gameObject.AddComponent<AnnouncementManager>();
        Instance.Player = player;
    }

    private KatieAnnouncement? _previous;

    public AudioPlayer Player { get; private set; } = null!;

    // ReSharper disable once MergeIntoPattern
    public bool IsSpeaking => !Player.IsPaused && !Player.HasEnded;

    private void Awake() => Instance = this;

    private void Update()
    {
        var current = CassieAnnouncementDispatcher.CurrentAnnouncement as KatieAnnouncement;
        if (current == _previous)
            return;
        Player.SampleProvider = current?.Chain;
        _previous = current;
    }

}
