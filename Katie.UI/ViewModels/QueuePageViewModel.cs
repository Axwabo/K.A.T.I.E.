using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Audio;

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
    private QueuedAnnouncement? _current;

    public QueuePageViewModel(PhrasesPageViewModel phrasesPage, IAudioPlayerFactory? factory)
    {
        PhrasesPage = phrasesPage;
        _factory = factory;
    }

    public QueuePageViewModel() : this(new PhrasesPageViewModel(), null)
    {
    }

    [RelayCommand]
    private void Enqueue(string language)
    {
        if (_factory == null)
            return;
        var format = _provider == null ? (SimpleWaveFormat?) null : (SimpleWaveFormat) _provider.WaveFormat;
        var segments = UtteranceChain.ParseToQueue(Input, PhrasesPage.Phrases[language], ref format);
        if (segments.Count == 0)
            return;
        var startPlayback = _provider == null;
        var announcement = new QueuedAnnouncement(Input, language, segments, format.Value, PhrasesPage.Signals.Selected);
        if (_provider == null)
        {
            _provider = new QueueSampleProvider(Queue, announcement);
            _provider.PropertyChanged += ProviderOnPropertyChanged;
        }

        Queue.Add(announcement);
        if (startPlayback)
            _ = Play().ConfigureAwait(false);
    }

    private async Task Play()
    {
        using var player = _factory!.CreatePlayer(_provider!);
        await player.Play();
        while (player.IsPlaying)
            await Task.Delay(10);
        _provider!.PropertyChanged -= ProviderOnPropertyChanged;
        _provider = null;
        Current = null;
        await player.Stop();
    }

    private void ProviderOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(QueueSampleProvider.Current))
            Dispatcher.UIThread.Post(() => Current = _provider?.Current);
    }

}
