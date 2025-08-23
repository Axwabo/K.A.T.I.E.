using Avalonia.Platform.Storage;

namespace Katie.UI.PhraseProviders;

public interface IFileToPhraseConverter
{

    Task<WaveStreamPhrase> ToPhraseAsync(IStorageFile file);

}
