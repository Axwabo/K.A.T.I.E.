using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    public IReadOnlyCollection<string> Pages { get; } = [nameof(Phrases), nameof(Shit)];

    public PhrasesPageViewModel Phrases { get; }

    public PageViewModel Shit { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PhrasesVisible), nameof(ShitVisible))]
    private string _page;

    [ObservableProperty]
    private bool _navigationBlocked;

    public bool PhrasesVisible => Page == nameof(Phrases);

    public bool ShitVisible => Page == nameof(Shit);

    public MainViewModel(PhrasesPageViewModel phrases)
    {
        _page = nameof(Phrases);
        Phrases = phrases;
        Shit = new ShitModel();
    }

    public MainViewModel() : this(new PhrasesPageViewModel())
    {
    }

    [RelayCommand]
    private void Navigate(string page) => Page = page;

}

internal sealed class ShitModel : PageViewModel
{

    public override bool NavigationBlocked { get; }

}
