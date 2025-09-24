using System.IO;

namespace Katie.UI.PhraseProviders;

public interface IStreamToPhraseConverter
{

    Task<WaveStreamPhrase> ToPhraseAsync(Stream stream, string name);

}
