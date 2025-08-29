using System.Text;
using Katie.NAudio;
using Respawning;

namespace Katie.SecretLab;

public static class Subtitles
{

    private const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։  </color></mark> ";
    private const string Split = "<split>";
    private const double SilenceDuration = 0.5;

    public const string Collection = "K.A.T.I.E.";

    public static void Play(string announcement, string subtitles)
    {
        var queue = NineTailedFoxAnnouncer.singleton.queue;
        var start = queue.Count;
        foreach (var controller in RespawnEffectsController.AllControllers)
            controller.RpcCassieAnnouncement(announcement, false, false, true, subtitles);
        for (var i = start; i < queue.Count; i++)
            queue[i].collection = Collection;
    }

    public static void Delay(float seconds)
        => NineTailedFoxAnnouncer.singleton.queue.Add(new NineTailedFoxAnnouncer.VoiceLine {length = seconds, collection = Collection});

    public static (string Announcement, string Subtitles) MakeCassieAnnouncement(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var announcementBuilder = new StringBuilder();
        var subtitleBuilder = new StringBuilder(SubtitlePrefix);
        var time = chain.Current.Segment.Duration;
        var start = 0;
        var wasFullStop = false;
        var anySplits = false;
        foreach (var segment in chain.Remaining)
        {
            time += segment.Duration;
            if (wasFullStop)
            {
                announcementBuilder.AppendSilence(time).Append(Split);
                var end = segment.EndIndex == -1 ? ^0 : segment.EndIndex + 1;
                subtitleBuilder.Append(text[start..end].Trim());
                subtitleBuilder.Append(Split).Append(SubtitlePrefix);
                start = end.GetOffset(text.Length);
                time = segment.Duration;
                wasFullStop = false;
                anySplits = true;
            }

            if (segment.EndIndex == -1 || text[segment.EndIndex] == '.')
                wasFullStop = true;
        }

        return (
            announcementBuilder.RemoveEnd(Split.Length, anySplits).ToString(),
            subtitleBuilder.RemoveEnd(Split.Length + SubtitlePrefix.Length, anySplits).ToString()
        );
    }

    private static StringBuilder AppendSilence(this StringBuilder builder, TimeSpan time)
    {
        var seconds = time.TotalSeconds;
        for (; seconds > SilenceDuration; seconds -= SilenceDuration)
            builder.Append(" . ");
        return seconds > 0
            ? builder.Append(" pitch_").Append(SilenceDuration / seconds).Append(" . pitch_1 ")
            : builder.Append(" pitch_1");
    }

    private static StringBuilder RemoveEnd(this StringBuilder builder, int characters, bool condition)
        => condition ? builder.Remove(builder.Length - characters, characters) : builder;

}
