namespace Katie.UI.Views;

public sealed partial class PhrasePackView : UserControl
{

    public PhrasePackView() => InitializeComponent();

    public PhrasePackViewModel Pack => (PhrasePackViewModel) DataContext!;

}
