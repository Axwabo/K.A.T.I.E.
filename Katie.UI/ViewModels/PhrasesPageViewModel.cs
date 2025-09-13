using System.ComponentModel;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.NAudio.Extensions;
using Katie.UI.Audio;
using Katie.UI.PhraseProviders;
using Katie.UI.Services;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasesPageViewModel : ViewModelBase
{

    private int _playIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NavigationBlocked), nameof(Opacity))]
    private string? _blockingOperation;

    public bool NavigationBlocked => BlockingOperation != null;

    public double Opacity => NavigationBlocked ? 0.5 : 1;

    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _currentPhrase = "";

    [ObservableProperty]
    private double _progress;

    public PhraseManager Phrases { get; }

    public SignalsViewModel Signals { get; }

    private readonly IAudioPlayerFactory? _factory;

    private CancellationTokenSource? _cts = new();

    [ObservableProperty]
    private bool _canCancel;

    public PhrasesPageViewModel(
        SignalsViewModel signals,
        PhraseManager phrases,
        IAudioPlayerFactory? audioPlayerFactory,
        IInitialPhraseLoader? initialPhrases = null
    )
    {
        _factory = audioPlayerFactory;
        Phrases = phrases;
        Signals = signals;
        if (!Design.IsDesignMode)
            LoadInitialPhrases(initialPhrases).ConfigureAwait(false);
    }

    public PhrasesPageViewModel() : this(new SignalsViewModel(), new PhraseManager(), null)
    {
        var np = new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0);
        Phrases.English.Add(new RawSourceSamplePhrase(np, "welcome"));
        Phrases.Hungarian.Add(new RawSourceSamplePhrase(np, "üdvözöljük"));
        Phrases.Global.Add(new RawSourceSamplePhrase(np, "Budapest"));
    }

    private void SetBlockingOperation(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(PhrasePackViewModel.BlockingOperation))
            return;
        var hadOperation = NavigationBlocked;
        BlockingOperation = Phrases.Hungarian.BlockingOperation ?? Phrases.English.BlockingOperation ?? Phrases.Global.BlockingOperation;
        if (hadOperation || !NavigationBlocked)
            return;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        EnableCancellation();
    }

    private void EnableCancellation()
    {
        CanCancel = true;
        Phrases.Hungarian.Cancellation = Phrases.English.Cancellation = Phrases.Global.Cancellation = _cts?.Token ?? CancellationToken.None;
    }

    private void SubscribeToOperations()
    {
        Phrases.Hungarian.PropertyChanged += SetBlockingOperation;
        Phrases.English.PropertyChanged += SetBlockingOperation;
        Phrases.Global.PropertyChanged += SetBlockingOperation;
    }

    [RelayCommand]
    private void Cancel()
    {
        CanCancel = false;
        _cts?.Cancel();
    }

    private async Task LoadInitialPhrases(IInitialPhraseLoader? initialPhrases)
    {
        BlockingOperation = "Loading phrases...";
        EnableCancellation();
        if (initialPhrases == null)
        {
            SubscribeToOperations();
            BlockingOperation = null;
            return;
        }

        await initialPhrases.LoadPhrasesAsync(Phrases.Hungarian, Phrases.English, Phrases.Global);
        SubscribeToOperations();
        Dispatcher.UIThread.Post(() => BlockingOperation = null);
    }

    [RelayCommand]
    public async Task Play(string language)
    {
        if (_factory == null)
            return;
        var index = ++_playIndex;
        var chain = UtteranceChain.Parse(Text, language == "English" ? Phrases.EnglishTree : Phrases.HungarianTree, language);
        if (chain == null)
            return;
        Progress = 0;

        var (signalProvider, signalName, signalDuration) = Signals.Selected;
        ISampleProvider master;
        if (Signals.Selected == SignalManager.DefaultSignal)
            master = chain;
        else
        {
            signalProvider.Position = 0;
            master = signalProvider.ToSampleProvider().EnsureFormat(chain.WaveFormat).FollowedBy(chain);
        }

        using var player = _factory.CreatePlayer(master);
        await player.Play();
        var totalTime = signalDuration + chain.TotalTime;
        while (true)
        {
            var currentTime = player.CurrentTime;
            Dispatcher.UIThread.Post(() =>
            {
                CurrentPhrase = currentTime < signalDuration ? signalName : chain.Current.Segment.Phrase?.Text ?? "";
                Progress = currentTime / totalTime;
            });
            if (index != _playIndex || !player.IsPlaying)
                break;
            await Task.Delay(10);
        }

        await player.Stop();
    }

    [RelayCommand]
    public void Stop() => _playIndex = -1;

}
