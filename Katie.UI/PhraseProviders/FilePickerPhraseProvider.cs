using System.IO;
using Avalonia.Platform.Storage;
using Katie.UI.Services;

namespace Katie.UI.PhraseProviders;

internal sealed class FilePickerPhraseProvider : IPhraseProvider
{

    private static readonly FilePickerOpenOptions Options = new()
    {
        Title = "Add phrases",
        AllowMultiple = true,
        FileTypeFilter = [StorageWrapper.WaveFiles]
    };

    private readonly StorageWrapper _storage;

    private readonly IStreamToPhraseConverter _converter;

    public FilePickerPhraseProvider(StorageWrapper storage, IStreamToPhraseConverter converter)
    {
        _storage = storage;
        _converter = converter;
    }

    public async IAsyncEnumerable<WavePhraseBase> EnumeratePhrasesAsync()
    {
        foreach (var file in await _storage.OpenFilePickerAsync(Options))
        {
            var stream = await file.OpenReadAsync();
            WaveStreamPhrase phrase;
            try
            {
                phrase = await _converter.ToPhraseAsync(stream, Path.GetFileNameWithoutExtension(file.Name));
            }
            catch (Exception)
            {
                await stream.DisposeAsync();
                throw;
            }

            yield return phrase;
        }
    }

}
