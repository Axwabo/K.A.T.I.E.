using Katie.Core;
using Katie.Core.DataStructures;

namespace Katie.Unity;

public static class QueuePlayerExtensions
{

    public static void EnqueueAnnouncement(this QueuePlayer player, ReadOnlySpan<char> text, PhraseTree<AudioClipPhrase> tree, Signal? signal)
    {
        if (signal)
            player.Enqueue(signal.Clip, signal.Duration);
        var parser = new PhraseParser<AudioClipPhrase>(text, tree);
        while (parser.Next(out var phrase))
        {
            if (phrase.Duration == TimeSpan.Zero)
                continue;
            var seconds = (float) phrase.Duration.TotalSeconds;
            if (phrase.Phrase != null)
                player.Enqueue(phrase.Phrase.Clip, seconds);
            else
                player.Delay(seconds);
        }
    }

}
