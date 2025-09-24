using System.IO;
using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

public sealed class FileStreamToPhraseConverter : IStreamToPhraseConverter
{

    public Task<WaveStreamPhrase> ToPhraseAsync(Stream stream, string name)
        => Task.FromResult(new WaveStreamPhrase(stream, name));

}
