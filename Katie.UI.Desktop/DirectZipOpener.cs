using System.IO.Compression;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

public sealed class DirectZipOpener : IZipOpener
{

    public async Task<ZipArchive> OpenArchiveAsync(IStorageFile file) => new(await file.OpenReadAsync());

}
