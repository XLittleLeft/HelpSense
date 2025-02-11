using CommandSystem;
using HelpSense.API;
using HelpSense.API.Features.Pool;
using HelpSense.API.Serialization;
using HelpSense.ConfigSystem;
using PlayerRoles;
using LabApi.Features.Wrappers;
using System;
using HelpSense.API.Events;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class InfoCommand : ICommand
    {
        public string Command => "Info";

        public string[] Aliases => ["info"];

        public string Description => "查询在本服务器游玩的时间和击杀人数";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;
            CommandTranslateConfig CommandTranslateConfig = CustomEventHandler.CommandTranslateConfig;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = CommandTranslateConfig.InfoCommandFailed;
                return false;
            }

            if (player.DoNotTrack || !CustomEventHandler.Config.SavePlayersInfo)
            {
                response = CommandTranslateConfig.InfoCommandFailed;
                return false;
            }

            PlayerLog log = player.GetLog();
            int playedTimes = log.PlayedTimes;
            int hour = playedTimes / 3600;
            int day = hour / 24;
            int minutes = (playedTimes - hour * 3600) / 60;

            int kills = log.PlayerKills;
            int scpKills = log.PlayerSCPKills;
            float playerDamage = log.PlayerDamage;
            int rolePlayed = log.RolePlayed;
            int playerDeath = log.PlayerDeath;
            var shot = log.PlayerShot;

            var sb = StringBuilderPool.Pool.Get();

            sb.AppendLine(CommandTranslateConfig.InfoCommandTitle);
            sb.AppendLine(CommandTranslateConfig.InfoCommandPlayedTime.Replace("%day%" , day.ToString()).Replace("%hour%", hour.ToString()).Replace("%minutes%", minutes.ToString()));
            sb.AppendLine(CommandTranslateConfig.InfoCommandRolePlayed.Replace("%rolePlayed%" , rolePlayed.ToString()));
            sb.AppendLine(CommandTranslateConfig.InfoCommandKillsDamages.Replace("%kills%" , kills.ToString()).Replace("%scpKills%", scpKills.ToString()).Replace("%playerDamage%", playerDamage.ToString()).Replace("%playerDeath%", playerDeath.ToString()));
            sb.AppendLine(CommandTranslateConfig.InfoCommandShot.Replace("%shot%" , shot.ToString()));

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);

            return true;
        }
    }
}
