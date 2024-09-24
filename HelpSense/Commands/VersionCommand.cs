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

            response = $"当前插件版本为：1.3.4\n最后更新时间：2024/08/07上午8:08\n游戏版本：13.5.1(有时候版本更新不太影响)\n功能开启列表(不是全部.只显示比较重要的):\n非礼勿视机动特遣队:{BoolTranslate[Config.EnableSeeNoEvil]}\n天网机动特遣队:{BoolTranslate[Config.EnableSkynet]}\nSCP-029 暗影之女:{BoolTranslate[Config.EnableSCP029]}\nSCP-073 亚伯/亚当:{BoolTranslate[Config.SCP073]}\nSCP-191 机械少女:{BoolTranslate[Config.SCP191]}\nSCP-347 隐形女:{BoolTranslate[Config.SCP347]}\nSCP-703 壁橱之中:{BoolTranslate[Config.EnableSCP703]}\nSCP-1056 大小改变器:{BoolTranslate[Config.SCP1056]}\nSCP-1068 无害核弹:{BoolTranslate[Config.SCP1068]}\n混沌领导者:{BoolTranslate[Config.EnableChaosLeader]}\nSCP-2936-1 巨型德国机器人:{BoolTranslate[Config.SCP2936]}\nSCP-1093 灯人:{BoolTranslate[Config.SCP1093]}\n无限子弹:{BoolTranslate[Config.InfiniteAmmo]}\n无限子弹模式:{Plugin.Instance.Config.InfiniteAmmoType}\n-Made By X小左-\nCopyright © X小左 2022-2024";
            return true;
        }

        public Dictionary<bool, string> BoolTranslate = new Dictionary<bool, string>()
        {
            {true , "✔" },
            {false , "✖" }
        };
    }
}
