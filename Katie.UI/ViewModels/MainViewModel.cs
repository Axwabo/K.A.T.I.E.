using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Katie.Core.DataStructures;
using Katie.NAudio;
using Katie.UI.PhraseProviders;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    private int _playIndex;

    [ObservableProperty]
    private bool _initialsLoaded;

    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _currentPhrase = "";

    [ObservableProperty]
    private double _progress;

    private PhraseTree<WavePhrase> _englishTree = new([]);

    private PhraseTree<WavePhrase> _hungarianTree = new([]);

    public PhrasePackViewModel Hungarian { get; }

    public PhrasePackViewModel English { get; }

    public PhrasePackViewModel Global { get; }

    public MainViewModel(Control? host)
    {
        English = new PhrasePackViewModel {Host = host, Language = "English"};
        Hungarian = new PhrasePackViewModel {Host = host, Language = "Hungarian"};
        Global = new PhrasePackViewModel {Host = host, Language = "Global"};
        Hungarian.PhrasesChanged += RebuildHungarian;
        English.PhrasesChanged += RebuildEnglish;
        Global.PhrasesChanged += RebuildHungarian;
        Global.PhrasesChanged += RebuildEnglish;
        LoadInitialPhrases().ConfigureAwait(false);
    }

    public MainViewModel() : this(null)
    {
    }

    private void RebuildHungarian() => _hungarianTree = new PhraseTree<WavePhrase>(Global.List.Concat(Hungarian.List));

    private void RebuildEnglish() => _englishTree = new PhraseTree<WavePhrase>(Global.List.Concat(English.List));

    private async Task LoadInitialPhrases()
    {
        await IPhraseProvider.LoadInitialPhrases(Hungarian, English, Global);
        Dispatcher.UIThread.Post(() => InitialsLoaded = true);
    }

    [RelayCommand]
    public async Task Play(string language)
    {
        var index = ++_playIndex;
        // TODO: replace with a SoundFlow backend
        var provider = PhraseChain.Parse(Text, language == "English" ? _englishTree : _hungarianTree);
        if (provider == null)
            return;
        using var device = new WasapiOut();
        device.Init(provider, true);
        device.Play();
        var divisor = device.OutputWaveFormat.BitsPerSample / 8d * device.OutputWaveFormat.SampleRate * device.OutputWaveFormat.Channels;
        while (device.PlaybackState == PlaybackState.Playing)
        {
            if (index != _playIndex)
                device.Stop();
            var currentTime = TimeSpan.FromSeconds(device.GetPosition() / divisor);
            Dispatcher.UIThread.Post(() =>
            {
                CurrentPhrase = provider.Current.Text;
                Progress = currentTime / provider.TotalTime;
            });
            await Task.Delay(10);
        }
    }

    [RelayCommand]
    public void Stop() => _playIndex = -1;

}
