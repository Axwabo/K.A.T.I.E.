namespace Katie.UI.PhraseProviders;

public interface IInitialPhraseLoader
{

    Task LoadPhrasesAsync(PhrasePackViewModel hungarian, PhrasePackViewModel english, PhrasePackViewModel global);

}
