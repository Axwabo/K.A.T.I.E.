using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Phrases;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Providers;
using UnityEngine;

namespace Katie.SecretLab;

using Announcement = (string Announcement, string? Subtitles, bool Noisy);

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
        var player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, Settings).WithMasterAmplification(2);
        Instance = player.gameObject.AddComponent<AnnouncementManager>();
        Instance.Player = player;
    }

    private readonly SampleProviderQueue _queue = new(AudioPlayer.SupportedFormat);

    private readonly Dictionary<ISampleProvider, Announcement> _announcements = [];

    private ISampleProvider? _previousProvider;

    private float _pauseTime;

    public AudioPlayer Player { get; private set; } = null!;

    // ReSharper disable once MergeIntoPattern
    public bool IsSpeaking => !Player.IsPaused && !Player.HasEnded;

    private void Awake() => Instance = this;

    private void Start() => Player.SampleProvider = _queue;

    private void Update()
    {
        if (KatieAnnouncer.IsCassieSpeaking)
            _pauseTime = EndDelaySeconds;
        if ((_pauseTime -= Time.deltaTime) >= 0)
        {
            Player.IsPaused = true;
            return;
        }

        Player.IsPaused = false;
        var current = _queue.Current;
        if (current == _previousProvider)
            return;
        _previousProvider = current;
        if (current == null || !_announcements.Remove(current, out var tuple))
            return;
        SubtitleHandler.Announce(tuple.Announcement, tuple.Subtitles, tuple.Noisy);
        if (!tuple.Noisy)
            SubtitleHandler.ServerOnlyDelay(EndDelaySeconds);
    }

    public bool OverrideCassieAnnouncement(string text, bool noisy)
    {
        var span = text.AsSpan().Trim();
        if (!span.TryGetBracketsValue(0, out var languageEnd, out var language) || !PhraseCache.TryGetTree(language.Trim(), out var tree))
            return false;
        languageEnd++;
        if (span.TryGetBracketsValue(languageEnd, out var signalEnd, out var signal))
            languageEnd = signalEnd + 1;
        var announcement = span[languageEnd..].Trim();
        Play(announcement, tree, signal, noisy && signal.Trim().IsEmpty);
        return true;
    }

    public void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal, bool noisy, bool showSubtitles = true)
    {
        var chain = UtteranceChain.From(text, tree);
        if (chain == null)
            return;
        if (!signal.IsEmpty && PhraseCache.TryGetSignal(signal, out var signalProvider))
            _queue.Enqueue(signalProvider.Copy(true));
        var offset = new OffsetSampleProvider(noisy ? chain.Volume(1.2f) : chain)
        {
            DelayBy = noisy ? StartNoise : TimeSpan.Zero,
            LeadOut = noisy ? Delay + EndNoise : Delay
        };
        _queue.Enqueue(offset);
        var (announcement, subtitles) = SubtitleHandler.MakeCassieAnnouncement(chain, text);
        _announcements[offset] = (announcement, showSubtitles ? subtitles : null, noisy);
    }

    public void Stop()
    {
        _queue.Clear();
        _queue.Next();
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
