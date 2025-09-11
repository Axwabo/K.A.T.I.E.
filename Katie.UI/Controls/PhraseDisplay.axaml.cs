using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Katie.UI.Controls;

public sealed partial class PhraseDisplay : UserControl
{

    public static readonly StyledProperty<PhrasePackViewModel> PackProperty = AvaloniaProperty.Register<PhraseDisplay, PhrasePackViewModel>(nameof(Pack));

    public required PhrasePackViewModel Pack
    {
        get => GetValue(PackProperty);
        set => SetValue(PackProperty, value);
    }

    private string? _original;

    private bool _lostFocus;

    private Visual? FocusedVisual => TopLevel.GetTopLevel(this) is not {FocusManager: { } focus} ? null : focus.GetFocusedElement() as Visual;

    private WavePhraseBase? Phrase => DataContext as WavePhraseBase;

    private bool Editing
    {
        get => Input.IsVisible;
        set => Input.IsVisible = value;
    }

    public PhraseDisplay() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _original = Phrase?.Text;
        Chain.IsVisible = DataContext is WavePhraseAlias;
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        _lostFocus = !this.IsVisualAncestorOf(FocusedVisual);
        if (_lostFocus && Editing)
            ToggleEditing();
    }

    private void EditClicked(object? sender, RoutedEventArgs e)
    {
        var text = Input.Text;
        if (Input.IsVisible && !string.IsNullOrWhiteSpace(text) && !string.Equals(_original, text))
            Pack.EditOrCreateAlias(Phrase!, text);
        if (FocusedVisual == Edit || !_lostFocus)
            ToggleEditing(true);
        _lostFocus = false;
    }

    private void ChainClicked(object? sender, RoutedEventArgs e)
    {
        var parent = this.FindAncestorOfType<ItemsControl>();
        if (parent == null)
            return;
        var original = ((WavePhraseAlias) DataContext!).Original;
        foreach (var child in parent.GetVisualDescendants())
            if (child is PhraseDisplay display && display.DataContext == original)
            {
                display.Edit.Focus(NavigationMethod.Tab);
                break;
            }
    }

    private void ToggleEditing(bool focus = false)
    {
        Label.IsVisible = !(Editing = !Editing);
        Edit.Content = Editing ? "✔" : "✏";
        if (focus && Editing)
            Input.Focus();
    }

}
