using Katie.Core;
using NAudio.Wave;

namespace Katie.NAudio.Phrases;

public abstract class WavePhraseBase : PhraseBase
{

    public abstract SimpleWaveFormat WaveFormat { get; }

    public abstract ISampleProvider ToSampleProvider();

}
