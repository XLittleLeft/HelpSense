using CommandSystem;
using HelpSense.API;
using HelpSense.API.Features.Pool;
using PluginAPI.Core;
using System;

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

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = "执行指令时发生错误，请稍后重试";
                return false;
            }

            if (player.DoNotTrack || !Plugin.Instance.Config.SavePlayersInfo)
            {
                response = "查询失败,请关闭DNT或服务器未启用此功能";
                return false;
            }

            var log = player.GetLog();
            var playedTimes = log.PlayedTimes;
            int hour = playedTimes / 3600;
            int day = hour / 24;
            int minutes = (playedTimes - hour * 3600) / 60;

            var kills = log.PlayerKills;
            var scpKills = log.PlayerSCPKills;
            var playerDamage = log.PlayerDamage;
            var rolePlayed = log.RolePlayed;
            var playerDeath = log.PlayerDeath;
            var shot = log.PlayerShot;

            var sb = StringBuilderPool.Pool.Get();
            sb.AppendLine("自从插件安装后你已在本服游玩了");
            sb.AppendLine($"<color=red>{day}</color>天<color=red>{hour}</color>小时<color=red>{minutes}</color>分钟");
            sb.AppendLine($"一共扮演了<color=red>{rolePlayed}</color>次角色");
            sb.AppendLine($"你一共击杀了<color=red>{kills}</color>个人<color=red>{scpKills}</color>个SCP | 一共造成了<color=red>{playerDamage}</color>点伤害(伤害包括无效的) | 一共死亡<color=red>{playerDeath}次</color>");
            sb.AppendLine($"一共开了<color=red>{shot}</color>枪");

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);

            return true;
        }
    }
}
