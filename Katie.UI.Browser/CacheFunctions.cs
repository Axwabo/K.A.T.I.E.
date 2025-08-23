using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Katie.UI.Browser;

internal static partial class CacheFunctions
{

    public const string Module = "cache";

    [JSImport("save", Module)]
    public static partial Task Save(string name, [JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> data);

    [JSImport("load", Module)]
    [return: JSMarshalAs<JSType.MemoryView>]
    private static partial ArraySegment<byte> Load(string name);

    public static async Task<MemoryStream> LoadMemoryStream(string name)
    {
        var bytes = await Task.Run(() => Load(name));
        var stream = new MemoryStream();
        await stream.WriteAsync(bytes.AsMemory());
        stream.Position = 0;
        return stream;
    }

}
