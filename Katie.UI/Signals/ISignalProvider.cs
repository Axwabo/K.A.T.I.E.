namespace Katie.UI.Signals;

public interface ISignalProvider
{

    public static ISignalProvider? InitialProvider { get; set; }

    IAsyncEnumerable<Signal> EnumerateSignalsAsync();

}
