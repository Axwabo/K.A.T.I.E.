using System.Threading.Tasks;
using Katie.UI.PhraseProviders;
using Katie.UI.ViewModels;

namespace Katie.UI.Browser.Caching;

public sealed class CacheStoragePhraseLoader : IInitialPhraseLoader
{

    public async Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        await hungarian.AddPhrases(new CacheStoragePhraseProvider {Language = "Hungarian"});
        await english.AddPhrases(new CacheStoragePhraseProvider {Language = "English"});
        await global.AddPhrases(new CacheStoragePhraseProvider {Language = "Global"});
    }

}
