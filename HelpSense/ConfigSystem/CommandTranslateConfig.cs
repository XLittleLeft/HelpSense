﻿using System;
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
        [Description("卡虚空自救指令_错误")]
        public string RescueCommandError { get; set; } = "执行指令时发生错误，请稍后再试";
        [Description("卡虚空自救指令_失败")]
        public string RescueCommandFailed { get; set; } = "失败，可能指令未启用或者身份不允许等";
        [Description("卡虚空自救指令_成功")]
        public string RescueCommandOk { get; set; } = "成功";
        [Description("聊天指令_错误")]
        public string ChatCommandError { get; set; } = "发送消息时出现错误，请稍后重试";
        [Description("聊天指令_失败")]
        public string ChatCommandFailed { get; set; } = "发送失败，你被禁言或者信息为空或者聊天系统未启用";
        [Description("聊天指令_成功")]
        public string ChatCommandOk { get; set; } = "发送成功";
        [Description("查询HelpSense插件版本和信息")] 
        public Dictionary<string,string> VersionCommand { get; set; } = new() { 
            { "PluginVersion", "当前插件版本为：{0}" },
            { "LastUpdateTime", "最后更新时间：{0}" }, 
            { "RequiredGameVersion", "推荐游戏版本：{0}" },
            { "Text", "功能开启列表(不是全部.只显示比较重要的):" },
            { "SeeNoEvil", "非礼勿视机动特遣队:{0}" },
            { "Skynet", "天网机动特遣队:{0}" },
            { "SCP023", "SCP-023 黑煞星:{0}" },
            { "SCP029", "SCP-029 暗影之女:{0}" },
            { "SCP073", "SCP-073 亚伯/亚当:{0}" },
            { "SCP191", "SCP-191 机械少女:{0}" },
            { "SCP347", "SCP-347 隐形女:{0}" },
            { "SCP703", "SCP-703 壁橱之中:{0}" },
            { "SCP1056", "SCP-1056 大小改变器: {0}" },
            { "SCP1068", "SCP-1068 无害核弹:{0}" },
            { "ChaosLeader", "混沌领导者:{0}" },
            { "SCP2936", "SCP-2936-1 巨型德国机器人:{0}" },
            { "SCP1093", "SCP-1093 灯人:{0}" },
            { "InfiniteAmmo", "无限子弹:{0}" },
            { "InfiniteAmmoType", "无限子弹模式:{0}" }
        };   
    }
}
