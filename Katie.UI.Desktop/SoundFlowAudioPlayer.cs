using NAudio.Wave;
using SoundFlow.Abstracts;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Structs;
using PlaybackState = SoundFlow.Enums.PlaybackState;

namespace Katie.UI.Desktop;

public sealed class SoundFlowAudioPlayer : IAudioPlayer
{

    private readonly AudioEngine _engine;
    private readonly AudioPlaybackDevice _device;
    private readonly SoundPlayer _player;

    public bool IsPlaying => _player.State == PlaybackState.Playing;

    public TimeSpan CurrentTime => TimeSpan.FromSeconds(_player.Time);

    public SoundFlowAudioPlayer(ISampleProvider provider)
    {
        var format = new AudioFormat
        {
            SampleRate = provider.WaveFormat.SampleRate,
            Channels = provider.WaveFormat.Channels,
            Format = SampleFormat.F32
        };
        _engine = new MiniAudioEngine();
        _device = _engine.InitializePlaybackDevice(null, format);
        _player = new SoundPlayer(_engine, format, new NAudioDataProvider(provider));
        _device.MasterMixer.AddComponent(_player);
    }

    public void Play()
    {
        _device.Start();
        _player.Play();
    }

    public void Stop()
    {
        _player.Stop();
        _device.Stop();
    }

    public void Dispose()
    {
        _engine.Dispose();
        _device.Dispose();
        _player.Dispose();
    }

}
