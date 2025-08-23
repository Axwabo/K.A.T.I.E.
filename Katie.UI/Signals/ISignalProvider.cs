namespace Katie.UI.Signals;

public interface ISignalProvider
{

    IAsyncEnumerable<Signal> EnumerateSignalsAsync();

}
