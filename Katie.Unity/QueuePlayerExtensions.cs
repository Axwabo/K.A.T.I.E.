using Katie.Core;
using Katie.Core.DataStructures;

namespace Katie.Unity;

public static class QueuePlayerExtensions
{

    extension(QueuePlayer player)
    {

        public TimeSpan EnqueueAnnouncement(ReadOnlySpan<char> text, PhraseTree<AudioClipPhrase> tree, Signal? signal = null)
        {
            var time = TimeSpan.Zero;
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
                time += phrase.Duration;
            }

            return time;
        }

        public TimeSpan EnqueueAnnouncement(ReadOnlySpan<char> text, PhrasePack pack, Signal? signal = null) => player.EnqueueAnnouncement(text, pack.Tree, signal);

    }

}
