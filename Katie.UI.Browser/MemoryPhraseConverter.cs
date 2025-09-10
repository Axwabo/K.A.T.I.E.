using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Katie.NAudio.Phrases;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class MemoryPhraseConverter : IFileToPhraseConverter
{

    public async Task<WaveStreamPhrase> ToPhraseAsync(IStorageFile file)
    {
        var name = Path.GetFileNameWithoutExtension(file.Name);
        await using var stream = await file.OpenReadAsync();
        var memory = await stream.CopyToMemory();
        return new MemoryStreamPhrase(memory, name);
    }

}
