using System.IO;
using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

public sealed class DirectoryPhraseProvider : IPhraseProvider
{

    private readonly string _directory;

    public DirectoryPhraseProvider(string directory) => _directory = directory;

    public async IAsyncEnumerable<SamplePhraseBase> EnumeratePhrasesAsync()
    {
        foreach (var file in Directory.EnumerateFiles(_directory, "*.wav"))
            yield return new WaveStreamPhrase(File.OpenRead(file), Path.GetFileNameWithoutExtension(file));
        await Task.CompletedTask;
    }

}
