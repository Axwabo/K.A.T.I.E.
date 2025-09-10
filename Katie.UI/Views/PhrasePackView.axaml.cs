using CommunityToolkit.Mvvm.Input;

namespace Katie.UI.Views;

public partial class PhrasePackView : UserControl
{

    public PhrasePackView() => InitializeComponent();

    public IRelayCommand? RemovePhrase => (DataContext as PhrasePackViewModel)?.RemovePhraseCommand;

    private void PhraseDisplay_OnNameChanged(object? sender, string text)
    {
        if (DataContext is not PhrasePackViewModel vm || sender is not UserControl {DataContext: SamplePhraseBase phrase})
            return;
        /*TODO: phrases to records

         var index = vm.List.IndexOf(phrase);
        if (index != -1)
            vm.List[index] = phrase with {Text = text};*/
    }

}
