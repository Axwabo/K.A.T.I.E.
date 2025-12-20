using Cassie;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Phrases;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.SecretLab;

public sealed class KatieAnnouncement : CassieAnnouncement
{

    private const float NoisyVolume = 1.2f;

    private static readonly TimeSpan StartNoise = TimeSpan.FromSeconds(2.5);

    public UtteranceChain Chain { get; }

    public ISampleProvider Provider { get; }

    public override float PostAnnouncementCooldown => 3;

    internal static void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal, bool noisy, bool showSubtitles = true)
    {
        var chain = UtteranceChain.From(text, tree);
        if (chain == null)
            return;
        var provider = !signal.IsEmpty && PhraseCache.TryGetSignal(signal, out var signalProvider)
            ? signalProvider.FollowedBy(chain)
            : noisy
                ? new OffsetSampleProvider(new VolumeSampleProvider(chain) {Volume = NoisyVolume}) {DelayBy = StartNoise}
                : chain;
        var (announcement, subtitles) = SubtitleHandler.MakeCassieAnnouncement(chain, text);
        new KatieAnnouncement(chain, provider, new CassieTtsPayload(announcement, subtitles, noisy)).AddToQueue();
    }

    private KatieAnnouncement(UtteranceChain chain, ISampleProvider provider, CassieTtsPayload payload, float priority = 0) : base(payload, priority, 0)
    {
        Chain = chain;
        Provider = provider;
    }

}
