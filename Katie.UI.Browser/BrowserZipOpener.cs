using System.IO.Compression;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class BrowserZipOpener : IZipOpener
{

    public async Task<ZipArchive> OpenArchiveAsync(IStorageFile file) => new(new AsyncWrapperStream(await file.OpenReadAsync()));

}
