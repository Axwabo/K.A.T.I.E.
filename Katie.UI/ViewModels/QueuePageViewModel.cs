using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Audio;
using Katie.UI.Services;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class QueuePageViewModel : ViewModelBase
{

    public PhrasesPageViewModel PhrasesPage { get; }

    public ObservableCollection<QueuedAnnouncement> Queue { get; } = [];

    private readonly IAudioPlayerFactory? _factory;

    private QueueSampleProvider? _provider;

    [ObservableProperty]
    private string _input = "";

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
        var chain = UtteranceChain.Parse(Input, PhrasesPage.Phrases[language], format);
        if (chain == null)
            return;
        var startPlayback = _provider == null;
        var provider = PhrasesPage.PrependSignal(chain, out var signalName, out _);
        var announcement = new QueuedAnnouncement(Input, language, provider, PhrasesPage.Signals.Selected == SignalManager.DefaultSignal ? null : signalName);
        _provider ??= new QueueSampleProvider(Queue, announcement);
        Queue.Add(announcement);
        if (startPlayback)
            _ = Play(_provider!).ConfigureAwait(false);
    }

    private async Task Play(ISampleProvider provider)
    {
        using var player = _factory!.CreatePlayer(provider);
        await player.Play();
        while (player.IsPlaying)
            await Task.Delay(10);
        await player.Stop();
    }

}
