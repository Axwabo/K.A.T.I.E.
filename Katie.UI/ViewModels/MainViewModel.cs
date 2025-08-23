using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Extensions;
using Katie.UI.Audio;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    private static readonly Signal DefaultSignal = new(null!, "None", TimeSpan.Zero);

    private int _playIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Opacity))]
    private bool _initialsLoaded;

    public double Opacity => InitialsLoaded ? 1 : 0.5;

    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _split = "Parsed text will show up here";

    [ObservableProperty]
    private string _currentPhrase = "";

    [ObservableProperty]
    private double _progress;

    private PhraseTree<SamplePhraseBase> _englishTree = new([]);

    private PhraseTree<SamplePhraseBase> _hungarianTree = new([]);

    public PhrasePackViewModel Hungarian { get; }

    public PhrasePackViewModel English { get; }

    public PhrasePackViewModel Global { get; }

    public ObservableCollection<Signal> Signals { get; } = [DefaultSignal];

    [ObservableProperty]
    private Signal _selectedSignal = DefaultSignal;

    private readonly Control? _host;

    private FilePickerSignalProvider? _signalProvider;

    public MainViewModel(Control? host)
    {
        _host = host;
        English = new PhrasePackViewModel {Host = host, Language = "English"};
        Hungarian = new PhrasePackViewModel {Host = host, Language = "Hungarian"};
        Global = new PhrasePackViewModel {Host = host, Language = "Global"};
        Hungarian.PhrasesChanged += RebuildHungarian;
        English.PhrasesChanged += RebuildEnglish;
        Global.PhrasesChanged += RebuildHungarian;
        Global.PhrasesChanged += RebuildEnglish;
        if (Design.IsDesignMode)
            return;
        LoadInitialPhrases().ConfigureAwait(false);
        LoadSignals(ISignalProvider.InitialProvider).ConfigureAwait(false);
    }

    private IAudioPlayerFactory? _factory;

    public MainViewModel() : this(null)
    {
    }

    private void RebuildHungarian() => _hungarianTree = new PhraseTree<SamplePhraseBase>(Global.List.Concat(Hungarian.List));

    private void RebuildEnglish() => _englishTree = new PhraseTree<SamplePhraseBase>(Global.List.Concat(English.List));

    private async Task LoadInitialPhrases()
    {
        await IPhraseProvider.LoadInitialPhrases(Hungarian, English, Global);
        Dispatcher.UIThread.Post(() => InitialsLoaded = true);
    }

    private async Task LoadSignals(ISignalProvider? signalProvider)
    {
        if (signalProvider == null)
            return;
        var signals = new List<Signal>();
        await foreach (var provider in signalProvider.EnumerateSignalsAsync())
            signals.Add(provider);
        Dispatcher.UIThread.Post(() =>
        {
            foreach (var signal in signals)
                Signals.Add(signal);
        });
    }

    [RelayCommand]
    public Task AddSignals() => TopLevel.GetTopLevel(_host) is {StorageProvider: {CanOpen: true} storage}
        ? LoadSignals(_signalProvider ??= new FilePickerSignalProvider(storage))
        : Task.CompletedTask;

    [RelayCommand]
    public async Task Play(string language)
    {
        if (_factory == null)
            return;
        var index = ++_playIndex;
        var chain = UtteranceChain.Parse(Text, language == "English" ? _englishTree : _hungarianTree, language);
        if (chain == null)
            return;
        SetSplit(chain);
        Progress = 0;

        var (signalProvider, signalName, signalDuration) = SelectedSignal;
        ISampleProvider master;
        if (SelectedSignal == DefaultSignal)
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
        var previous = provider.Current.Segment.EndIndex;
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
        Hungarian.Cache(),
        English.Cache(),
        Global.Cache()
    );

}
