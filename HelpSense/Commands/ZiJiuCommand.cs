using CommandSystem;
using HelpSense.Helper;
using MEC;
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

        public string[] Aliases => new string[] { "Rescue", "ZJ" };

        public string Description => "卡虚空自救";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if(sender is null || (player = Player.Get(sender)) is null)
            {

            }

            WaypointBase.GetRelativePosition(player.Position, out byte id, out _);

            if (player != null && player.IsAlive && player.Zone is MapGeneration.FacilityZone.None && (WaypointBase.TryGetWaypoint(id, out WaypointBase waypoint) && waypoint is not ElevatorWaypoint))
            {
                player.IsGodModeEnabled = true;
                Timing.CallDelayed(1f, () =>
                {
                    Timing.CallDelayed(2f, () =>
                    {
                        player.IsGodModeEnabled = false;
                    });
                    player.Position = player.Role.GetRandomSpawnLocation();
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
