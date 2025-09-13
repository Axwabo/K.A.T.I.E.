using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;

namespace Katie.SecretLab;

internal sealed class EventHandlers : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers() => AnnouncementManager.Initialize();

    public override void OnServerCassieAnnouncing(CassieAnnouncingEventArgs ev)
    {
        if (AnnouncementManager.Instance.OverrideCassieAnnouncement(ev.Words, ev.MakeNoise))
            ev.IsAllowed = false;
    }

}
