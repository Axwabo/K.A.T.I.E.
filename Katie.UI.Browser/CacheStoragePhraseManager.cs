using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class CacheStoragePhraseManager : IPhraseCacheManager
{

    public Task CacheAsync(WaveStreamPhrase phrase, string language)
        => phrase is MemoryStreamPhrase memory
            ? PhraseCacheFunctions.Save(language, phrase.Text, memory.Data)
            : Task.CompletedTask;

    public Task ClearAsync() => PhraseCacheFunctions.DeleteAll();

}
