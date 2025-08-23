using System.IO;
using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;
using Katie.UI.ViewModels;

namespace Katie.UI.Desktop;

public sealed class DirectoryPhraseProvider : IInitialPhraseProvider
{

    public required string Root { get; init; }

    private static List<WaveStreamPhrase> GetPhrasesAsync(string directory)
    {
        var list = new List<WaveStreamPhrase>(20);
        foreach (var file in Directory.EnumerateFiles(directory, "*.wav"))
            list.Add(new WaveStreamPhrase(File.OpenRead(file), Path.GetFileNameWithoutExtension(file)));
        return list;
    }

    public Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        LoadDirectoryAsync(hungarian, "Hungarian");
        LoadDirectoryAsync(english, "English");
        LoadDirectoryAsync(global, "Global");
        return Task.CompletedTask;
    }

    private void LoadDirectoryAsync(PhrasePackViewModel model, string language)
    {
        var directory = Path.Combine(Root, language);
        if (Directory.Exists(directory))
            model.ReplacePhrases(GetPhrasesAsync(directory));
    }

}
