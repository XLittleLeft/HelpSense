using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.ConfigSystem
{
    public class CommandTranslateConfig
    {
        [Description("玩家信息指令_标题")]
        public string InfoCommandTitle { get; set; } = "自从插件安装后你已在本服游玩了";
        [Description("玩家信息指令_时间")]
        public string InfoCommandPlayedTime { get; set; } = "<color=red>%day%</color>天<color=red>%hour%</color>小时<color=red>%minutes%</color>分钟";
        [Description("玩家信息指令_角色")]
        public string InfoCommandRolePlayed { get; set; } = "一共扮演了<color=red>%rolePlayed%</color>次角色";
        [Description("玩家信息指令_击杀和伤害")]
        public string InfoCommandKillsDamages { get; set; } = "你一共击杀了<color=red>%kills%</color>个人<color=red>%scpKills%</color>个SCP | 一共造成了<color=red>%playerDamage%</color>点伤害(伤害包括无效的) | 一共死亡<color=red>%playerDeath%次</color>";
        [Description("玩家信息指令_射击")]
        public string InfoCommandShot { get; set; } = "一共开了<color=red>%shot%</color>枪";
        [Description("玩家信息指令_因DNT查询失败的消息")]
        public string InfoCommandFailed { get; set; } = "查询失败,请关闭DNT或服务器未启用此功能";
    }
}
