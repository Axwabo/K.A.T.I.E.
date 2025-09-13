using Katie.Core.DataStructures;
using Katie.UI.PhraseProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.Services;

public sealed class PhraseManager
{

    public PhrasePackViewModel English { get; }

    public PhrasePackViewModel Hungarian { get; }

    public PhrasePackViewModel Global { get; }

    public PhraseTree<WavePhraseBase> EnglishTree { get; private set; } = new([]);

    public PhraseTree<WavePhraseBase> HungarianTree { get; private set; } = new([]);

    public PhraseManager([FromKeyedServices(nameof(FilePickerPhraseProvider))] IPhraseProvider? phrasePicker = null)
    {
        English = new PhrasePackViewModel {Picker = phrasePicker, Language = "English"};
        Hungarian = new PhrasePackViewModel {Picker = phrasePicker, Language = "Hungarian"};
        Global = new PhrasePackViewModel {Picker = phrasePicker, Language = "Global"};
        Hungarian.PhrasesChanged += RebuildHungarian;
        English.PhrasesChanged += RebuildEnglish;
        Global.PhrasesChanged += RebuildHungarian;
        Global.PhrasesChanged += RebuildEnglish;
    }

    private void RebuildHungarian() => HungarianTree = new PhraseTree<WavePhraseBase>(Global.List.Concat(Hungarian.List));

    private void RebuildEnglish() => EnglishTree = new PhraseTree<WavePhraseBase>(Global.List.Concat(English.List));

}
