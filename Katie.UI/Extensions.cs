using Katie.NAudio.Extensions;

namespace Katie.UI;

public static class Extensions
{

    public static void InvokeOnUIThread(this Action action) => Dispatcher.UIThread.Post(action);

    public static List<SamplePhraseBase> ToSamplePhrases(this IEnumerable<SamplePhraseBase> phrases)
    {
        var list = new List<SamplePhraseBase>();
        foreach (var phrase in phrases)
        {
            if (phrase is not WaveStreamPhrase streamPhrase)
            {
                list.Add(phrase);
                continue;
            }

            var samplePhrase = streamPhrase.ToSamplePhrase();
            streamPhrase.Dispose();
            list.Add(samplePhrase);
        }

        return list;
    }

}
