using CommandSystem;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VersionCommand : ICommand
    {
        public string Command => "version";

        public string[] Aliases => new string[]{"versioninfo"};

        public string Description => "查询HelpSense插件版本和信息";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var Config = Plugin.Instance.Config;

            StringBuilder responseBuilder = new StringBuilder();

            responseBuilder.AppendLine("当前插件版本为：1.3.5");
            responseBuilder.AppendLine("最后更新时间：2024/09/24中午12:23");
            responseBuilder.AppendLine("游戏版本：13.5.1(有时候版本更新不太影响)");
            responseBuilder.AppendLine("功能开启列表(不是全部.只显示比较重要的):");
            responseBuilder.AppendLine($"非礼勿视机动特遣队: {BoolTranslate[Config.EnableSeeNoEvil]}");
            responseBuilder.AppendLine($"天网机动特遣队: {BoolTranslate[Config.EnableSkynet]}");
            responseBuilder.AppendLine($"SCP-029 暗影之女: {BoolTranslate[Config.EnableSCP029]}");
            responseBuilder.AppendLine($"SCP-073 亚伯/亚当: {BoolTranslate[Config.SCP073]}");
            responseBuilder.AppendLine($"SCP-191 机械少女: {BoolTranslate[Config.SCP191]}");
            responseBuilder.AppendLine($"SCP-347 隐形女: {BoolTranslate[Config.SCP347]}");
            responseBuilder.AppendLine($"SCP-703 壁橱之中: {BoolTranslate[Config.EnableSCP703]}");
            responseBuilder.AppendLine($"SCP-1056 大小改变器: {BoolTranslate[Config.SCP1056]}");
            responseBuilder.AppendLine($"SCP-1068 无害核弹: {BoolTranslate[Config.SCP1068]}");
            responseBuilder.AppendLine($"混沌领导者: {BoolTranslate[Config.EnableChaosLeader]}");
            responseBuilder.AppendLine($"SCP-2936-1 巨型德国机器人: {BoolTranslate[Config.SCP2936]}");
            responseBuilder.AppendLine($"SCP-1093 灯人: {BoolTranslate[Config.SCP1093]}");
            responseBuilder.AppendLine($"无限子弹: {BoolTranslate[Config.InfiniteAmmo]}");
            responseBuilder.AppendLine($"无限子弹模式: {Plugin.Instance.Config.InfiniteAmmoType}");
            responseBuilder.AppendLine("-Made By X小左-");
            responseBuilder.AppendLine("Copyright © X小左(XLittleLeft) 2022-2024");

            response = responseBuilder.ToString();
            return true;
        }

        public Dictionary<bool, string> BoolTranslate = new Dictionary<bool, string>()
        {
            {true , "✔" },
            {false , "✖" }
        };
    }
}
