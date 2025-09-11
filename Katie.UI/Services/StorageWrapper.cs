using Avalonia.Platform.Storage;

namespace Katie.UI.Services;

public sealed class StorageWrapper
{

    private readonly HostControl? _host;

    private IStorageProvider? _provider;

    private IStorageProvider? Provider => _provider ??= TopLevel.GetTopLevel(_host?.Host)?.StorageProvider;

    public StorageWrapper(HostControl? host) => _host = host;

    public bool CanOpen => Provider?.CanOpen ?? false;

    public bool CanSave => Provider?.CanSave ?? false;

    public Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        => Provider?.OpenFilePickerAsync(options) ?? Task.FromResult<IReadOnlyList<IStorageFile>>([]);

    public Task<IStorageFile?> SaveFilePickerAsync(FilePickerSaveOptions options)
        => Provider?.SaveFilePickerAsync(options) ?? Task.FromResult<IStorageFile?>(null);

    public Task<IStorageFolder?> TryGetFolderFromPathAsync(Uri folderPath)
        => Provider?.TryGetFolderFromPathAsync(folderPath) ?? Task.FromResult<IStorageFolder?>(null);

}
