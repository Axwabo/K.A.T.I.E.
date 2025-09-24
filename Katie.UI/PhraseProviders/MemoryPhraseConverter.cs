using System.IO;
using Katie.UI.Extensions;

namespace Katie.UI.PhraseProviders;

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
