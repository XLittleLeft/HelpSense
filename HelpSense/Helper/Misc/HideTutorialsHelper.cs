using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using RelativePositioning;
using System.Collections.Generic;
using UnityEngine;

namespace HelpSense.Helper.Misc
{
    public static class HideTutorialsHelper
    {
        public static void SendRoleAndPosition(Player spectator, Player target, RoleTypeId role, Vector3 position)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteUShort(38952);
            writer.WriteUInt(target.NetworkId);
            writer.WriteRoleType(role);

            if (target.RoleBase is not IFpcRole fpc)
                return;

            fpc.FpcModule.MouseLook.GetSyncValues(0, out ushort syncH, out _);
            writer.WriteRelativePosition(new RelativePosition(position));
            writer.WriteUShort(syncH);

            spectator.Connection.Send(writer.ToArraySegment());
            NetworkWriterPool.Return(writer);
        }

        public static void ResyncSpectator(Player spectator, Player target)
        {
            SendRoleAndPosition(spectator, target, target.Role, Vector3.zero);
        }

        public static IEnumerator<float> DesyncSpectator(Player spectator, Player target)
        {
            yield return Timing.WaitForOneFrame;
            SendRoleAndPosition(spectator, target, RoleTypeId.Spectator, Vector3.zero);
        }

        public static bool IsWhitelisted(ReferenceHub referenceHub) => referenceHub.authManager.RemoteAdminGlobalAccess || referenceHub.serverRoles.RemoteAdmin || referenceHub.roleManager.CurrentRole.RoleTypeId is RoleTypeId.Overwatch;
    }
}
