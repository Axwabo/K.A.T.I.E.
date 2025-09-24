using System.IO.Compression;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class BrowserZipOpener : IZipOpener
{

    public async Task<ZipArchive> OpenArchiveAsync(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        var memory = await stream.CopyToMemory();
        return new ZipArchive(memory);
    }

}
