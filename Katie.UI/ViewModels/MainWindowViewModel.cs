using Katie.UI.Views;

namespace Katie.UI.ViewModels;

public sealed class MainWindowViewModel
{

    public MainViewModel? ViewModel { get; }

    public MainWindowViewModel(MainWindow? window = null) => ViewModel = new MainViewModel(window);

}
