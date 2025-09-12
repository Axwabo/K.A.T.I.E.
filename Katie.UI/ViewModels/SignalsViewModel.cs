using CommunityToolkit.Mvvm.Input;
using Katie.UI.Services;
using Katie.UI.Signals;
using Microsoft.Extensions.DependencyInjection;

namespace Katie.UI.ViewModels;

public sealed partial class SignalsViewModel : ViewModelBase
{

    public IReadOnlyCollection<Signal> List { get; }

    [ObservableProperty]
    private Signal _selected = SignalManager.DefaultSignal;

    private readonly SignalManager _manager;

    private readonly ISignalProvider? _signalPicker;

    public SignalsViewModel(SignalManager manager, [FromKeyedServices(nameof(FilePickerSignalProvider))] ISignalProvider? signalPicker)
    {
        _manager = manager;
        _signalPicker = signalPicker;
        List = manager.List;
    }

    public SignalsViewModel() : this(new SignalManager(), null)
    {
    }

    [RelayCommand]
    private Task AddSignals() => _manager.LoadSignals(_signalPicker);

}
