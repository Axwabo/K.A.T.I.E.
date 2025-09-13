using System.IO;
using Katie.UI.Signals;

namespace Katie.UI.Desktop;

public sealed class DirectorySignalProvider : ISignalProvider
{

    private readonly string _directory;

    public DirectorySignalProvider(string directory) => _directory = directory;

    public async IAsyncEnumerable<Signal> EnumerateSignalsAsync()
    {
        foreach (var file in Directory.EnumerateFiles(_directory, "*.wav"))
        {
            await using var fileStream = File.OpenRead(file);
            yield return await Signal.LoadIntoMemory(fileStream, file);
        }
    }

}
