using System.Reflection;
using System.Reflection.Emit;
using Cassie;
using HarmonyLib;

namespace Katie.SecretLab.Patches;

[HarmonyPatch(typeof(CassieAnnouncementDispatcher), nameof(CassieAnnouncementDispatcher.PlayNewAnnouncement))]
public static class PlayNewAnnouncementPatch
{

    public static bool IsReady(CassieAnnouncement announcement) => announcement is not KatieAnnouncement {SignalPlayed: false};

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var list = instructions.ToList();
        var index = list.FindIndex(static e => e.operand is MethodInfo {Name: nameof(CassieAnnouncement.OnStartedPlaying)}) + 1;
        var label = generator.DefineLabel();
        list[index].labels.Add(label);
        list.InsertRange(index, [
            new CodeInstruction(OpCodes.Ldarg_0),
            CodeInstruction.Call(typeof(PlayNewAnnouncementPatch), nameof(IsReady)),
            new CodeInstruction(OpCodes.Brtrue, label),
            new CodeInstruction(OpCodes.Ret)
        ]);
        return list;
    }

}
