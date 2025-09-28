using System.Diagnostics.CodeAnalysis;
using System.IO;
using Katie.Core.DataStructures;
using Katie.Core.Extensions;
using Katie.NAudio.Phrases;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;
using Logger = LabApi.Features.Console.Logger;

namespace Katie.SecretLab;

public static class PhraseCache
{

    public static PhraseTree<WavePhraseBase> Hungarian { get; } = new(nameof(Hungarian));

    public static PhraseTree<WavePhraseBase> English { get; } = new(nameof(English));

    private static readonly Dictionary<int, RawSourceSampleProvider> Signals = [];

    public static void Initialize(DirectoryInfo root)
    {
        InitializeSignals(root.CreateSubdirectory(nameof(Signals)));
        InitializePhrases(root.CreateSubdirectory("Phrases"));
    }

    private static void InitializeSignals(DirectoryInfo directory)
    {
        var count = 0;
        foreach (var file in directory.EnumerateFiles("*.wav"))
        {
            var name = Path.GetFileNameWithoutExtension(file.FullName);
            Logger.Debug($"Found signal: {name}");
            using var stream = CreateAudioReader.Stream(file.FullName);
            var provider = stream.ReadPlayerCompatibleSamples();
            provider.ClipName = name;
            Signals[name.AsSpan().LowercaseHashCode()] = provider;
            count++;
        }

        Logger.Info($"Loaded {count} signal(s)");
    }

    private static void InitializePhrases(DirectoryInfo phrases)
    {
        var global = phrases.EnumeratePhrases("Global").ToArray();
        Hungarian.Rebuild(global.Concat(phrases.EnumeratePhrases(nameof(Hungarian))));
        English.Rebuild(global.Concat(phrases.EnumeratePhrases(nameof(English))));
    }

    public static bool TryGetTree(ReadOnlySpan<char> language, [NotNullWhen(true)] out PhraseTree<WavePhraseBase>? tree)
    {
        tree = language.Equals(nameof(Hungarian), StringComparison.OrdinalIgnoreCase)
            ? Hungarian
            : language.Equals(nameof(English), StringComparison.OrdinalIgnoreCase)
                ? English
                : null;
        return tree != null;
    }

    public static bool TryGetSignal(ReadOnlySpan<char> name, [NotNullWhen(true)] out RawSourceSampleProvider? provider)
        => Signals.TryGetValue(name.LowercaseHashCode(), out provider);

    private static IEnumerable<WavePhraseBase> EnumeratePhrases(this DirectoryInfo directory, string subdirectory)
    {
        var count = 0;
        var fullPath = directory.CreateSubdirectory(subdirectory).FullName;
        var aliases = Path.Combine(fullPath, "aliases.txt");
        var useAliases = File.Exists(aliases);
        var phrases = new Dictionary<int, WavePhraseBase>();
        foreach (var file in Directory.EnumerateFiles(fullPath, "*.wav"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            Logger.Debug($"Found {subdirectory} phrase: {name}");
            using var stream = CreateAudioReader.Stream(file);
            var samples = stream.ReadPlayerCompatibleSamples();
            var phrase = new RawSourcePhrase(samples, name);
            count++;
            if (useAliases)
                phrases[name.AsSpan().LowercaseHashCode()] = phrase;
            yield return phrase;
        }

        Logger.Info($"Loaded {count} {subdirectory} phrase(s)");
        if (!useAliases)
            yield break;
        count = 0;
        foreach (var line in File.ReadLines(aliases))
        {
            var span = line.AsSpan();
            var equals = span.IndexOf('=');
            if (equals == -1 || !phrases.TryGetValue(span[(equals + 1)..].Trim().LowercaseHashCode(), out var phrase))
                continue;
            var name = span[..equals].Trim();
            var alias = WavePhraseAlias.Create(phrase, name.ToString());
            phrases[name.LowercaseHashCode()] = alias;
            count++;
            yield return alias;
        }

        Logger.Info($"Assigned {count} {subdirectory} alias(es)");
    }

}
