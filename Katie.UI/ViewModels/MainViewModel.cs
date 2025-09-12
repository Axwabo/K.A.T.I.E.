using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    public IReadOnlyCollection<string> Pages { get; } = [nameof(Phrases), nameof(Cache)];

    public PhrasesPageViewModel Phrases { get; }

    public CacheManagerViewModel Cache { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PhrasesVisible), nameof(CacheVisible))]
    private string _page = nameof(Phrases);

    [ObservableProperty]
    private bool _navigationBlocked;

    public bool PhrasesVisible => Page == nameof(Phrases);

    public bool CacheVisible => Page == nameof(Cache);

    public MainViewModel(PhrasesPageViewModel phrases, CacheManagerViewModel cache)
    {
        Phrases = phrases;
        Cache = cache;
    }

    public MainViewModel() : this(new PhrasesPageViewModel(), new CacheManagerViewModel())
    {
    }

    [RelayCommand]
    private void Navigate(string page) => Page = page;

}
