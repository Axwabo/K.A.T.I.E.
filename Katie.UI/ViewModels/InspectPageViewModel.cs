using System.Collections.ObjectModel;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.Input;
using Katie.Core;
using Katie.UI.Audio;
using Katie.UI.Services;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class InspectPageViewModel : ViewModelBase
{

    private readonly PhraseManager? _manager;

    [ObservableProperty]
    private string _input = "";

    [ObservableProperty]
    private string _error = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Orientation))]
    private bool _vertical;

    public Orientation Orientation => Vertical ? Orientation.Vertical : Orientation.Horizontal;

    public ObservableCollection<ParsedText> Parsed { get; } = [];

    public InspectPageViewModel(PhraseManager? manager) => _manager = manager;

    public InspectPageViewModel()
    {
        Parsed.Add(new ParsedText("Test", [new UtteranceSegment<WavePhraseBase>(TimeSpan.FromSeconds(0.5), new RawSourceSamplePhrase(new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0), "testing"))]));
        Parsed.Add(new ParsedText("1", [new UtteranceSegment<WavePhraseBase>(TimeSpan.FromSeconds(0.3), new RawSourceSamplePhrase(new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0), "one"))]));
        Parsed.Add(new ParsedText("200", [
            new UtteranceSegment<WavePhraseBase>(TimeSpan.FromSeconds(0.3), new RawSourceSamplePhrase(new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0), "two")),
            new UtteranceSegment<WavePhraseBase>(TimeSpan.FromSeconds(0.6), new RawSourceSamplePhrase(new RawSourceSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(48000, 1), [], 0), "hundred"))
        ]));
    }

    [RelayCommand]
    private void Parse(string language)
    {
        if (_manager == null)
            return;
        try
        {
            Error = "";
            Parsed.Clear();
            var parser = new PhraseParser<WavePhraseBase>(Input, language == "English" ? _manager.EnglishTree : _manager.HungarianTree);
            var index = 0;
            var list = new List<UtteranceSegment<WavePhraseBase>>();
            while (parser.Next(out var phrase))
            {
                if (index == phrase.EndIndex && Parsed.Count != 0)
                {
                    list.Add(phrase);
                    continue;
                }

                var start = index;
                index = phrase.EndIndex == -1 ? Input.Length : phrase.EndIndex;
                list.Add(phrase);
                Parsed.Add(new ParsedText(Input.Substring(start, index - start), list));
                list = [];
            }
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
    }

}

public sealed record ParsedText(string Text, IReadOnlyCollection<UtteranceSegment<WavePhraseBase>> Segments);
