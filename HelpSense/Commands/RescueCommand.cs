using CommandSystem;
using HelpSense.ConfigSystem;
using HelpSense.Helper;
using MEC;
using PluginAPI.Core;
using RelativePositioning;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class RescueCommand : ICommand
    {
        public string Command => "ZiJiu";

        public string[] Aliases => ["Rescue", "ZJ"];

        public string Description => "卡虚空自救-AntiVoid";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            CommandTranslateConfig CommandTranslateConfig = Plugin.Instance.CommandTranslateConfig;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = CommandTranslateConfig.RescueCommandError;
                return false;
            }

            WaypointBase.GetRelativePosition(player.Position, out byte id, out _);

            if (!player.IsAlive ||
                player.Zone is not MapGeneration.FacilityZone.None ||
                !WaypointBase.TryGetWaypoint(id, out WaypointBase waypoint) ||
                waypoint is ElevatorWaypoint)
            {
                response = CommandTranslateConfig.RescueCommandFailed;
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
            catch (Exception ex)
            {
                player.IsGodModeEnabled = false;
                Log.Error(ex.ToString());

                response = CommandTranslateConfig.RescueCommandError;
                return true;
            }

            response = CommandTranslateConfig.RescueCommandOk;
            return true;
        }
    }
}
