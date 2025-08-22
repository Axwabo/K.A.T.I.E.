using System.Threading.Tasks;
using NAudio.Wave;

namespace Katie.UI.Browser;

public sealed class WebAudioPlayer : IAudioPlayer
{

    public static ISampleProvider? Provider { get; set; }

    public static WebAudioPlayer Instance { get; } = new();

    public bool IsPlaying { get; set; }

    public TimeSpan CurrentTime { get; private set; }

    public async Task Play()
    {
        await WebAudioFunctions.Play();
        IsPlaying = true;
    }

    public async Task Stop()
    {
        await WebAudioFunctions.Stop();
        IsPlaying = false;
    }

    public void Dispose()
    {
        CurrentTime = TimeSpan.Zero;
        IsPlaying = false;
    }

}
