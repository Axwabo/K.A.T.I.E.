using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Katie.UI.Browser.JSInterop;

internal static partial class SignalCacheFunctions
{

    public const string Module = "signalCache";

    [JSImport("save", Module)]
    public static partial Task Save(string name, [JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> data);

    [JSImport("prepare", Module)]
    public static partial Task PrepareCache();

    [JSImport("keys", Module)]
    public static partial string[] GetKeys();

    [JSImport("load", Module)]
    public static partial byte[] Load(string key);

    [JSImport("clearMemory", Module)]
    public static partial void ClearMemory();

    [JSImport("clear", Module)]
    public static partial Task DeleteAll();

}
