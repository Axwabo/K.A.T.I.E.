using Katie.UI.Views;

namespace Katie.UI.ViewModels;

public sealed class MainWindowViewModel
{

    public MainViewModel? ViewModel { get; }

    public MainWindowViewModel(MainWindow? window) => ViewModel = new MainViewModel(window);

    public MainWindowViewModel() : this(null)
    {
    }

}
