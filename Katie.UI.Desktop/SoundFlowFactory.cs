using Katie.UI.Audio;
using NAudio.Wave;

namespace Katie.UI.Desktop;

public sealed class SoundFlowFactory : IAudioPlayerFactory
{

    public IAudioPlayer CreatePlayer(ISampleProvider provider) => new SoundFlowAudioPlayer(provider);

}
