using System.ComponentModel;
using System.Text;
using System.Threading;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Extensions;
using Katie.UI.Audio;
using Katie.UI.PhraseProviders;
using Microsoft.Extensions.DependencyInjection;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    private int _playIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBlockingOperation), nameof(Opacity))]
    private string? _blockingOperation = null; //"Loading phrases...";

    public bool HasBlockingOperation => BlockingOperation != null;

    public double Opacity => HasBlockingOperation ? 0.5 : 1;

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

    private PhraseTree<SamplePhraseBase> _englishTree = new([]);

    private PhraseTree<SamplePhraseBase> _hungarianTree = new([]);

    public PhrasePackViewModel Hungarian { get; }

    public PhrasePackViewModel English { get; }

    public PhrasePackViewModel Global { get; }

    public SignalsViewModel Signals { get; }

    private readonly IAudioPlayerFactory? _factory;

    private CancellationTokenSource? _cts = new();

    [ObservableProperty]
    private bool _canCancel;

    public MainViewModel(
        SignalsViewModel signals,
        IAudioPlayerFactory? audioPlayerFactory,
        IInitialPhraseLoader? initialPhrases = null,
        IPhraseCacheManager? cacheSaver = null,
        [FromKeyedServices(nameof(FilePickerPhraseProvider))]
        IPhraseProvider? phrasePicker = null
    )
    {
        _factory = audioPlayerFactory;
        Signals = signals;
        English = new PhrasePackViewModel {PhraseProvider = phrasePicker, Language = "English", Cache = cacheSaver};
        Hungarian = new PhrasePackViewModel {PhraseProvider = phrasePicker, Language = "Hungarian", Cache = cacheSaver};
        Global = new PhrasePackViewModel {PhraseProvider = phrasePicker, Language = "Global", Cache = cacheSaver};
        Hungarian.PhrasesChanged += RebuildHungarian;
        English.PhrasesChanged += RebuildEnglish;
        Global.PhrasesChanged += RebuildHungarian;
        Global.PhrasesChanged += RebuildEnglish;
        if (!Design.IsDesignMode)
            LoadInitialPhrases(initialPhrases).ConfigureAwait(false);
    }

    public MainViewModel() : this(new SignalsViewModel(), null)
    {
    }

    private void RebuildHungarian() => _hungarianTree = new PhraseTree<SamplePhraseBase>(Global.List.Concat(Hungarian.List));

    private void RebuildEnglish() => _englishTree = new PhraseTree<SamplePhraseBase>(Global.List.Concat(English.List));

    private void SetBlockingOperation(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is not nameof(PhrasePackViewModel.BlockingOperation))
            return;
        var hadOperation = HasBlockingOperation;
        BlockingOperation = Hungarian.BlockingOperation ?? English.BlockingOperation ?? Global.BlockingOperation;
        if (hadOperation || !HasBlockingOperation)
            return;
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        EnableCancellation();
    }

    private void EnableCancellation()
    {
        CanCancel = true;
        Hungarian.Cancellation = English.Cancellation = Global.Cancellation = _cts?.Token ?? CancellationToken.None;
    }

    private void SubscribeToOperations()
    {
        Hungarian.PropertyChanged += SetBlockingOperation;
        English.PropertyChanged += SetBlockingOperation;
        Global.PropertyChanged += SetBlockingOperation;
    }

    [RelayCommand]
    private void Cancel()
    {
        CanCancel = false;
        _cts?.Cancel();
    }

    private async Task LoadInitialPhrases(IInitialPhraseLoader? initialPhrases)
    {
        EnableCancellation();
        if (initialPhrases == null)
        {
            SubscribeToOperations();
            BlockingOperation = null;
            return;
        }

        await initialPhrases.LoadPhrasesAsync(Hungarian, English, Global);
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
            chain = UtteranceChain.Parse(Text, language == "English" ? _englishTree : _hungarianTree, language);
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
        if (Signals.Selected == SignalsViewModel.DefaultSignal)
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
        Hungarian.CacheAll(),
        English.CacheAll(),
        Global.CacheAll()
    );

}
