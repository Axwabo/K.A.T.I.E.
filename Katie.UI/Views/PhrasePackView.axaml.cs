namespace Katie.UI.Views;

public sealed partial class PhrasePackView : UserControl
{

    public PhrasePackView() => InitializeComponent();

    public PhrasePackViewModel Pack => (PhrasePackViewModel) DataContext!;

    private void PhraseDisplay_OnNameChanged(object? sender, string text)
    {
        if (sender is UserControl {DataContext: WavePhraseBase phrase})
            Pack.EditOrCreateAlias(phrase, text);
    }

}
