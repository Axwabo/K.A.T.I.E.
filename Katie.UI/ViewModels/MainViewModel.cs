using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    public IReadOnlyCollection<string> Pages { get; } = [nameof(Phrases), nameof(Cache), nameof(Inspect)];

    public PhrasesPageViewModel Phrases { get; }

    public CacheManagerViewModel Cache { get; }

    public InspectPageViewModel Inspect { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PhrasesVisible), nameof(CacheVisible), nameof(InspectVisible))]
    private string _page = nameof(Phrases);

    [ObservableProperty]
    private bool _navigationBlocked;

    public bool PhrasesVisible => Page == nameof(Phrases);

    public bool CacheVisible => Page == nameof(Cache);

    public bool InspectVisible => Page == nameof(Inspect);

    public MainViewModel(PhrasesPageViewModel phrases, CacheManagerViewModel cache, InspectPageViewModel inspect)
    {
        Phrases = phrases;
        Cache = cache;
        Inspect = inspect;
    }

    public MainViewModel() : this(new PhrasesPageViewModel(), new CacheManagerViewModel(), new InspectPageViewModel())
    {
    }

    [RelayCommand]
    private void Navigate(string page) => Page = page;

}
