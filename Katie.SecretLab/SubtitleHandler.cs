using System.Text;
using Katie.NAudio;

namespace Katie.SecretLab;

public static class SubtitleHandler
{

    private const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։ </color></mark> ";
    private const string Split = "<split>";

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
                announcementBuilder.AppendSilence(time).Append(Split);
                subtitleBuilder.Append(text[start..end].Trim()).Append(Split).Append(SubtitlePrefix);
                start = end;
                time = TimeSpan.Zero;
            }

            time += segment.Duration;
            if (segment.EndIndex < 1 || segment.Phrase != null || text[segment.EndIndex - 1] != '.')
                continue;
            wasFullStop = true;
            end = segment.EndIndex;
        }

        return endsWithSplit
            ? (
                announcementBuilder.RemoveEnd(Split.Length).ToString(),
                subtitleBuilder.RemoveEnd(Split.Length + SubtitlePrefix.Length).ToString()
            )
            : (
                announcementBuilder.AppendSilence(time).ToString(),
                subtitleBuilder.Append(text[end..].Trim()).ToString()
            );
    }

    extension(StringBuilder builder)
    {

        private StringBuilder AppendSilence(TimeSpan time)
            => time == TimeSpan.Zero
                ? builder
                : builder.Append(" $SLEEP_").Append((float) time.TotalSeconds).Append(" $MAXDUR_0 . ");

        private StringBuilder RemoveEnd(int characters)
            => builder.Remove(builder.Length - characters, characters);

    }

}
