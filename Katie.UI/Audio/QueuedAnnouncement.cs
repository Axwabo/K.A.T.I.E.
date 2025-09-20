using Katie.Core;
using Katie.NAudio;
using Katie.UI.Extensions;
using Katie.UI.Services;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.Audio;

public sealed record QueuedAnnouncement(string Text, string Language, Queue<UtteranceSegment<WavePhraseBase>> Segments, SimpleWaveFormat Format, Signal Signal)
{

    public bool DefaultSignal => Signal == SignalManager.DefaultSignal;

    private ISampleProvider? _provider;

    public ISampleProvider Provider
    {
        get
        {
            if (_provider != null)
                return _provider;
            ISampleProvider provider = new UtteranceChain(Segments.Dequeue(), Segments, Format);
            if (!DefaultSignal)
                provider = new RestartingSampleProvider(Signal.Provider, Format).FollowedBy(provider);
            return _provider = provider.LeadOut(TimeSpan.FromSeconds(3));
        }
    }

}
