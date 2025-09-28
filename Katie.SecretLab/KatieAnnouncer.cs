using Katie.Core.DataStructures;
using Katie.NAudio.Phrases;

namespace Katie.SecretLab;

public static class KatieAnnouncer
{

    public static byte ControllerId => AnnouncementManager.Instance.Player.Id;

    public static bool IsKatieSpeaking => AnnouncementManager.Instance.IsSpeaking;

    public static bool IsCassieSpeaking => NineTailedFoxAnnouncer.singleton.queue is [{collection: not Subtitles.Collection}, ..];

    public static bool IsAnyAnnouncerSpeaking => IsCassieSpeaking || IsKatieSpeaking;

    public static void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, ReadOnlySpan<char> signal, bool showSubtitles = true)
        => AnnouncementManager.Instance.Play(text, tree, signal, false, showSubtitles);

    public static void Play(ReadOnlySpan<char> text, PhraseTree<WavePhraseBase> tree, bool noisy, bool showSubtitles = true)
        => AnnouncementManager.Instance.Play(text, tree, ReadOnlySpan<char>.Empty, noisy, showSubtitles);

    public static void Stop() => AnnouncementManager.Instance.Stop();

}
