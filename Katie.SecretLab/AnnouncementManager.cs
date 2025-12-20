using Cassie;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using UnityEngine;

namespace Katie.SecretLab;

internal sealed class AnnouncementManager : MonoBehaviour
{

    private const float EndDelaySeconds = 4;

    private static readonly SpeakerSettings Settings = new()
    {
        IsSpatial = false,
        MaxDistance = 10000,
        Volume = 1
    };

    private static readonly TimeSpan StartNoise = TimeSpan.FromSeconds(2.5);

    private static readonly TimeSpan EndNoise = TimeSpan.FromSeconds(0.2);

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(EndDelaySeconds);

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

    public static bool OverrideCassieAnnouncement(string text, bool noisy)
    {
        var span = text.AsSpan().Trim();
        if (!span.TryGetBracketsValue(0, out var languageEnd, out var language) || !PhraseCache.TryGetTree(language.Trim(), out var tree))
            return false;
        languageEnd++;
        if (span.TryGetBracketsValue(languageEnd, out var signalEnd, out var signal))
            languageEnd = signalEnd + 1;
        var announcement = span[languageEnd..].Trim();
        KatieAnnouncement.Play(announcement, tree, signal, noisy && signal.Trim().IsEmpty);
        return true;
    }

}

file static class Extensions
{

    public static bool TryGetBracketsValue(this ReadOnlySpan<char> span, int startIndex, out int endIndex, out ReadOnlySpan<char> match)
    {
        match = default;
        endIndex = 0;
        var search = span[startIndex..];
        var start = search.IndexOf('[');
        if (start == -1)
            return false;
        var end = search.IndexOf(']');
        if (end == -1)
            return false;
        endIndex = startIndex + end;
        match = span[(startIndex + start + 1)..endIndex];
        return true;
    }

}
