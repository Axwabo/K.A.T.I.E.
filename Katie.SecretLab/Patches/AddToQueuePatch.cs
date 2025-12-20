using System.Reflection.Emit;
using Cassie;
using HarmonyLib;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;

namespace Katie.SecretLab.Patches;

[HarmonyPatch(typeof(CassieAnnouncementDispatcher), nameof(CassieAnnouncementDispatcher.AddToQueue))]
public static class AddToQueuePatch
{

    public static bool Queue(CassieAnnouncement announcement)
    {
        if (announcement is KatieAnnouncement)
            return true;
        var ev = new CassieAnnouncingEventArgs(
            announcement.Payload.Content,
            false,
            announcement.Payload.PlayBackground,
            announcement.Payload.SubtitleSource is CassieTtsPayload.SubtitleMode.Custom or CassieTtsPayload.SubtitleMode.Automatic,
            announcement.Payload._customSubtitle
        );
        ServerEvents.OnCassieAnnouncing(ev);
        return ev.IsAllowed;
    }

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var list = instructions.ToList();
        var label = generator.DefineLabel();
        list[0].labels.Add(label);
        list.InsertRange(0, [
            new CodeInstruction(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(AddToQueuePatch), nameof(Queue)),
            new CodeInstruction(OpCodes.Brtrue, label),
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Ret)
        ]);
        return list;
    }

}
