using NAudio.Wave;

namespace Katie.UI;

public interface IAudioPlayer : IDisposable
{

    bool IsPlaying { get; }

    TimeSpan CurrentTime { get; }

    void Play();

    void Stop();

    public static Func<ISampleProvider, IAudioPlayer>? Factory { get; set; }

}
