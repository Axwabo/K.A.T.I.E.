using System.IO;

namespace Katie.UI.Extensions;

public static class StreamExtensions
{

    public static async Task<MemoryStream> CopyToMemory(this Stream stream)
    {
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        memory.Position = 0;
        return memory;
    }

}
