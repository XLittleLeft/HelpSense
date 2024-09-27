using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using HelpSense.Helper;
using HelpSense.Helper.Misc;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.NetworkMessages;
using PlayerRoles.Spectating;
using UnityEngine;

namespace HelpSense.Patches
{
    [HarmonyPatch(typeof(FpcServerPositionDistributor), nameof(FpcServerPositionDistributor.GetNewSyncData))]
    public class PositionSyncingPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Newobj);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldarg_0),
            new (OpCodes.Call, AccessTools.Method(typeof(PositionSyncingPatch), nameof(CheckTutorialPosition))),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static Vector3 CheckTutorialPosition(Vector3 position, ReferenceHub tutorial, ReferenceHub spectator)
        {
            if (HideTutorialsHelper.IsWhitelisted(spectator))
                return position;

            if (tutorial.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Tutorial)
                return position;

            if (spectator.roleManager.CurrentRole is not SpectatorRole spectatorRole)
                return position;

            if (spectatorRole.SyncedSpectatedNetId != tutorial.netId && spectatorRole.SyncedSpectatedNetId != 0)
                return position;

            return Vector3.zero;
        }
    }
}