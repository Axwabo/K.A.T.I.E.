using System.IO;
using Katie.Core.DataStructures;
using Katie.NAudio.Phrases;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;

namespace Katie.SecretLab;

public static class PhraseCache
{

    public static PhraseTree<SamplePhraseBase> Tree { get; private set; } = new([]);

    public static void Initialize(DirectoryInfo configDirectory)
    {
        var directory = configDirectory.CreateSubdirectory("Phrases");
        // TODO: different languages
        var phrases = new List<SamplePhraseBase>();
        foreach (var file in Directory.EnumerateFiles(directory.FullName, "*.wav"))
        {
            using var stream = CreateAudioReader.Stream(file);
            phrases.Add(new RawSourcePhrase(stream.ReadPlayerCompatibleSamples(), Path.GetFileNameWithoutExtension(file)));
        }

        Tree = new PhraseTree<SamplePhraseBase>(phrases);
    }

}
