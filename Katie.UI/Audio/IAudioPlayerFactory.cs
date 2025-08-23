using NAudio.Wave;

namespace Katie.UI.Audio;

public interface IAudioPlayerFactory
{

    IAudioPlayer CreatePlayer(ISampleProvider provider);

}
