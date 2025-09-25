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

    private UtteranceChain? _utteranceChain;

    private ISampleProvider? _provider;

    public TimeSpan CurrentTime => _utteranceChain?.CurrentTime ?? TimeSpan.Zero;

    public TimeSpan TotalTime => _utteranceChain?.TotalTime ?? TimeSpan.Zero;

    public ISampleProvider Provider
    {
        get
        {
            if (_provider != null)
                return _provider;
            _utteranceChain = new UtteranceChain(Segments.Dequeue(), Segments, Format);
            ISampleProvider provider = _utteranceChain;
            if (!DefaultSignal)
                provider = new RestartingSampleProvider(Signal.Provider, Format).FollowedBy(provider);
            return _provider = provider.LeadOut(TimeSpan.FromSeconds(3));
        }
    }

}
