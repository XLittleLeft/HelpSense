using CommandSystem;
using HelpSense.API.Features.Pool;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VersionCommand : ICommand
    {
        public string Command => "Version";

        public string[] Aliases => new[]{"VersionInfo", "VI"};

        public string Description => "查询HelpSense插件版本和信息";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var config = Plugin.Instance.Config;
            
            var sb = StringBuilderPool.Pool.Get();

            //Version and update time
            sb.AppendFormat("当前插件版本为：{0}", Plugin.PluginVersion).AppendLine();
            sb.AppendFormat("最后更新时间：{0}", Plugin.LastUpdateTime).AppendLine();
            sb.AppendFormat("推荐游戏版本：{0}", Plugin.RequiredGameVersion).AppendLine();

            //Function list
            sb.AppendLine("功能开启列表(不是全部.只显示比较重要的):");
            sb.AppendFormat("非礼勿视机动特遣队:{0}", BoolTranslate(config.EnableSeeNoEvil)).AppendLine();
            sb.AppendFormat("天网机动特遣队:{0}", BoolTranslate(config.EnableSkynet)).AppendLine();
            sb.AppendFormat("SCP-029 暗影之女:{0}", BoolTranslate(config.EnableSCP029)).AppendLine();
            sb.AppendFormat("SCP-073 亚伯/亚当:{0}", BoolTranslate(config.SCP073)).AppendLine();
            sb.AppendFormat("SCP-191 机械少女:{0}", BoolTranslate(config.SCP191)).AppendLine();
            sb.AppendFormat("SCP-347 隐形女:{0}", BoolTranslate(config.SCP347)).AppendLine();
            sb.AppendFormat("SCP-703 壁橱之中:{0}", BoolTranslate(config.EnableSCP703)).AppendLine();
            sb.AppendFormat("SCP-1056 大小改变器: {0}", BoolTranslate(config.SCP1056)).AppendLine();
            sb.AppendFormat("SCP-1068 无害核弹:{0}", BoolTranslate(config.SCP1068)).AppendLine();
            sb.AppendFormat("混沌领导者:{0}", BoolTranslate(config.EnableChaosLeader)).AppendLine();
            sb.AppendFormat("SCP-2936-1 巨型德国机器人:{0}", BoolTranslate(config.SCP2936)).AppendLine();
            sb.AppendFormat("SCP-1093 灯人:{0}", BoolTranslate(config.SCP1093)).AppendLine();
            sb.AppendFormat("无限子弹:{0}", BoolTranslate(config.InfiniteAmmo)).AppendLine();
            sb.AppendFormat("无限子弹模式:{0}", Plugin.Instance.Config.InfiniteAmmoType).AppendLine();

            //Copyright
            sb.AppendLine("-Made By X小左-");
            sb.AppendLine("Copyright © X小左 2022-2024");

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);
            
            return true;
        }

        public string BoolTranslate(bool value) => value? "✔":"✖";
    }
}
