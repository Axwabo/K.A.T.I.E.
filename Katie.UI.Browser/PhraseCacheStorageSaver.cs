using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class PhraseCacheStorageSaver : IPhraseCacheSaver
{

    public Task CacheAsync(WaveStreamPhrase phrase, string language)
        => phrase is MemoryStreamPhrase memory
            ? CacheFunctions.Save(language, phrase.Text, memory.Data)
            : Task.CompletedTask;

}
