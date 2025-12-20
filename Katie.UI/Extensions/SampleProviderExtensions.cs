using Katie.UI.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.UI.Extensions;

public static class SampleProviderExtensions
{

    extension(ISampleProvider provider)
    {

        public RawSourceSampleProvider ReadSamples(TimeSpan duration)
        {
            var total = 0;
            var buffer = new float[(int) (duration.TotalSeconds * provider.WaveFormat.AverageBytesPerSecond / 4)];
            int read;
            while ((read = provider.Read(buffer, total, Math.Min(4800, buffer.Length - total))) != 0)
                total += read;
            return new RawSourceSampleProvider(provider.WaveFormat, buffer, total);
        }

        public OffsetSampleProvider LeadOut(TimeSpan duration) => new(provider) {LeadOut = duration};

    }

}
