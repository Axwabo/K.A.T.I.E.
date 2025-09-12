using System.Collections.ObjectModel;
using Katie.UI.Signals;

namespace Katie.UI.Services;

public sealed class SignalManager
{

    public static Signal DefaultSignal { get; } = new(null!, "None", TimeSpan.Zero);

    public ObservableCollection<Signal> List { get; } = [];

    public SignalManager(ISignalProvider? initialSignals = null)
    {
        if (!Design.IsDesignMode)
            LoadSignals(initialSignals).ConfigureAwait(false);
    }

    public async Task LoadSignals(ISignalProvider? signalProvider)
    {
        if (signalProvider == null)
            return;
        await foreach (var signal in signalProvider.EnumerateSignalsAsync())
            Dispatcher.UIThread.Post(() => List.Add(signal));
    }

}
