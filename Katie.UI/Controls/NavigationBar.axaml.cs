using Avalonia;

namespace Katie.UI.Controls;

public sealed partial class NavigationBar : UserControl
{

    public static readonly StyledProperty<string> CurrentPageProperty = AvaloniaProperty.Register<NavigationBar, string>(nameof(CurrentPage));

    public static readonly StyledProperty<IReadOnlyCollection<string>> PagesProperty = AvaloniaProperty.Register<NavigationBar, IReadOnlyCollection<string>>(nameof(Pages));

    public string CurrentPage
    {
        get => GetValue(CurrentPageProperty);
        set => SetValue(CurrentPageProperty, value);
    }

    public IReadOnlyCollection<string> Pages
    {
        get => GetValue(PagesProperty);
        set => SetValue(PagesProperty, value);
    }

    public NavigationBar() => InitializeComponent();

}
