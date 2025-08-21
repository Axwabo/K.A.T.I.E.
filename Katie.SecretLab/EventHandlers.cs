using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;

namespace Katie.SecretLab;

internal sealed class EventHandlers : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers() => AnnouncementManager.Initialize();

    public override void OnServerCassieAnnouncing(CassieAnnouncingEventArgs ev)
    {
        if (AnnouncementManager.Instance.Play(ev.Words))
            ev.IsAllowed = false;
    }

}
