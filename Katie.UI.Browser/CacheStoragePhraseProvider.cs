using System.IO;
using System.Threading.Tasks;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;
using Katie.UI.ViewModels;
using NAudio.Wave;

namespace Katie.UI.Browser;

public sealed class CacheStoragePhraseProvider : IInitialPhraseProvider
{

    private static async Task<IReadOnlyCollection<RawSourceSamplePhrase>> EnumeratePhrasesAsync(string language)
    {
        try
        {
            await CacheFunctions.PrepareCache(language);
            var keys = CacheFunctions.GetKeys();
            var list = new List<RawSourceSamplePhrase>(keys.Length);
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
                list.Add(new RawSourceSamplePhrase(raw, key));
            }

            return list;
        }
        finally
        {
            CacheFunctions.ClearMemory();
        }
    }

    public async Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        hungarian.ReplacePhrases(await EnumeratePhrasesAsync("Hungarian"));
        english.ReplacePhrases(await EnumeratePhrasesAsync("English"));
        global.ReplacePhrases(await EnumeratePhrasesAsync("Global"));
    }

}
