using NAudio.Wave;

namespace Katie.UI.Extensions;

public static class SampleProviderExtensions
{

    public static RawSourceSampleProvider ReadSamples(this ISampleProvider provider, TimeSpan duration)
    {
        var total = 0;
        var buffer = new float[(int) (duration.TotalSeconds * provider.WaveFormat.AverageBytesPerSecond / 4)];
        int read;
        while ((read = provider.Read(buffer, total, Math.Min(4800, buffer.Length - total))) != 0)
            total += read;
        return new RawSourceSampleProvider(provider.WaveFormat, buffer, total);
    }

}
