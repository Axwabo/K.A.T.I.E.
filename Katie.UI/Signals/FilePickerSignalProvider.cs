using System.IO;
using Avalonia.Platform.Storage;
using Katie.UI.Extensions;
using NAudio.Wave;

namespace Katie.UI.Signals;

internal sealed class FilePickerSignalProvider : ISignalProvider
{

    private static readonly FilePickerOpenOptions Options = new()
    {
        Title = "Add signals",
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

    public FilePickerSignalProvider(IStorageProvider storageProvider) => _storageProvider = storageProvider;

    public async IAsyncEnumerable<Signal> EnumerateSignalsAsync()
    {
        foreach (var file in await _storageProvider.OpenFilePickerAsync(Options))
        {
            await using var fileStream = await file.OpenReadAsync();
            await using var reader = new WaveFileReader(fileStream);
            var provider = await Task.Run(() => reader.ToSampleProvider().ReadSamples(reader.TotalTime));
            yield return new Signal(provider, Path.GetFileNameWithoutExtension(file.Name), reader.TotalTime);
        }
    }

}
