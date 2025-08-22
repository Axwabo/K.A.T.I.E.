using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Katie.UI.Browser;

internal static partial class WebAudioFunctions
{

    private const int SamplesPerFrame = 128;

    private static readonly float[] FloatBuffer = new float[SamplesPerFrame];
    private static readonly double[] DoubleBuffer = new double[SamplesPerFrame];

    public const string Module = "audio";

    [JSImport("play", Module)]
    public static partial Task Play();

    [JSImport("stop", Module)]
    public static partial Task Stop();

    [JSExport]
    [return: JSMarshalAs<JSType.MemoryView>]
    public static ArraySegment<double> ReadFromProvider()
    {
        if (WebAudioPlayer.Provider == null)
            return ArraySegment<double>.Empty;
        var read = WebAudioPlayer.Provider.Read(FloatBuffer, 0, FloatBuffer.Length);
        if (read == 0)
        {
            WebAudioPlayer.Instance.IsPlaying = false;
            return ArraySegment<double>.Empty;
        }

        for (var i = 0; i < read; i++)
            DoubleBuffer[i] = FloatBuffer[i];
        return new ArraySegment<double>(DoubleBuffer, 0, read);
    }

}
