using System.ComponentModel;
using PlayerRoles;
using System.Collections.Generic;
using HelpSense.Helper;
using Org.BouncyCastle.Asn1.Mozilla;
using PluginAPI.Helpers;
using System.IO;
using System;

namespace HelpSense.ConfigSystem
{
    public class Config
    {
        [Description("Debug")]
        public bool Debug { get; set; } = true;
        [Description("玩家信息储存")]
        public bool SavePlayersInfo { get; set; } = true;
        [Description("保存位置")]
        public string SavePath { get; set; } = Path.Combine(Paths.Configs ?? Environment.CurrentDirectory, @"HelpSense_玩家信息储存.Database");
        [Description("是否打开开局给D级人员一张卡")]
        public bool EnableRoundSupplies { get; set; } = true;
        [Description("给D级人员什么卡？")]
        public ItemType ClassDCard { get; set; } = ItemType.KeycardJanitor;

        // //////////////////////////////////
        [Description("给九尾指挥官刷电磁炮")]
        public bool SpawnHID { get; set; } = true;

        // //////////////////////////////////
        [Description("是否打开入服欢迎")]
        public bool EnableWelcomeInfo { get; set; } = true;
        [Description("欢迎消息显示时长")]
        public ushort WelcomeTime { get; set; } = 5;

        // /////////////////////////////////////////////////
        [Description("是否打开回合结束消息")]
        public bool EnableRoundEndInfo { get; set; } = true;

        // ///////////////////////////////////
        [Description("开启回合结束时开友伤")]
        public bool EnableFriendlyFire { get; set; } = true;
        [Description("覆盖友伤系数数值")]
        public float OverriddenFriendlyFireMultiplier { get; set; } = 1f;
        [Description("通知玩家的消息类型，广播/提示")]
        public MessageType FriendlyFireNotifyingType { get; set; } = MessageType.Hint;
        [Description("显示友伤消息多长时间")]
        public float FFMessageDuration { get; set; } = 0.3f;
        [Description("回合结束把所有人变成同一阵营的人一起互相van♂")]
        public bool EnableFFRoundEndRoleChange { get; set; } = true;
        [Description("回合结束所有人变成的角色")]
        public RoleTypeId FFRoundEndRole { get; set; } = RoleTypeId.NtfCaptain;

        // /////////////////////////////////////////////////
        [Description("开启开局所有人都变成教程角色在一起van")]
        public bool EnableRoundWaitingLobby { get; set; } = true;

        [Description("大厅速度加成")]
        public byte MovementBoostIntensity { get; set; } = 50;

        [Description("在广播室显示文本")]
        public bool DisplayInIcom { get; set; } = true;

        [Description("在广播室的字体大小")]
        public int IcomTextSize { get; set; } = 40;

        [Description("人们将在大厅中变成的角色")]
        public RoleTypeId LobbyPlayerRole { get; set; } = RoleTypeId.Tutorial;
        [Description("大厅物品")]
        public HashSet<ItemType> LobbyInventory { get; set; } = new HashSet<ItemType>()
        {
            ItemType.Flashlight,
        };

        [Description("生成在哪（一个就是固定位置，俩就是随机刷在哪里）")]
        public List<LobbyLocationType> LobbyLocation { get; set; } = new List<LobbyLocationType>()
        {
            LobbyLocationType.Tower,
            LobbyLocationType.Intercom,
            LobbyLocationType.Mountain,
            LobbyLocationType.Chaos
        };
        [Description("练枪大厅")]
        public bool PracticeHall { get; set; } = true;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("无限子弹")]
        public bool InfiniteAmmo { get; set; } = true;
        [Description("无限子弹模式（0->Old：给玩家加备用子弹来装填；1->Normal：（可能有点问题）正常模式，有换弹动作，不需要子弹换弹；2->Moment：瞬间换弹，无换弹动作，不需要子弹换弹；3->Infinite：真·无限子弹，不需要换弹）")]
        public InfiniteAmmoType InfiniteAmmoType { get; set; } = InfiniteAmmoType.Old;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用修改SCP血量系统")]
        public bool EnableChangeSCPHPSystem { get; set; } = true;
        [Description("SCP173,SCP939,SCP049,SCP049-2,SCP096,SCP106血量")]
        public List<float> SCPsHP { get; set; } = new List<float> { 4200,2700,2200,400,2500,2200};

        // /////////////////////////////////////////////////
        [Description("启用.bc和.c聊天系统")]
        public bool EnableChatSystem { get; set; } = true;
        [Description("聊天系统大小，默认30")]
        public int ChatSystemSize { get; set; } = 30;

        [Description("启用玩家反馈管理系统")]
        public bool EnableAcSystem { get; set; } = true;
        [Description("一个人的聊天信息显示多久，单位为秒")]
        public int MessageTime { get; set; } = 10;

        [Description("聊天消息的格式，可用的标签为{Message}，{MessageType}, {MessageTypeColor}, {SenderNickname}，{SenderTeam}，{SenderTeamColor}, {CountDown}")]
        public string MessageTemplate { get; set; } = "[{CountDown}]<color={{SenderTeamColor}}>[{SenderTeam}]</color><color={MessageTypeColor}>[{MessageType}]</color>{SenderNickname}: {Message}";

        // /////////////////////////////////////////////////
        [Description("启用懒人系统")]
        public bool EnableLazySystem { get; set; } = true;
        [Description("对于SCP的门也有效")]
        public bool AffectScpLockers { get; set; } = true;
        [Description("对柜子也有效")]
        public bool AffectLockers { get; set; } = true;
        [Description("对电板也有效")]
        public bool AffectGenerators { get; set; } = true;
        [Description("如果玩家在狗子的屁里不起作用")]
        public bool AffectAmnesia { get; set; } = true;

        [Description("对正常的门有效")]
        public bool AffectDoors { get; set; } = true;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("SCP-703")]
        public bool EnableSCP703 { get; set; } = true;
        [Description("多久给703一次物品")]
        public int SCP703ItemTIme { get; set; } = 4;
        [Description("SCP-029")]
        public bool EnableSCP029 { get; set; } = true;
        [Description("混沌领导者")]
        public bool EnableChaosLeader { get; set; } = true;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用回合刷新时间显示")]
        public bool EnableRespawnTimer { get; set; } = true;
        [Description("列出将要用的时间名字")]
        public List<string> Timers { get; private set; } = new List<string>
        {
            "TimerList"
        };
        [Description("在回合结束时候重新刷新时间")]
        public bool ReloadTimerEachRound { get; private set; } = true;
        [Description("当玩家是Overwatch时候隐藏")]
        public bool HideTimerForOverwatch { get; private set; } = false;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用多变保安队")]
        public bool EnableBaoAn { get; set; } = true;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用服务器定时广播")]
        public bool EnableAutoServerMessage { get; set; } = true;
        [Description("服务器定时广播多久一次")]
        public int AutoServerMessageTime { get; set; } = 5;
        [Description("服务器定时广播显示多久")]
        public ushort AutoServerMessageTimer { get; set; } = 5;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用观战列表")]
        public bool EnableSpectatorList { get; set; } = false;
        [Description("观战列表忽略身份")]
        public HashSet<string> IgnoredRoles { get; set; } = new HashSet<string>();
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("启用天网特遣队")]
        public bool EnableSkynet { get; set; } = true;
        [Description("启动非礼勿视特遣队")]
        public bool EnableSeeNoEvil { get; set; } = true;
        [Description("非礼勿视刷新几率")]
        public int SeeNoEvilPer { get; set; } = 80;
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        [Description("SCP-1068")]
        public bool SCP1068 { get; set; } = true;
        [Description("SCP-1056")]
        public bool SCP1056 { get; set; } = true;
        [Description("SCP-1056缩小倍数")]
        public float SCP1056X { get; set; } = 0.8f;
        [Description("广播管理员干的事")]
        public bool AdminLogShow { get; set; } = false;
        [Description("记录管理日志地址")]
        public string AdminLogPath { get; set; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\管理记录.txt";
        [Description("隐藏教程角色")]
        public bool HideTutorials { get; set; } = false;
        [Description("自救指令")]
        public bool ZiJiuCommand { get; set; } = true;
        [Description("SCP-191")]
        public bool SCP191 { get;set; } = true;
        [Description("子弹对SCP-191造成的单发伤害")]
        public float SCP191Ammo { get; set; } = 10;
        [Description("SCP-073")]
        public bool SCP073 { get; set; } = true;
        [Description("SCP-073伤害减免％")]
        public byte SCP073DD { get; set; } = 50;
        [Description("SCP-073人类反弹伤害")]
        public float SCP073RRD { get; set; } = 5;
        [Description("SCP-073SCP反弹伤害")]
        public float SCP073SCPRD { get; set; } = 50;
        [Description("SCP-347")]
        public bool SCP347 { get; set; } = true;
        [Description("禁止SCP-347拿投掷物")]
        public bool NoT347 { get; set; } = false;
        [Description("SCP-2936")]
        public bool SCP2936 { get; set; } = true;
        [Description("SCP-1093")]
        public bool SCP1093 { get; set; } = true;
    }
    public enum MessageType
    {
        Broadcast,
        Hint
    };
    public enum InfiniteAmmoType
    {
        Old,
        Normal,
        Moment,
        Infinite
    }
}
