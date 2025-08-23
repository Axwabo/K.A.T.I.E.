using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Katie.NAudio.Phrases;
using Katie.UI.PhraseProviders;

namespace Katie.UI.Desktop;

public sealed class FileStreamToPhraseConverter : IFileToPhraseConverter
{

    public async Task<WaveStreamPhrase> ToPhraseAsync(IStorageFile file)
        => new(await file.OpenReadAsync(), Path.GetFileNameWithoutExtension(file.Name));

}
