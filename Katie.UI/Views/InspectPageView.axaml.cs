using Avalonia;
using Avalonia.Markup.Xaml.Templates;

namespace Katie.UI.Views;

public sealed partial class InspectPageView : UserControl
{

    private const string MinimalKey = "Minimal";
    private const string FullKey = "Full";

    public static readonly StyledProperty<bool> MinimalProperty = AvaloniaProperty.Register<InspectPageView, bool>(nameof(Minimal), true);

    public static readonly StyledProperty<DataTemplate> ItemTemplateProperty = AvaloniaProperty.Register<InspectPageView, DataTemplate>(nameof(ItemTemplate));

    public bool Minimal
    {
        get => GetValue(MinimalProperty);
        set => SetValue(MinimalProperty, value);
    }

    public DataTemplate ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public InspectPageView()
    {
        InitializeComponent();
        RefreshTemplate();
    }

    private void RefreshTemplate() => ItemTemplate = (DataTemplate) Resources[Minimal ? MinimalKey : FullKey]!;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MinimalProperty)
            RefreshTemplate();
    }

}
