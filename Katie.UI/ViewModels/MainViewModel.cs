using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Katie.Core;
using Katie.Core.DataStructures;
using Katie.NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    [ObservableProperty]
    private string _text = "";

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
    }

    public MainViewModel() : this(null)
    {
    }

    private void RebuildHungarian() => _hungarianTree = new PhraseTree<WavePhrase>(Hungarian.List.Concat(Global.List));

    private void RebuildEnglish() => _englishTree = new PhraseTree<WavePhrase>(English.List.Concat(Global.List));

    [RelayCommand]
    public async Task Play(string language)
    {
        // TODO: replace with a SoundFlow backend
        using var device = new WasapiOut();
        var provider = new ConcatenatingSampleProvider(
            PhraseParser<WavePhrase>.Parse(Text, language == "English" ? _englishTree : _hungarianTree)
                .Select(e => e.ToSampleProvider())
        );
        device.Init(provider, true);
        device.Play();
        while (device.PlaybackState == PlaybackState.Playing)
            await Task.Delay(100);
    }

}
