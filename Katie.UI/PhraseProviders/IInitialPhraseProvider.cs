namespace Katie.UI.PhraseProviders;

public interface IInitialPhraseProvider
{

    Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global);

}
