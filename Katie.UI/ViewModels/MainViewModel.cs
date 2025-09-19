using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    public IReadOnlyCollection<string> Pages { get; } = [nameof(Phrases), nameof(Cache), nameof(Inspect), nameof(Queue)];

    public PhrasesPageViewModel Phrases { get; }

    public CacheManagerViewModel Cache { get; }

    public InspectPageViewModel Inspect { get; }

    public QueuePageViewModel Queue { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PhrasesVisible), nameof(CacheVisible), nameof(InspectVisible), nameof(QueueVisible))]
    private string _page = nameof(Phrases);

    [ObservableProperty]
    private bool _navigationBlocked;

    public bool PhrasesVisible => Page == nameof(Phrases);

    public bool CacheVisible => Page == nameof(Cache);

    public bool InspectVisible => Page == nameof(Inspect);

    public bool QueueVisible => Page == nameof(Queue);

    public MainViewModel(PhrasesPageViewModel phrases, CacheManagerViewModel cache, InspectPageViewModel inspect, QueuePageViewModel queue)
    {
        Phrases = phrases;
        Cache = cache;
        Inspect = inspect;
        Queue = queue;
    }

    public MainViewModel()
    {
        Phrases = new PhrasesPageViewModel();
        Cache = new CacheManagerViewModel();
        Inspect = new InspectPageViewModel(Phrases);
        Queue = new QueuePageViewModel(Phrases, null);
    }

    [RelayCommand]
    private void Navigate(string page) => Page = page;

}
