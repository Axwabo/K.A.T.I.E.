using Avalonia;

namespace Katie.UI.Views;

public sealed partial class InspectPageView : UserControl
{

    public static readonly StyledProperty<bool> MinimalProperty = AvaloniaProperty.Register<InspectPageView, bool>(nameof(Minimal), true);

    public bool Minimal
    {
        get => GetValue(MinimalProperty);
        set => SetValue(MinimalProperty, value);
    }

    public InspectPageView() => InitializeComponent();

}
