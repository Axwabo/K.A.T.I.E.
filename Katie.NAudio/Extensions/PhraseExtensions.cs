using Katie.NAudio.Phrases;

namespace Katie.NAudio.Extensions;

public static class PhraseExtensions
{

    public static bool IsAliasOf(this WavePhraseBase phrase, WavePhraseBase other)
        => phrase is WavePhraseAlias {Original: var original} && original == other;

}
