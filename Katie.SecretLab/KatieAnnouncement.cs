using Cassie;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Phrases;
using Mirror;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.SecretLab;

public sealed class KatieAnnouncement : CassieAnnouncement
{

    private const float NoisyVolume = 1.2f;
    private const float Cooldown = 3;

    private static readonly TimeSpan StartNoise = TimeSpan.FromSeconds(2.5);

    private double _signalFinishTime;

    public TimeSpan SignalDuration { get; }

    public UtteranceChain Chain { get; }

    public ISampleProvider Provider { get; }

    public override float PostAnnouncementCooldown => Cooldown;

    public bool SignalPlayed => NetworkTime.time >= _signalFinishTime;

    internal static void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal, bool noisy, bool showSubtitles = true)
    {
        var chain = UtteranceChain.From(text, tree);
        if (chain == null)
            return;
        ISampleProvider? provider;
        TimeSpan signalDuration;
        if (!signal.IsEmpty && PhraseCache.TryGetSignal(signal, out var signalProvider))
        {
            provider = signalProvider.FollowedBy(chain);
            signalDuration = signalProvider.TotalTime;
        }
        else
        {
            provider = noisy
                ? new OffsetSampleProvider(new VolumeSampleProvider(chain) {Volume = NoisyVolume}) {DelayBy = StartNoise}
                : chain;
            signalDuration = TimeSpan.Zero;
        }

        var (announcement, subtitles) = SubtitleHandler.MakeCassieAnnouncement(chain, text);
        new KatieAnnouncement(signalDuration, chain, provider, new CassieTtsPayload(announcement, subtitles, noisy)).AddToQueue();
    }

    private KatieAnnouncement(TimeSpan signalDuration, UtteranceChain chain, ISampleProvider provider, CassieTtsPayload payload, float priority = 1) : base(payload, priority, 0)
    {
        SignalDuration = signalDuration;
        Chain = chain;
        Provider = provider;
    }

    public override void OnStartedPlaying()
    {
        if (_signalFinishTime != 0)
            return;
        _signalFinishTime = NetworkTime.time + SignalDuration.TotalSeconds;
        var duration = SignalDuration.TotalSeconds + Chain.TotalTime.TotalSeconds;
        CassieAnnouncementDispatcher.AnnouncementFinishTime = NetworkTime.time + duration;
        CassieAnnouncementDispatcher.NextAnnouncementTime = CassieAnnouncementDispatcher.AnnouncementFinishTime + Cooldown;
    }

}
