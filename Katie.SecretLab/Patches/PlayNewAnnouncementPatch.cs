using System.Reflection;
using System.Reflection.Emit;
using Cassie;
using HarmonyLib;

namespace Katie.SecretLab.Patches;

[HarmonyPatch(typeof(CassieAnnouncementDispatcher), nameof(CassieAnnouncementDispatcher.PlayNewAnnouncement))]
internal static class PlayNewAnnouncementPatch
{

    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var list = instructions.ToList();
        var index = list.FindIndex(static e => e.operand is MethodInfo {Name: nameof(CassieAnnouncement.OnStartedPlaying)}) + 1;
        var label = generator.DefineLabel();
        list[index].labels.Add(label);
        list.InsertRange(index, [
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Isinst, typeof(KatieAnnouncement)),
            new CodeInstruction(OpCodes.Brfalse, label),
            new CodeInstruction(OpCodes.Ret)
        ]);
        return list;
    }

}
