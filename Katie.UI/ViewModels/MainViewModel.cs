using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.NAudio.Extensions;
using Katie.UI.PhraseProviders;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    private static readonly Signal DefaultSignal = new(new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0), "None", TimeSpan.Zero);

    private int _playIndex;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Opacity))]
    private bool _initialsLoaded;

    public double Opacity => InitialsLoaded ? 1 : 0.5;

    [ObservableProperty]
    private string _text = "";

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

    private readonly FilePickerSignalProvider? _signalProvider;

    public MainViewModel(Control? host)
    {
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
        if (TopLevel.GetTopLevel(host) is {StorageProvider: var storage})
            _signalProvider = new FilePickerSignalProvider(storage);
    }

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
    public Task AddSignals() => LoadSignals(_signalProvider);

    [RelayCommand]
    public async Task Play(string language)
    {
        var index = ++_playIndex;
        // TODO: replace with a SoundFlow backend
        var originalText = Text;
        var provider = UtteranceChain.Parse(originalText, language == "English" ? _englishTree : _hungarianTree, language);
        if (provider == null)
            return;
        using var device = new WasapiOut();
        var (signalProvider, signalName, signalDuration) = SelectedSignal;
        signalProvider.Position = 0;
        device.Init(signalProvider.EnsureFormat(provider.WaveFormat).FollowedBy(provider), true);
        device.Play();
        var bytesPerSecond = (double) device.OutputWaveFormat.AverageBytesPerSecond;
        var totalTime = signalDuration + provider.TotalTime;
        while (device.PlaybackState == PlaybackState.Playing)
        {
            if (index != _playIndex)
                device.Stop();
            var currentTime = TimeSpan.FromSeconds(device.GetPosition() / bytesPerSecond);
            Dispatcher.UIThread.Post(() =>
            {
                CurrentPhrase = currentTime < signalDuration ? signalName : provider.Current.Text;
                Progress = currentTime / totalTime;
                var currentIndex = provider.Current.Index;
                if (currentTime < signalDuration || currentIndex >= originalText.Length - 1)
                    return;
                Span<char> textSpan = stackalloc char[originalText.Length + 1];
                var i = currentIndex == -1 ? ^1 : currentIndex;
                originalText.AsSpan()[..i].CopyTo(textSpan);
                textSpan[i] = '█';
                if (currentIndex != -1)
                    originalText.AsSpan()[currentIndex..].CopyTo(textSpan[(currentIndex + 1)..]);
                Text = textSpan.ToString();
            });
            await Task.Delay(10);
        }
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
