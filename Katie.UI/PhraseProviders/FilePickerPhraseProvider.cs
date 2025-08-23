using Avalonia.Platform.Storage;

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

    private readonly StorageWrapper _storage;

    private readonly IFileToPhraseConverter _converter;

    public FilePickerPhraseProvider(StorageWrapper storage, IFileToPhraseConverter converter)
    {
        _storage = storage;
        _converter = converter;
    }

    public async IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync()
    {
        // TODO: list getting to interface
        foreach (var file in await _storage.OpenFilePickerAsync(Options))
            yield return await _converter.ToPhraseAsync(file);
    }

}
