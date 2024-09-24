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
    public class RescueCommand : ICommand
    {
        public string Command => "ZiJiu";

        public string[] Aliases => new[] { "Rescue", "ZJ" };

        public string Description => "卡虚空自救";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if(sender is null || (player = Player.Get(sender)) is null)
            {
                response = "执行指令时发生错误，请稍后再试";
                return false;
            }

            WaypointBase.GetRelativePosition(player.Position, out byte id, out _);

            if (!player.IsAlive || 
                player.Zone is not MapGeneration.FacilityZone.None ||
                !WaypointBase.TryGetWaypoint(id, out WaypointBase waypoint) ||
                waypoint is ElevatorWaypoint)
            {
                response = "失败，可能指令未启用或者身份不允许等";
                return false;
            }

            player.IsGodModeEnabled = true;
            try
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.Position = player.Role.GetRandomSpawnLocation();
                });

                Timing.CallDelayed(2f, () =>
                {
                    player.IsGodModeEnabled = false;
                });
            }
            catch(Exception ex)
            {
                player.IsGodModeEnabled = false;
                Log.Error(ex.ToString());

                response = "执行指令时发生错误，请稍后再试";
                return true;
            }

            response = "成功";
            return true;
        }
    }
}
