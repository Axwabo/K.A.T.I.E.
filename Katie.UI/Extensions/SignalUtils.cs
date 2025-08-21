using System.IO;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.Extensions;

public static class SignalUtils
{

    public static async Task<Signal> ReadSignalAsync(Stream stream, string path)
    {
        await using var reader = new WaveFileReader(stream);
        var provider = await Task.Run(() => reader.ToSampleProvider().ReadSamples(reader.TotalTime));
        return new Signal(provider, Path.GetFileNameWithoutExtension(path), reader.TotalTime);
    }

}
