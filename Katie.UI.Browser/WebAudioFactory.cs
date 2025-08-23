using Katie.UI.Audio;
using NAudio.Wave;

namespace Katie.UI.Browser;

public sealed class WebAudioFactory : IAudioPlayerFactory
{

    public IAudioPlayer CreatePlayer(ISampleProvider provider)
    {
        WebAudioPlayer.Provider = provider;
        return WebAudioPlayer.Instance;
    }

}
