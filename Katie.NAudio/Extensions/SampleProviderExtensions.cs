using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.NAudio.Extensions;

public static class SampleProviderExtensions
{

    extension(ISampleProvider provider)
    {

        public ISampleProvider EnsureFormat(SimpleWaveFormat format)
            => (provider.WaveFormat.Channels, format.Channels) switch
            {
                (1, 2) => new MonoToStereoSampleProvider(provider.EnsureSampleRate(format.SampleRate)),
                (2, 1) => new StereoToMonoSampleProvider(provider).EnsureSampleRate(format.SampleRate),
                _ => provider.EnsureSampleRate(format.SampleRate)
            };

        public ISampleProvider EnsureSampleRate(int sampleRate)
            => provider.WaveFormat.SampleRate == sampleRate
                ? provider
                : new WdlResamplingSampleProvider(provider, sampleRate);

        public TimeSpan SamplesToTime(double samples)
            => TimeSpan.FromSeconds(samples / provider.WaveFormat.SampleRate / provider.WaveFormat.Channels);

    }

}
