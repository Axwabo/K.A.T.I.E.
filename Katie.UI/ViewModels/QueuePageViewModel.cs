using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Audio;
using Katie.UI.Extensions;
using Katie.UI.Services;

namespace Katie.UI.ViewModels;

public sealed partial class QueuePageViewModel : ViewModelBase
{

    private static readonly TimeSpan Delay = TimeSpan.FromSeconds(3);

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
        var chain = UtteranceChain.From(Input, PhrasesPage.Phrases[language], format);
        if (chain == null)
            return;
        var startPlayback = _provider == null;
        var provider = PhrasesPage.PrependSignal(chain, out _, out _);
        var announcement = new QueuedAnnouncement(Input, language, provider.LeadOut(Delay), PhrasesPage.Signals.Selected, PhrasesPage.Signals.Selected == SignalManager.DefaultSignal);
        _provider ??= new QueueSampleProvider(Queue, announcement);
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
        _provider = null;
        await player.Stop();
    }

}
