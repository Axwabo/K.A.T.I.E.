using System.Diagnostics.CodeAnalysis;
using System.IO;
using Katie.Core.DataStructures;
using Katie.NAudio.Phrases;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;
using Logger = LabApi.Features.Console.Logger;

namespace Katie.SecretLab;

public static class PhraseCache
{

    public static PhraseTree<SamplePhraseBase> Hungarian { get; private set; } = new([]);

    public static PhraseTree<SamplePhraseBase> English { get; private set; } = new([]);

    public static void Initialize(DirectoryInfo phrasesRoot)
    {
        var global = phrasesRoot.EnumeratePhrases("Global").ToList();
        Hungarian = new PhraseTree<SamplePhraseBase>(global.Concat(phrasesRoot.EnumeratePhrases("Hungarian")));
        English = new PhraseTree<SamplePhraseBase>(global.Concat(phrasesRoot.EnumeratePhrases("English")));
    }

    public static bool TryGetTree(ReadOnlySpan<char> language, [NotNullWhen(true)] out PhraseTree<SamplePhraseBase>? tree)
    {
        tree = language.Equals("Hungarian", StringComparison.OrdinalIgnoreCase)
            ? Hungarian
            : language.Equals("English", StringComparison.OrdinalIgnoreCase)
                ? English
                : null;
        return tree != null;
    }

    private static IEnumerable<SamplePhraseBase> EnumeratePhrases(this DirectoryInfo directory, string subdirectory)
    {
        foreach (var file in Directory.EnumerateFiles(directory.CreateSubdirectory(subdirectory).FullName, "*.wav"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            Logger.Debug($"Found {subdirectory} phrase: {name}");
            using var stream = CreateAudioReader.Stream(file);
            var samples = stream.ReadPlayerCompatibleSamples();
            yield return new RawSourcePhrase(samples, name);
        }
    }

}
