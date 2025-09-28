using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;

namespace Katie.SecretLab;

internal sealed class EventHandlers : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers() => AnnouncementManager.Initialize();

    public override void OnServerCassieAnnouncing(CassieAnnouncingEventArgs ev)
    {
        if (ev.CustomAnnouncement && AnnouncementManager.Instance.OverrideCassieAnnouncement(ev.Words, ev.MakeNoise))
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

}
