namespace Katie.UI.Audio;

public interface IAudioPlayer : IDisposable
{

    bool IsPlaying { get; }

    TimeSpan CurrentTime { get; }

    Task Play();

    Task Stop();

}
