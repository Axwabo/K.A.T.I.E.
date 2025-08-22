using System.Text;
using Katie.NAudio;
using Respawning;

namespace Katie.SecretLab;

public static class Subtitles
{

    public const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։  </color></mark> ";
    private const string Split = "<split>";
    private const double SilenceDuration = 0.5;

    public static void PlayCassie(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var (announcement, subtitles) = MakeCassieAnnouncement(chain, text);
        foreach (var controller in RespawnEffectsController.AllControllers)
            controller.RpcCassieAnnouncement(announcement, false, false, true, subtitles);
    }

    private static (string Announcement, string Subtitles) MakeCassieAnnouncement(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var announcementBuilder = new StringBuilder();
        var subtitleBuilder = new StringBuilder(SubtitlePrefix);
        var time = chain.Current.Segment.Duration;
        var start = 0;
        var wasFullStop = false;
        foreach (var segment in chain.Remaining)
        {
            time += segment.Duration;
            if (wasFullStop)
            {
                announcementBuilder.AppendSilence(time);
                announcementBuilder.Append(Split).Append(" pitch_1");
                var end = segment.EndIndex == -1 ? ^0 : segment.EndIndex + 1;
                subtitleBuilder.Append(text[start..end].Trim());
                subtitleBuilder.Append(Split).Append(SubtitlePrefix);
                start = end.GetOffset(text.Length);
                time = TimeSpan.Zero;
                wasFullStop = false;
            }

            if (segment.EndIndex == -1 || text[segment.EndIndex] == '.')
                wasFullStop = true;
        }

        return (announcementBuilder.ToString(), subtitleBuilder.ToString());
    }

    private static void AppendSilence(this StringBuilder builder, TimeSpan time)
    {
        var seconds = time.TotalSeconds;
        for (; seconds > SilenceDuration; seconds -= SilenceDuration)
            builder.Append(" . ");
        builder.Append(" pitch_").Append(SilenceDuration / seconds).Append(" . ");
    }

}
