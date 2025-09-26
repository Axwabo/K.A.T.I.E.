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

    private QueueSampleProvider? _provider;

    [ObservableProperty]
    private string _input = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlayPauseText))]
    private bool _playing;

    [ObservableProperty]
    private bool _canTogglePlayback;

    public string PlayPauseText => Playing ? "Pause" : "Resume";

    public QueuedAnnouncement? Current => _provider?.Current;

    public QueuePageViewModel(PhrasesPageViewModel phrasesPage, IAudioPlayerFactory? factory)
    {
        PhrasesPage = phrasesPage;
        _factory = factory;
    }

    public QueuePageViewModel() : this(new PhrasesPageViewModel(), null)
    {
    }

    [RelayCommand]
    private void Enqueue(string language) => Enqueue(Input, language, PhrasesPage.Signals.Selected);

    private void Enqueue(string input, string language, Signal signal)
    {
        if (_factory == null)
            return;
        var format = _provider == null ? (SimpleWaveFormat?) null : (SimpleWaveFormat) _provider.WaveFormat;
        var segments = UtteranceChain.ParseToQueue(input, PhrasesPage.Phrases[language], ref format);
        if (segments.Count == 0)
            return;
        var startPlayback = _provider == null;
        var announcement = new QueuedAnnouncement(input, language, segments, format.Value, signal);
        _provider ??= new QueueSampleProvider(Queue, announcement);
        Queue.Add(announcement);
        if (startPlayback)
            _ = Play().ConfigureAwait(false);
    }

    private async Task Play()
    {
        using var player = _factory!.CreatePlayer(_provider!);
        await player.Play();
        Playing = true;
        CanTogglePlayback = true;
        while (player.IsPlaying && Playing)
            await Task.Delay(10);
        Playing = false;
        Dispatcher.UIThread.Post(() => CanTogglePlayback = _provider != null);
        await player.Stop();
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (Playing)
            Playing = false;
        else
            _ = Play().ConfigureAwait(false);
    }

    [RelayCommand]
    private void Clear()
    {
        Playing = false;
        _provider = null;
        Queue.Clear();
    }

    [RelayCommand]
    private void RestartAll()
    {
        var announcements = Queue.Select(e => (e.Text, e.Language, e.Signal)).ToArray();
        Clear();
        foreach (var (text, language, signal) in announcements)
            Enqueue(text, language, signal);
    }

}
