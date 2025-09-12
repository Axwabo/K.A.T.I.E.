using System.ComponentModel;
using System.Text;
using System.Threading;
using Avalonia.Media;
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
    private string _split = "Parsed text will show up here";

    [ObservableProperty]
    private IImmutableBrush _brush = Brushes.White;

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
        UtteranceChain? chain;
        try
        {
            chain = UtteranceChain.Parse(Text, language == "English" ? Phrases.EnglishTree : Phrases.HungarianTree, language);
        }
        catch (Exception e)
        {
            Split = e.Message;
            Brush = Brushes.Red;
            return;
        }

        Brush = Brushes.White;
        if (chain == null)
            return;
        SetSplit(chain);
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

    private void SetSplit(UtteranceChain provider)
    {
        var span = Text.AsSpan();
        var builder = new StringBuilder(span.Length + 20);
        var previous = Math.Max(0, provider.Current.Segment.EndIndex);
        builder.Append(span[..previous]);
        builder.Append('█');
        foreach (var segment in provider.Remaining)
        {
            var current = segment.EndIndex == -1 ? span.Length : segment.EndIndex;
            builder.Append(span[previous..current]);
            builder.Append('█');
            previous = current;
        }

        Split = builder.ToString();
    }

    [RelayCommand]
    public void Stop() => _playIndex = -1;

    [RelayCommand]
    public Task Cache() => Task.WhenAll(
        Phrases.Hungarian.CacheAll(),
        Phrases.English.CacheAll(),
        Phrases.Global.CacheAll()
    );

}
