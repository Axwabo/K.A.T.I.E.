using System.IO.Compression;
using Avalonia.Platform.Storage;

namespace Katie.UI.PhraseProviders;

public interface IZipOpener
{

    Task<ZipArchive> OpenArchiveAsync(IStorageFile file);

}
