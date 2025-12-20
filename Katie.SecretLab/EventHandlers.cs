using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;

namespace Katie.SecretLab;

internal sealed class EventHandlers : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers() => AnnouncementManager.Initialize();

    public override void OnServerCassieAnnouncing(CassieAnnouncingEventArgs ev)
    {
        if (ev.CustomAnnouncement && OverrideCassieAnnouncement(ev.Words, ev.MakeNoise))
        {
            ev.IsAllowed = false;
            return;
        }

        var config = KatiePlugin.Instance.Config!;
        if (!config.ReplaceCassie || !PhraseCache.TryGetTree(config.DefaultLanguage, out var tree))
            return;
        ev.IsAllowed = false;
        if (ev is {CustomAnnouncement: true, MakeNoise: true})
            KatieAnnouncer.Play(ev.Words, tree, true);
        else if (!ev.CustomAnnouncement && !string.IsNullOrEmpty(config.DefaultSignal))
            KatieAnnouncer.Play(ev.Words, tree, config.DefaultSignal, false);
        else
            KatieAnnouncer.Play(ev.Words, tree, ev.MakeNoise, ev.CustomAnnouncement);
    }

    private static bool OverrideCassieAnnouncement(string text, bool noisy)
    {
        var span = text.AsSpan().Trim();
        if (!span.TryGetBracketsValue(0, out var languageEnd, out var language) || !PhraseCache.TryGetTree(language.Trim(), out var tree))
            return false;
        languageEnd++;
        if (span.TryGetBracketsValue(languageEnd, out var signalEnd, out var signal))
            languageEnd = signalEnd + 1;
        var announcement = span[languageEnd..].Trim();
        KatieAnnouncement.Play(announcement, tree, signal, noisy && signal.Trim().IsEmpty);
        return true;
    }

}

file static class Extensions
{

    public static bool TryGetBracketsValue(this ReadOnlySpan<char> span, int startIndex, out int endIndex, out ReadOnlySpan<char> match)
    {
        match = default;
        endIndex = 0;
        var search = span[startIndex..];
        var start = search.IndexOf('[');
        if (start == -1)
            return false;
        var end = search.IndexOf(']');
        if (end == -1)
            return false;
        endIndex = startIndex + end;
        match = span[(startIndex + start + 1)..endIndex];
        return true;
    }

}
