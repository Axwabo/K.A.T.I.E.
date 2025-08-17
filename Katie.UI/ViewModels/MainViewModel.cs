using Avalonia.Controls;

namespace Katie.UI.ViewModels;

public sealed class MainViewModel : ViewModelBase
{

    public PhrasePackViewModel Hungarian { get; }

    public PhrasePackViewModel English { get; }

    public PhrasePackViewModel Global { get; }

    public MainViewModel(Control? host)
    {
        English = new PhrasePackViewModel {Host = host, Language = "English"};
        Global = new PhrasePackViewModel {Host = host, Language = "Global"};
        Hungarian = new PhrasePackViewModel {Host = host, Language = "Hungarian"};
    }

    public MainViewModel() : this(null)
    {
    }

}
