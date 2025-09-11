namespace Katie.UI.ViewModels;

public abstract class ViewModelBase : ObservableObject;

public abstract class PageViewModel : ViewModelBase
{

    public abstract bool NavigationBlocked { get; }

}
