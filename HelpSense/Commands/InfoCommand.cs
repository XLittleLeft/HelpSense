using CommandSystem;
using HelpSense.API;
using HelpSense.API.Features.Pool;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class InfoCommand : ICommand
    {
        public string Command => "Info";

        public string[] Aliases => new[] { "info" };

        public string Description => "查询在本服务器游玩的时间和击杀人数";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var player = Player.Get(sender);

            if (player is null || player.DoNotTrack || !Plugin.Instance.Config.SavePlayersInfo)
            {
                response = "查询失败,请关闭DNT或服务器未启用此功能";
                return false;
            }

            var Log = player.GetLog();
            var PlayedTimes = Log.PlayedTimes;
            int hour = PlayedTimes / 3600;
            int day = hour / 24;
            int minutes = (PlayedTimes - hour * 3600) / 60;

            var kills = Log.PlayerKills;
            var scpkills = Log.PlayerSCPKills;
            var playerdamage = Log.PlayerDamage;
            var roleplayed = Log.RolePlayed;
            var playerdeath = Log.PlayerDeath;
            var shot = Log.PlayerShot;

            var sb = StringBuilderPool.Pool.Get();
            sb.AppendLine("自从插件安装后你已在本服游玩了");
            sb.AppendLine($"<color=red>{day}</color>天<color=red>{hour}</color>小时<color=red>{minutes}</color>分钟");
            sb.AppendLine($"\n一共扮演了<color=red>{roleplayed}</color>次角色");
            sb.AppendLine($"\n你一共击杀了<color=red>{kills}</color>个人<color=red>{scpkills}</color>个SCP | 一共造成了<color=red>{playerdamage}</color>点伤害(伤害包括无效的) | 一共死亡<color=red>{playerdeath}次</color>");
            sb.AppendLine($"\n一共开了<color=red>{shot}</color>枪");

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);

            return true;
        }
    }
}
