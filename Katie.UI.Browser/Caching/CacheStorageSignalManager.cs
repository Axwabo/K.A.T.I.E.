using System.Threading.Tasks;
using Katie.UI.Browser.JSInterop;
using Katie.UI.Extensions;
using Katie.UI.Signals;

namespace Katie.UI.Browser.Caching;

public sealed class CacheStorageSignalManager : ISignalCacheManager
{

    public Task CacheAsync(Signal signal)
        => signal.Data == null
            ? Task.CompletedTask
            : SignalCacheFunctions.Save(signal.Name, signal.Data.ToArraySegment());

    public Task ClearAsync() => SignalCacheFunctions.DeleteAll();

}
