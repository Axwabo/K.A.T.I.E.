using Katie.Core.Extensions;
using Katie.NAudio;
using Respawning;

namespace Katie.SecretLab;

public static class Subtitles
{

    public const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։  </color></mark> ";
    private const string Split = "<split>";

    public static void PlayCassie(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var (subtitles, announcement) = MakeCassieAnnouncement(chain, text);
        foreach (var controller in RespawnEffectsController.AllControllers)
            controller.RpcCassieAnnouncement(announcement, false, false, true, subtitles);
    }

    private static (string Subtitles, string Announcement) MakeCassieAnnouncement(UtteranceChain chain, ReadOnlySpan<char> text)
    {
        var fullStops = text.FindFullStops();
        Span<char> subtitleSpan = stackalloc char[SubtitlePrefix.Length + text.Length + fullStops.Count * Split.Length];
        SubtitlePrefix.AsSpan().CopyTo(subtitleSpan);
        text.CopyTo(subtitleSpan[SubtitlePrefix.Length..]);

        var subtitles = subtitleSpan.ToString();
        var announcement = $"pitch_{0.5 / chain.TotalTime.TotalSeconds} .";
        return (subtitles, announcement);
    }

    // TODO: remove this and add context to UtteranceSegment
    private static List<int> FindFullStops(this ReadOnlySpan<char> text)
    {
        var list = new List<int>();
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '.' && !IsOrdinal(text, i))
                list.Add(i);
        }

        return list;
    }

    private static bool IsOrdinal(ReadOnlySpan<char> text, int startIndex)
    {
        var isOrdinal = false;
        for (var i = startIndex - 1; i >= 0; i--)
        {
            var c = text[i];
            if (SpanExtensions.Delimiters.Contains(c))
                break;
            if (!char.IsDigit(c))
                return false;
            isOrdinal = true;
        }

        return isOrdinal;
    }

}
