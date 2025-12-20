using System.Text;
using Katie.NAudio;

namespace Katie.SecretLab;

public static class SubtitleHandler
{

    private const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։  </color></mark> ";
    private const string Split = "<split>";
    private const double SilenceDuration = 0.5;

    public static (string Announcement, string Subtitles) MakeCassieAnnouncement(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var announcementBuilder = new StringBuilder();
        var subtitleBuilder = new StringBuilder(SubtitlePrefix);
        var time = chain.Current.Segment.Duration;
        var start = Index.Start;
        var end = Index.Start;
        var wasFullStop = false;
        var endsWithSplit = false;
        foreach (var segment in chain.Remaining)
        {
            endsWithSplit = wasFullStop;
            if (wasFullStop)
            {
                wasFullStop = false;
                subtitleBuilder.Append(text[start..end]).Append(Split).Append(SubtitlePrefix);
                start = end;
                time = TimeSpan.Zero;
            }

            time += segment.Duration;
            if (segment.EndIndex < 1 || text[segment.EndIndex - 1] != '.')
                continue;
            wasFullStop = true;
            end = segment.EndIndex;
        }

        return endsWithSplit
            ? (
                announcementBuilder.RemoveEnd(Split.Length, endsWithSplit).ToString(),
                subtitleBuilder.RemoveEnd(Split.Length + SubtitlePrefix.Length, endsWithSplit).ToString()
            )
            : (
                announcementBuilder.AppendSilence(time).ToString(),
                subtitleBuilder.Append(text[end..].Trim()).ToString()
            );
    }

    private static StringBuilder AppendSilence(this StringBuilder builder, TimeSpan time)
        => builder.Append("$SLEEP_").Append((float) time.TotalSeconds);

    private static StringBuilder RemoveEnd(this StringBuilder builder, int characters, bool condition)
        => condition ? builder.Remove(builder.Length - characters, characters) : builder;

}
