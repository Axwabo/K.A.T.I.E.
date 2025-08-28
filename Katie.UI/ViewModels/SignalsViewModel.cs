using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.ViewModels;

public sealed partial class SignalsViewModel : ViewModelBase
{

    public static Signal DefaultSignal { get; } = new(null!, "None", TimeSpan.Zero);

    public ObservableCollection<Signal> List { get; } = [DefaultSignal];

    [ObservableProperty]
    private Signal _selected = DefaultSignal;

    private readonly ISignalProvider? _signalPicker;

    public SignalsViewModel([FromKeyedServices(nameof(FilePickerSignalProvider))] ISignalProvider? signalPicker, ISignalProvider? initialSignals = null)
    {
        _signalPicker = signalPicker;
        if (!Design.IsDesignMode)
            LoadSignals(initialSignals).ConfigureAwait(false);
    }

    public SignalsViewModel()
    {
    }

    private async Task LoadSignals(ISignalProvider? signalProvider)
    {
        if (signalProvider == null)
            return;
        var signals = new List<Signal>();
        await foreach (var provider in signalProvider.EnumerateSignalsAsync())
            signals.Add(provider);
        Dispatcher.UIThread.Post(() =>
        {
            foreach (var signal in signals)
                List.Add(signal);
        });
    }

    [RelayCommand]
    private Task AddSignals() => LoadSignals(_signalPicker);

}
