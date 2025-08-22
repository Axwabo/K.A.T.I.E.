using NAudio.Wave;

namespace Katie.UI;

public interface IAudioPlayer : IDisposable
{

    bool IsPlaying { get; }

    TimeSpan CurrentTime { get; }

    Task Play();

    Task Stop();

    public static Func<ISampleProvider, IAudioPlayer>? Factory { get; set; }

}
