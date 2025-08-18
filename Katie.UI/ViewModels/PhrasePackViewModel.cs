using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.PhraseProviders;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    private FilePickerPhraseProvider? _fileProvider;

    public Control? Host { get; init; }

    public required string Language { get; set; }

    public string Content => $"Add {Language} phrases";

    public ObservableCollection<WavePhrase> List { get; } = [];

    public event Action? PhrasesChanged;

    [RelayCommand]
    private async Task AddPhrases()
    {
        if (TopLevel.GetTopLevel(Host) is not {StorageProvider: {CanOpen: true} storage})
            return;
        _fileProvider ??= new FilePickerPhraseProvider(storage);
        var provider = _fileProvider;
        await AddPhrases(provider);
    }

    public async Task AddPhrases(IPhraseProvider provider)
    {
        var any = false;
        await foreach (var phrase in provider.EnumeratePhrasesAsync())
        {
            List.Add(phrase);
            any = true;
        }

        if (any)
            PhrasesChanged?.InvokeOnUIThread();
    }

}
