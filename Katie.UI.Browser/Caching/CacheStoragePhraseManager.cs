using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.Browser.JSInterop;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser.Caching;

public sealed class CacheStoragePhraseManager : IPhraseCacheManager
{

    public string Info => "Caching saves phrases to the browser's cache storage, persisting them across sessions.";

    public Task CacheAsync(WaveStreamPhrase phrase, string language)
        => phrase is MemoryStreamPhrase memory
            ? PhraseCacheFunctions.Save(language, phrase.Text, memory.Data)
            : Task.CompletedTask;

    public Task ClearAsync() => PhraseCacheFunctions.DeleteAll();

}
