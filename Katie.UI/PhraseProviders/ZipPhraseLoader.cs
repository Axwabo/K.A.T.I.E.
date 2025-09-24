using System.IO.Compression;
using Avalonia.Platform.Storage;
using Katie.UI.Services;
using Path = System.IO.Path;

namespace Katie.UI.PhraseProviders;

internal sealed class ZipPhraseLoader : IInitialPhraseLoader
{

    private static readonly FilePickerOpenOptions Options = new()
    {
        Title = "Choose phrase archive",
        FileTypeFilter =
        [
            new FilePickerFileType("ZIP archives")
            {
                Patterns = ["*.zip"],
                MimeTypes = ["application/zip"]
            }
        ]
    };

    private readonly StorageWrapper _storage;
    private readonly MemoryPhraseConverter _converter = new();

    public ZipPhraseLoader(StorageWrapper storage) => _storage = storage;

    public async Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        if (await _storage.OpenFilePickerAsync(Options) is not [var file])
            return;
        using var zipArchive = new ZipArchive(await file.OpenReadAsync());
        await LoadEntries(zipArchive, hungarian, english, global);
    }

    private async Task LoadEntries(ZipArchive archive, PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        foreach (var entry in archive.Entries)
        {
            var span = entry.FullName.AsSpan();
            var slash = span.IndexOfAny('/', '\\');
            if (slash == -1 || !Path.GetExtension(span).Equals(".wav", StringComparison.OrdinalIgnoreCase))
                continue;
            var directory = span[..slash];
            var target =
                directory.Equals("Hungarian", StringComparison.OrdinalIgnoreCase)
                    ? hungarian
                    : directory.Equals("English", StringComparison.OrdinalIgnoreCase)
                        ? english
                        : directory.Equals("Global", StringComparison.OrdinalIgnoreCase)
                            ? global
                            : null;
            if (target == null)
                continue;
            await using var stream = entry.Open();
            target.Add(await _converter.ToPhraseAsync(stream, Path.GetFileNameWithoutExtension(entry.FullName)));
        }
    }

}
