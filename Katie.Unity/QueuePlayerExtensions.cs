using Katie.Core;
using Katie.Core.DataStructures;

namespace Katie.Unity;

public static class QueuePlayerExtensions
{

    extension(QueuePlayer player)
    {

        public void EnqueueAnnouncement(ReadOnlySpan<char> text, PhraseTree<AudioClipPhrase> tree, Signal? signal = null)
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

        public void EnqueueAnnouncement(ReadOnlySpan<char> text, PhrasePack pack, Signal? signal = null) => player.EnqueueAnnouncement(text, pack.Tree, signal);

    }

}
