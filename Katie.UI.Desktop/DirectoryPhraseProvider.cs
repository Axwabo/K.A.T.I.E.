using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Katie.NAudio;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

public sealed class DirectoryPhraseProvider : IPhraseProvider
{

    private readonly string _directory;

    public DirectoryPhraseProvider(string directory) => _directory = directory;

    public async IAsyncEnumerable<WavePhrase> EnumeratePhrasesAsync()
    {
        foreach (var file in Directory.EnumerateFiles(_directory, "*.wav"))
            yield return new WavePhrase(File.OpenRead(file), Path.GetFileNameWithoutExtension(file));
        await Task.CompletedTask;
    }

}
