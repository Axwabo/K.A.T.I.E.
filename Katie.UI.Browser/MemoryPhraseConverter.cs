using System.IO;
using System.Threading.Tasks;
using Katie.NAudio.Phrases;
using Katie.UI.Extensions;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Browser;

public sealed class MemoryPhraseConverter : IStreamToPhraseConverter
{

    public async Task<WaveStreamPhrase> ToPhraseAsync(Stream stream, string name)
    {
        await using (stream)
        {
            var memory = await stream.CopyToMemory();
            return new MemoryStreamPhrase(memory, name);
        }
    }

}
