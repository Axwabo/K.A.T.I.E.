using System.Collections.ObjectModel;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using Katie.Core;
using Katie.UI.Audio;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class InspectPageViewModel : ViewModelBase
{

    private const string DefaultNote = "There may be words that do not map to any phrase. These will be replaced with a pause of (number of characters * 100) milliseconds.";

    public PhrasesPageViewModel PhrasesView { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NoteBrush))]
    private string _note = DefaultNote;

    public IBrush NoteBrush => Note == DefaultNote ? Brushes.White : Brushes.Red;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Orientation))]
    private bool _vertical;

    public Orientation Orientation => Vertical ? Orientation.Vertical : Orientation.Horizontal;

    public ObservableCollection<ParsedText> Parsed { get; } = [];

    public InspectPageViewModel(PhrasesPageViewModel phrasesView) => PhrasesView = phrasesView;

    public InspectPageViewModel() : this(new PhrasesPageViewModel())
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
        try
        {
            Note = DefaultNote;
            Parsed.Clear();
            var input = PhrasesView.Text;
            var parser = new PhraseParser<WavePhraseBase>(input, language == "English" ? PhrasesView.Phrases.EnglishTree : PhrasesView.Phrases.HungarianTree);
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
                index = phrase.EndIndex == -1 ? input.Length : phrase.EndIndex;
                list.Add(phrase);
                Parsed.Add(new ParsedText(input.Substring(start, index - start), list));
                list = [];
            }
        }
        catch (Exception e)
        {
            Note = e.Message;
        }
    }

}

public sealed record ParsedText(string Text, IReadOnlyCollection<UtteranceSegment<WavePhraseBase>> Segments);
