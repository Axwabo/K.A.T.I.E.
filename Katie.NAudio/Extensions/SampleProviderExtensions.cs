using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.NAudio.Extensions;

public static class SampleProviderExtensions
{

    public static ISampleProvider EnsureFormat(this ISampleProvider provider, SimpleWaveFormat format)
        => (provider.WaveFormat.Channels, format.Channels) switch
        {
            (1, 2) => new MonoToStereoSampleProvider(provider.EnsureSampleRate(format.SampleRate)),
            (2, 1) => new StereoToMonoSampleProvider(provider).EnsureSampleRate(format.SampleRate),
            _ => provider.EnsureSampleRate(format.SampleRate)
        };

    public static ISampleProvider EnsureSampleRate(this ISampleProvider provider, int sampleRate)
        => provider.WaveFormat.SampleRate == sampleRate
            ? provider
            : new WdlResamplingSampleProvider(provider, sampleRate);

    public static TimeSpan SamplesToTime(this ISampleProvider provider, double samples)
        => TimeSpan.FromSeconds(samples / provider.WaveFormat.SampleRate / provider.WaveFormat.Channels);

}
