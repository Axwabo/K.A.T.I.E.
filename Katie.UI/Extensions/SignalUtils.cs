using System.IO;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.Extensions;

public static class SignalUtils
{

    public static bool IsBrowser { get; set; }

    public static async Task<Signal> ReadSignalAsync(Stream stream, string path)
    {
        var memory = await stream.ToMemoryStream();
        var reader = new WaveFileReader(memory);
        return new Signal(reader, Path.GetFileNameWithoutExtension(path), reader.TotalTime);
    }

}
