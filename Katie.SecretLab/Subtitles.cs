using System.Text;
using CentralAuth;
using Katie.NAudio;
using Mirror;
using Respawning;
using Utils.Networking;

namespace Katie.SecretLab;

public static class Subtitles
{

    private const string SubtitlePrefix = "<pos=-1px><mark=#000000ff> <color=#88f>Κ．Α．Τ．Ι．Ε．։  </color></mark> ";
    private const string Split = "<split>";
    private const double SilenceDuration = 0.5;

    public const string Collection = "K.A.T.I.E.";

    private const ushort CassieRpcHash = unchecked((ushort) -31296712);

    public static void Announce(string announcement, string? subtitles, bool noisy = false)
    {
        var queue = NineTailedFoxAnnouncer.singleton.queue;
        var start = queue.Count;
        var custom = !string.IsNullOrEmpty(subtitles);
        NineTailedFoxAnnouncer.singleton.AddPhraseToQueue(announcement, noisy, false, false, custom, subtitles ?? "");
        for (var i = start; i < queue.Count; i++)
            queue[i].collection = Collection;
        using var writer = NetworkWriterPool.Get();
        writer.WriteString(announcement);
        writer.WriteBool(false); // makeHold
        writer.WriteBool(noisy); // makeNoise
        writer.WriteBool(custom); // customAnnouncement
        writer.WriteString(subtitles ?? "");
        foreach (var controller in RespawnEffectsController.AllControllers)
            new RpcMessage
            {
                netId = controller.netId,
                componentIndex = controller.ComponentIndex,
                functionHash = CassieRpcHash,
                payload = writer.ToArraySegment()
            }.SendToHubsConditionally(static hub => hub.Mode == ClientInstanceMode.ReadyClient);
    }

    public static void ServerOnlyDelay(float seconds)
        => NineTailedFoxAnnouncer.singleton.queue.Add(new NineTailedFoxAnnouncer.VoiceLine {apiName = ".", length = seconds, collection = Collection});

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
