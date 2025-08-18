using Katie.Core;
using NAudio.Wave;

namespace Katie.NAudio.Phrases;

public abstract class SamplePhraseBase : PhraseBase
{

    public abstract ISampleProvider ToSampleProvider();

}
