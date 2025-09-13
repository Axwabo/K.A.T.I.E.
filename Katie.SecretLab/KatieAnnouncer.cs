using Katie.Core.DataStructures;
using Katie.NAudio.Phrases;

namespace Katie.SecretLab;

public static class KatieAnnouncer
{

    public static byte ControllerId => AnnouncementManager.Instance.Player.Id;

    public static bool IsKatieSpeaking => AnnouncementManager.Instance.IsSpeaking;

    public static bool IsCassieSpeaking => NineTailedFoxAnnouncer.singleton.queue is [{collection: not Subtitles.Collection}, ..];

    public static bool IsAnyAnnouncerSpeaking => IsCassieSpeaking || IsKatieSpeaking;

    public static void Play(ReadOnlySpan<char> text, ReadOnlySpan<char> language, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal)
        => AnnouncementManager.Instance.Play(text, language, tree, signal, false);

    public static void Play(ReadOnlySpan<char> text, ReadOnlySpan<char> language, PhraseTree<WavePhraseBase> tree, bool noisy)
        => AnnouncementManager.Instance.Play(text, language, tree, ReadOnlySpan<char>.Empty, noisy);

}
