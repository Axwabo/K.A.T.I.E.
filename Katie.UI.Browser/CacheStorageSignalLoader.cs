using System.IO;
using Katie.UI.Signals;
using NAudio.Wave;

namespace Katie.UI.Browser;

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
                var reader = new WaveFileReader(new MemoryStream(bytes));
                yield return new Signal(reader, key, reader.TotalTime);
            }
        }
        finally
        {
            PhraseCacheFunctions.ClearMemory();
        }
    }

}
