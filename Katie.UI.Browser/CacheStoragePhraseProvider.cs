using System.IO;
using Katie.NAudio.Phrases;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;
using NAudio.Wave;

namespace Katie.UI.Browser;

public sealed class CacheStoragePhraseProvider : IPhraseProvider
{

    public string Language => "Global";

    public async IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync()
    {
        foreach (var (name, bytes) in await CacheFunctions.LoadMemoryStream())
        {
            using var stream = new MemoryStream(bytes);
            await using var reader = new WaveFileReader(stream);
            var provider = reader.ToSampleProvider();
            var raw = provider.ReadSamples(reader.TotalTime);
            yield return new RawSourceSamplePhrase(raw, name);
        }
    }

}
