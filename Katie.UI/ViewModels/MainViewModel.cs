using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    public IReadOnlyCollection<string> Pages { get; } = [nameof(Phrases), nameof(Cache), nameof(Inspect), nameof(Queue), nameof(Export)];

    public PhrasesPageViewModel Phrases { get; }

    public CacheManagerViewModel Cache { get; }

    public InspectPageViewModel Inspect { get; }

    public QueuePageViewModel Queue { get; }

    public ExportPageViewModel Export { get; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PhrasesVisible), nameof(CacheVisible), nameof(InspectVisible), nameof(QueueVisible), nameof(ExportVisible))]
    private string _page = nameof(Phrases);

    [ObservableProperty]
    private bool _navigationBlocked;

    public bool PhrasesVisible => Page == nameof(Phrases);

    public bool CacheVisible => Page == nameof(Cache);

    public bool InspectVisible => Page == nameof(Inspect);

    public bool QueueVisible => Page == nameof(Queue);

    public bool ExportVisible => Page == nameof(Export);

    public MainViewModel(PhrasesPageViewModel phrases, CacheManagerViewModel cache, InspectPageViewModel inspect, QueuePageViewModel queue, ExportPageViewModel export)
    {
        Phrases = phrases;
        Cache = cache;
        Inspect = inspect;
        Queue = queue;
        Export = export;
    }

    public MainViewModel()
    {
        Phrases = new PhrasesPageViewModel();
        Cache = new CacheManagerViewModel();
        Inspect = new InspectPageViewModel(Phrases);
        Queue = new QueuePageViewModel(Phrases, null);
        Export = new ExportPageViewModel(null, Phrases);
    }

    [RelayCommand]
    private void Navigate(string page) => Page = page;

}
