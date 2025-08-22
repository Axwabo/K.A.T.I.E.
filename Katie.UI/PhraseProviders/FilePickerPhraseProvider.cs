using System.IO;
using Avalonia.Platform.Storage;
using Katie.UI.Extensions;

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

    public async IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync()
    {
        foreach (var file in await _storageProvider.OpenFilePickerAsync(Options))
        {
            var name = Path.GetFileNameWithoutExtension(file.Name);
            if (!IPhraseProvider.IsBrowser)
            {
                yield return new WaveStreamPhrase(await file.OpenReadAsync(), name);
                continue;
            }

            await using var stream = await file.OpenReadAsync();
            var memory = await stream.ToMemoryStream();
            yield return new WaveStreamPhrase(memory, name);
        }
    }

}
