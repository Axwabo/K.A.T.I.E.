using System.Collections.Generic;
using System.IO;
using Avalonia.Platform.Storage;
using Katie.NAudio;

namespace Katie.UI.PhraseProviders;

internal sealed class FilePickerPhraseProvider : IPhraseProvider
{

    private static readonly FilePickerOpenOptions Options = new()
    {
        Title = "Add phrases",
        AllowMultiple = true,
        FileTypeFilter =
        [
            new FilePickerFileType("Wave files")
            {
                Patterns = ["*.wav"],
                MimeTypes = ["audio/wav"]
            }
        ]
    };

    private readonly IStorageProvider _storageProvider;

    public FilePickerPhraseProvider(IStorageProvider storageProvider) => _storageProvider = storageProvider;

    public async IAsyncEnumerable<WavePhrase> EnumeratePhrasesAsync()
    {
        foreach (var file in await _storageProvider.OpenFilePickerAsync(Options))
            yield return new WavePhrase(await file.OpenReadAsync(), Path.GetFileNameWithoutExtension(file.Name));
    }

}
