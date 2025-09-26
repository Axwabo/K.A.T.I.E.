using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Audio;
using Katie.UI.Signals;

namespace Katie.UI.ViewModels;

public sealed partial class QueuePageViewModel : ViewModelBase
{

    private CancellationTokenSource? _cts;

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
    private bool _playing;

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
        if (!Playing)
            Play();
    }

    private void Play()
    {
        StopPlayback();
        _ = PlayCore(_cts.Token).ConfigureAwait(false);
        return;

        async Task PlayCore(CancellationToken token)
        {
            using var player = _factory!.CreatePlayer(Provider!);
            await player.Play();
            Dispatcher.UIThread.Post(() => Playing = true);
            try
            {
                while (player.IsPlaying && !token.IsCancellationRequested)
                    await Task.Delay(10, token);
            }
            finally
            {
                Dispatcher.UIThread.Post(() => Playing = false);
                await player.Stop();
            }
        }
    }

    [MemberNotNull(nameof(_cts))]
    private void StopPlayback()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
    }

    [RelayCommand]
    private void TogglePause()
    {
        if (Playing)
            StopPlayback();
        else
            Play();
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
        StopPlayback();
        Provider = null;
    }

}
