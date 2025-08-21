using System.Diagnostics.CodeAnalysis;
using System.IO;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;
using Katie.NAudio.Phrases;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;
using Logger = LabApi.Features.Console.Logger;

namespace Katie.SecretLab;

public static class PhraseCache
{

    private static readonly Dictionary<int, PhraseTree<SamplePhraseBase>> Trees = [];

    public static void Initialize(DirectoryInfo phrasesRoot)
    {
        var global = phrasesRoot.CreateSubdirectory("Global").EnumeratePhrases().ToList();
        foreach (var directory in phrasesRoot.EnumerateDirectories())
            if (directory.Name != "Global")
                Trees[directory.Name.AsSpan().LowercaseHashCode()] = new PhraseTree<SamplePhraseBase>(global.Concat(directory.EnumeratePhrases()));
    }

    public static bool TryGetTree(ReadOnlySpan<char> language, [NotNullWhen(true)] out PhraseTree<SamplePhraseBase>? tree)
        => Trees.TryGetValue(language.LowercaseHashCode(), out tree);

    private static IEnumerable<SamplePhraseBase> EnumeratePhrases(this DirectoryInfo directory)
    {
        foreach (var file in Directory.EnumerateFiles(directory.FullName, "*.wav"))
        {
            using var stream = CreateAudioReader.Stream(file);
            var samples = stream.ReadPlayerCompatibleSamples();
            var name = Path.GetFileNameWithoutExtension(file);
            Logger.Debug($"Added {directory.Name} phrase: {name}");
            yield return new RawSourcePhrase(samples, name);
        }
    }

}
