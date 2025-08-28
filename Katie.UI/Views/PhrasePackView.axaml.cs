using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.Views;

public partial class PhrasePackView : UserControl
{

    public PhrasePackView() => InitializeComponent();

    public IAsyncRelayCommand? RemovePhrase => (DataContext as PhrasePackViewModel)?.RemovePhraseCommand;

}
