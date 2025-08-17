using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;

namespace Katie.UI.ViewModels;

public sealed partial class PhrasePackViewModel : ViewModelBase
{

    private static readonly FilePickerOpenOptions Options = new()
    {
        Title = "Add phrases",
        AllowMultiple = true,
        FileTypeFilter =
        [
            new FilePickerFileType("Wave files")
            {
                Patterns = ["*.wav"],
                MimeTypes = ["audio/wav"]
            }
        ]
    };

    public Control? Host { get; init; }

    public required string Language { get; set; }

    public string Content => $"Add {Language} phrases";

    public ObservableCollection<WavePhrase> List { get; } = [];

    public event Action? PhrasesChanged;

    [RelayCommand]
    public async Task AddPhrases()
    {
        if (TopLevel.GetTopLevel(Host) is not {StorageProvider: {CanOpen: true} storage})
            return;
        var files = await storage.OpenFilePickerAsync(Options);
        foreach (var file in files)
            List.Add(new WavePhrase(await file.OpenReadAsync(), Path.GetFileNameWithoutExtension(file.Name)));
        PhrasesChanged?.InvokeOnUIThread();
    }

}
