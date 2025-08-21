namespace Katie.UI.SignalProviders;

public interface ISignalProvider
{

    public static ISignalProvider? InitialProvider { get; set; }

    IAsyncEnumerable<RawSourceSampleProvider> EnumerateSignalsAsync();

}
