using System.IO;
using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;
using NAudio.Wave;

namespace Katie.UI.Browser;

public sealed class CacheStoragePhraseProvider : IPhraseProvider
{

    public required string Language { get; init; }

    public async IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync()
    {
        try
        {
            await CacheFunctions.PrepareCache(Language);
            var keys = CacheFunctions.GetKeys();
            foreach (var key in keys.Order())
            {
                var bytes = CacheFunctions.Load(key);
                var raw = await Task.Run(() =>
                {
                    using var stream = new MemoryStream(bytes);
                    using var reader = new WaveFileReader(stream);
                    var provider = reader.ToSampleProvider();
                    return provider.ReadSamples(reader.TotalTime);
                });
                yield return new RawSourceSamplePhrase(raw, key);
            }
        }
        finally
        {
            CacheFunctions.ClearMemory();
        }
    }

}
