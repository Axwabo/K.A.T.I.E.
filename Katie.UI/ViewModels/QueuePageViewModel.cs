using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Audio;
using Katie.UI.Signals;

namespace Katie.UI.ViewModels;

public sealed partial class QueuePageViewModel : ViewModelBase
{

    public PhrasesPageViewModel PhrasesPage { get; }

    public ObservableCollection<QueuedAnnouncement> Queue { get; } = [];

    private readonly IAudioPlayerFactory? _factory;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasProvider), nameof(PlayPauseText))]
    private QueueSampleProvider? _provider;

    [ObservableProperty]
    private string _input = "";

    [ObservableProperty]
    private string _error = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlayPauseText))]
    private int _playIndex = -1;

    private bool Playing => PlayIndex != -1;

    public string PlayPauseText => Playing || !HasProvider ? "Pause" : "Resume";

    public bool HasProvider => Provider != null;

    public QueuedAnnouncement? Current => Provider?.Current;

    public QueuePageViewModel(PhrasesPageViewModel phrasesPage, IAudioPlayerFactory? factory)
    {
        PhrasesPage = phrasesPage;
        _factory = factory;
    }

    public QueuePageViewModel() : this(new PhrasesPageViewModel(), null)
    {
    }

    private void Enqueue(string input, string language, Signal signal)
    {
        if (_factory == null)
            return;
        Error = "";
        var format = Provider == null ? (SimpleWaveFormat?) null : (SimpleWaveFormat) Provider.WaveFormat;
        var segments = UtteranceChain.ParseToQueue(input, PhrasesPage.Phrases[language], ref format);
        if (segments.Count == 0)
            return;
        var announcement = new QueuedAnnouncement(input, language, segments, format.Value, signal);
        Provider ??= new QueueSampleProvider(Queue, announcement);
        Queue.Add(announcement);
        if (PlayIndex == -1)
            _ = Play().ConfigureAwait(false);
    }

    private async Task Play()
    {
        using var player = _factory!.CreatePlayer(Provider!);
        await player.Play();
        var index = PlayIndex = Random.Shared.Next();
        while (player.IsPlaying && PlayIndex == index)
            await Task.Delay(10);
        if (PlayIndex != -1 && PlayIndex != index)
            return;
        Dispatcher.UIThread.Post(() =>
        {
            PlayIndex = -1;
            if (Current is null)
                Provider = null;
        });
        await player.Stop();
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (Playing)
            PlayIndex = -1;
        else
            _ = Play().ConfigureAwait(false);
    }

    [RelayCommand]
    private void Enqueue(string language)
    {
        try
        {
            Enqueue(Input, language, PhrasesPage.Signals.Selected);
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Stop();
        Queue.Clear();
    }

    [RelayCommand]
    private void RestartAll()
    {
        var announcements = Queue.Select(e => (e.Text, e.Language, e.Signal)).ToArray();
        Clear();
        try
        {
            foreach (var (text, language, signal) in announcements)
                Enqueue(text, language, signal);
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
    }

    [RelayCommand]
    private void Skip() => Provider?.Next();

    [RelayCommand]
    private void Stop()
    {
        PlayIndex = -1;
        Provider = null;
    }

}
