using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Katie.NAudio;
using Katie.UI.Services;
using NAudio.Wave;

namespace Katie.UI.ViewModels;

public sealed partial class ExportPageViewModel : ViewModelBase
{

    private static readonly FilePickerSaveOptions SaveOptions = new()
    {
        Title = "Export announcement",
        DefaultExtension = "wav",
        FileTypeChoices = [StorageWrapper.WaveFiles]
    };

    [ObservableProperty]
    private string _input = "";

    [ObservableProperty]
    private string _error = "";

    public bool CanSave => Storage?.CanSave ?? false;

    public StorageWrapper? Storage { get; }

    public PhrasesPageViewModel PhrasesPage { get; }

    public ExportPageViewModel(StorageWrapper? storage, PhrasesPageViewModel phrasesPage)
    {
        Storage = storage;
        PhrasesPage = phrasesPage;
    }

    [RelayCommand]
    private async Task Export(string language)
    {
        if (Storage == null)
            return;
        try
        {
            var chain = UtteranceChain.From(Input, PhrasesPage.Phrases[language]);
            if (chain == null)
            {
                Error = "Nothing to export";
                return;
            }

            Error = "";
            var file = await Storage.SaveFilePickerAsync(SaveOptions);
            if (file == null)
                return;
            var provider = PhrasesPage.PrependSignal(chain, out _, out _);
            await using var writer = await file.OpenWriteAsync();
            await using var waveWriter = new WaveFileWriter(writer, new WaveFormat(provider.WaveFormat.SampleRate, provider.WaveFormat.Channels));
            var buffer = new float[480];
            int read;
            while ((read = provider.Read(buffer, 0, buffer.Length)) != 0)
                waveWriter.WriteSamples(buffer, 0, read);
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
    }

}
