namespace Katie.UI.ViewModels;

public sealed class MainWindowViewModel
{

    public MainViewModel? ViewModel { get; }

    public MainWindowViewModel(MainViewModel? viewModel) => ViewModel = viewModel;

    public MainWindowViewModel() : this(null)
    {
    }

}
