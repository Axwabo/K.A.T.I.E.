using Cassie;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Phrases;
using Mirror;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Providers;
using Utils.Networking;

namespace Katie.SecretLab;

public sealed class KatieAnnouncement : CassieAnnouncement
{

    private const float NoisyVolume = 1.2f;
    private const float Cooldown = 3;

    private static readonly TimeSpan StartNoise = TimeSpan.FromSeconds(2.5);

    private double _signalFinishTime;

    private bool _payloadSent;

    public RawSourceSampleProvider? Signal { get; }

    public UtteranceChain Chain { get; }

    public ISampleProvider Provider { get; }

    public bool SignalPlayed => NetworkTime.time >= _signalFinishTime;

    public double SignalDuration => Signal?.TotalTime.TotalSeconds ?? 0;

    public override float PostAnnouncementCooldown => Cooldown;

    internal static void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal, bool noisy, bool showSubtitles = true)
    {
        var chain = UtteranceChain.From(text, tree);
        if (chain == null)
            return;
        ISampleProvider? provider;
        if (!signal.IsEmpty && PhraseCache.TryGetSignal(signal, out var signalProvider))
            provider = signalProvider.FollowedBy(chain);
        else
        {
            signalProvider = null;
            provider = noisy
                ? new OffsetSampleProvider(new VolumeSampleProvider(chain) {Volume = NoisyVolume}) {DelayBy = StartNoise}
                : chain;
        }

        var (announcement, subtitles) = SubtitleHandler.MakeCassieAnnouncement(chain, text);
        new KatieAnnouncement(signalProvider, chain, provider, new CassieTtsPayload(announcement, subtitles, noisy)).AddToQueue();
    }

    private KatieAnnouncement(RawSourceSampleProvider? signal, UtteranceChain chain, ISampleProvider provider, CassieTtsPayload payload, float priority = 1) : base(payload, priority, 0)
    {
        Signal = signal;
        Chain = chain;
        Provider = provider;
    }

    public override void OnStartedPlaying()
    {
        _signalFinishTime = NetworkTime.time + SignalDuration;
        var duration = (Payload.PlayBackground ? StartNoise.TotalSeconds : SignalDuration) + Chain.TotalTime.TotalSeconds;
        CassieAnnouncementDispatcher.AnnouncementFinishTime = NetworkTime.time + duration;
        CassieAnnouncementDispatcher.NextAnnouncementTime = CassieAnnouncementDispatcher.AnnouncementFinishTime + Cooldown;
    }

    public override void UpdatePlayed()
    {
        if (_payloadSent || !SignalPlayed)
            return;
        _payloadSent = true;
        Payload.SendToAuthenticated();
    }

}
