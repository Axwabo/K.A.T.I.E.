using Katie.NAudio;
using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.Providers;
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

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(3);

    public static AnnouncementManager Instance { get; private set; } = null!;

    public static void Initialize()
    {
        var player = AudioPlayer.Create(AudioPlayerPool.NextAvailableId, Settings);
        Instance = player.gameObject.AddComponent<AnnouncementManager>();
        Instance.Player = player;
    }

    private readonly SampleProviderQueue _queue = new(AudioPlayer.SupportedFormat);

    public AudioPlayer Player { get; private set; } = null!;

    private void Awake() => Instance = this;

    private void Start() => Player.SampleProvider = _queue.Volume(2);

    public bool Play(string text)
    {
        var span = text.AsSpan().Trim();
        if (!span.TryGetBracketsValue(0, out var languageEnd, out var language) || !PhraseCache.TryGetTree(language.Trim(), out var tree))
            return false;
        languageEnd++;
        RawSourceSampleProvider? signal = null;
        if (span.TryGetBracketsValue(languageEnd, out var signalEnd, out var signalName) && PhraseCache.TryGetSignal(signalName.Trim(), out signal))
            languageEnd = signalEnd + 1;
        var announcement = span[languageEnd..].Trim();
        var chain = UtteranceChain.Parse(announcement, tree, language);
        if (chain == null)
            return true;
        if (signal != null)
        {
            signal.Position = 0;
            _queue.Enqueue(signal);
            _queue.Enqueue(new OffsetSampleProvider(chain) {LeadOut = Delay});
            Subtitles.PlaySilence(signal.TotalTime);
        }
        else
            _queue.Enqueue(new OffsetSampleProvider(chain) {LeadOut = Delay});

        Subtitles.PlayCassie(chain, announcement);
        Subtitles.PlaySilence(Delay);
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
