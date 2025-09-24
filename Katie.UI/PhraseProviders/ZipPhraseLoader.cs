using System.IO.Compression;
using Path = System.IO.Path;

namespace Katie.UI.PhraseProviders;

internal sealed class ZipPhraseLoader : IInitialPhraseLoader
{

    private readonly ZipArchive _zipArchive;
    private readonly IStreamToPhraseConverter _converter;

    public ZipPhraseLoader(ZipArchive zipArchive, IStreamToPhraseConverter converter)
    {
        _zipArchive = zipArchive;
        _converter = converter;
    }

    public async Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global)
    {
        foreach (var entry in _zipArchive.Entries)
        {
            var span = entry.FullName.AsSpan();
            var slash = span.IndexOfAny('/', '\\');
            if (slash == -1 || !Path.GetExtension(span).Equals(".wav", StringComparison.OrdinalIgnoreCase))
                continue;
            var directory = span[..slash];
            var target =
                directory.Equals("Hungarian", StringComparison.OrdinalIgnoreCase)
                    ? hungarian
                    : directory.Equals("English", StringComparison.OrdinalIgnoreCase)
                        ? english
                        : directory.Equals("Global", StringComparison.OrdinalIgnoreCase)
                            ? global
                            : null;
            if (target == null)
                continue;
            await using var stream = entry.Open();
            target.Add(await _converter.ToPhraseAsync(stream, Path.GetFileNameWithoutExtension(entry.FullName)));
        }
    }

}
