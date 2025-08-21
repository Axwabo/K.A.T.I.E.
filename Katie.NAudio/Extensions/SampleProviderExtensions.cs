using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.NAudio.Extensions;

public static class SampleProviderExtensions
{

    public static ISampleProvider EnsureFormat(this ISampleProvider provider, int sampleRate, int channels)
        => (provider.WaveFormat.Channels, channels) switch
        {
            (1, 2) => new MonoToStereoSampleProvider(provider.EnsureSampleRate(sampleRate)),
            (2, 1) => new StereoToMonoSampleProvider(provider).EnsureSampleRate(sampleRate),
            _ => provider.EnsureSampleRate(sampleRate)
        };

    public static ISampleProvider EnsureSampleRate(this ISampleProvider provider, int sampleRate)
        => provider.WaveFormat.SampleRate == sampleRate
            ? provider
            : new WdlResamplingSampleProvider(provider, sampleRate);

    public static TimeSpan SamplesToTime(this ISampleProvider provider, double samples)
        => TimeSpan.FromSeconds(samples / provider.WaveFormat.SampleRate / provider.WaveFormat.Channels);

}
