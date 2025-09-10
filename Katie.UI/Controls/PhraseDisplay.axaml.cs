using Avalonia;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.Controls;

public partial class PhraseDisplay : UserControl
{

    public static readonly StyledProperty<IRelayCommand> DeleteProperty = AvaloniaProperty.Register<PhraseDisplay, IRelayCommand>(nameof(Delete));

    public required IRelayCommand Delete
    {
        get => GetValue(DeleteProperty);
        set => SetValue(DeleteProperty, value);
    }

    public event EventHandler<string>? NameChanged;

    private string? PhraseText => (DataContext as WavePhraseBase)?.Text;

    private string? _original;

    public PhraseDisplay() => InitializeComponent();

    protected override void OnDataContextChanged(EventArgs e) => _original = PhraseText;

    private void Text_OnLostFocus(object? sender, RoutedEventArgs e)
    {
        var text = PhraseText;
        if (!string.IsNullOrWhiteSpace(text) && !string.Equals(_original, text))
            NameChanged?.Invoke(this, text);
        ToggleEditing();
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e) => ToggleEditing();

    private void ToggleEditing() => Input.IsVisible = !(Label.IsVisible = !Label.IsVisible);

}
