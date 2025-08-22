using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Katie.UI.Browser;

internal static partial class WebAudioFunctions
{

    public const string Module = "audio";

    [JSImport("play", Module)]
    public static partial Task Play();

    [JSImport("stop", Module)]
    public static partial Task Stop();

}
