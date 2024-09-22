using CommandSystem;
using HelpSense.Helper;
using MEC;
using NWAPIPermissionSystem;
using PluginAPI.Core;
using RelativePositioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class ZiJiuCommand : ICommand
    {
        public string Command => "ZiJiu";

        public string[] Aliases => new string[] { "ZJ" };

        public string Description => "卡虚空自救";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player Player = Player.Get(sender);
            WaypointBase.GetRelativePosition(Player.Position, out byte id, out _);
            if (Player != null && Player.IsAlive && Player.Zone is MapGeneration.FacilityZone.None && (WaypointBase.TryGetWaypoint(id, out WaypointBase waypoint) && waypoint is not ElevatorWaypoint))
            {
                Player.IsGodModeEnabled = true;
                Timing.CallDelayed(1f, () =>
                {
                    Player.Position = XHelper.GetRandomSpawnLocation(Player.Role);
                });
                Timing.CallDelayed(3f, () =>
                {
                    Player.IsGodModeEnabled = false;
                });
                response = "成功";
                return true;
            }
            else
            {
                response = "失败，可能指令未启用或者身份不允许等";
                return false;
            }
        }
    }
}
