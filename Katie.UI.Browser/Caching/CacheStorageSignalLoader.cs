using System.IO;
using Katie.UI.Browser.JSInterop;
using Katie.UI.Signals;

namespace Katie.UI.Browser.Caching;

public sealed class CacheStorageSignalLoader : ISignalProvider
{

    public async IAsyncEnumerable<Signal> EnumerateSignalsAsync()
    {
        try
        {
            await SignalCacheFunctions.PrepareCache();
            var keys = PhraseCacheFunctions.GetKeys();
            foreach (var key in keys.Order())
            {
                var bytes = PhraseCacheFunctions.Load(key);
                var stream = new MemoryStream(bytes);
                yield return await Signal.LoadIntoMemory(stream, key);
            }
        }
        finally
        {
            PhraseCacheFunctions.ClearMemory();
        }
    }

}
