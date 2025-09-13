using System.IO;
using Katie.UI.Extensions;
using NAudio.Wave;

namespace Katie.UI.Signals;

public sealed record Signal(WaveFileReader Provider, string Name, TimeSpan Duration)
{

    public MemoryStream? Data { get; init; }

    public static async Task<Signal> LoadIntoMemory(Stream source, string name)
    {
        if (source is WaveStream)
            throw new InvalidOperationException("Cannot create a Signal from a WaveStream");
        var memory = source as MemoryStream ?? await source.CopyToMemory();
        var reader = new WaveFileReader(memory);
        return new Signal(reader, name, reader.TotalTime) {Data = memory};
    }

}
