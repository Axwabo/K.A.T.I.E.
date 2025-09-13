namespace Katie.UI.Signals;

public interface ISignalCacheManager
{

    Task CacheAsync(Signal signal);

    Task ClearAsync();

}
