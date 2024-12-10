using HelpSense.Helper.Chat;
using PlayerRoles;
using System.Collections.Generic;
using System.ComponentModel;

namespace HelpSense.ConfigSystem
{
    public class TranslateConfig
    {
        [Description("当玩家进入服务器的欢迎消息")]
        public string WelcomeMessage { get; set; } = "欢迎 %playername%~";
        [Description("回合结束消息")]
        public string RoundEndInfo { get; set; } = "回合结束啦！不要离开，下局再见ヾ(￣▽￣)~";
        [Description("友伤开启时玩家将收到的消息")]
        public string FFMessage { get; set; } = "<align=center><voffset=28em> <color=#F6511D> ~友伤已开启~ </color></voffset></align>";
        [Description("服务器广播文本")]
        public string AutoServerMessageText { get; set; } = "服务器广播";
        /// <summary>
        /// 
        /// </summary>
        [Description("聊天中消息列表的标题")]
        public string ChatMessageTitle { get; set; } = "消息列表:";

        [Description("聊天中每种消息的名字")]
        public Dictionary<ChatMessage.MessageType, string> MessageTypeName { get; set; } = new()
        {
            { ChatMessage.MessageType.AdminPrivateChat, "管理私聊" },
            { ChatMessage.MessageType.BroadcastChat, "公共消息" },
            { ChatMessage.MessageType.TeamChat, "队友消息" },
        };
        /// <summary>
        /// 
        /// </summary>
        [Description("聊天系统自定义玩家角色名称")]
        public Dictionary<RoleTypeId, string> ChatSystemRoleTranslation { get; set; } = new Dictionary<RoleTypeId, string>
        {
            {RoleTypeId.ClassD , "D级人员"},
            {RoleTypeId.FacilityGuard , "保安" },
            {RoleTypeId.ChaosConscript , "混沌征召兵"},
            {RoleTypeId.ChaosMarauder , "混沌掠夺者"},
            {RoleTypeId.ChaosRepressor , "混沌压制者"},
            {RoleTypeId.ChaosRifleman , "混沌步枪兵"},
            {RoleTypeId.NtfCaptain , "九尾狐指挥官"},
            {RoleTypeId.NtfPrivate , "九尾狐列兵"},
            {RoleTypeId.NtfSergeant , "九尾狐中士"},
            {RoleTypeId.NtfSpecialist , "九尾狐收容专家"},
            {RoleTypeId.Scientist , "科学家"},
            {RoleTypeId.Tutorial , "教程角色"},
            {RoleTypeId.Scp096 , "SCP-0肘6" },
            {RoleTypeId.Scp049 , "SCP-049" },
            {RoleTypeId.Scp173 , "SCP-173" },
            {RoleTypeId.Scp939 , "SCP-肘3肘" },
            {RoleTypeId.Scp079 , "SCP-07肘" },
            {RoleTypeId.Scp0492 , "SCP-049-2" },
            {RoleTypeId.Scp106 , "SCP-106" },
            {RoleTypeId.Scp3114 , "SCP-3114" },
            {RoleTypeId.Spectator , "观察者" },
            {RoleTypeId.Overwatch , "监管" },
            {RoleTypeId.Filmmaker , "导演" }
        };
        [Description("聊天系统自定义玩家团队名称")]
        public Dictionary<Team, string> ChatSystemTeamTranslation { get; set; } = new Dictionary<Team, string>
        {
            {Team.Dead , "死亡阵营" },
            {Team.ClassD , "DD阵营" },
            {Team.OtherAlive , "神秘阵营" },
            {Team.Scientists , "博士阵营" },
            {Team.SCPs , "SCP阵营" },
            {Team.ChaosInsurgency , "混沌阵营" },
            {Team.FoundationForces , "九尾狐阵营" },
        };
        /// <summary>
        /// 
        /// </summary>
        [Description("管理员监察输出文本")]
        public string AdminLog { get; set; } = "%Nickname%在%Time%时候 | 使用了指令:%Command% | Steam64ID:%UserId%";
        [Description("管理员监察指令广播")]
        public string AdminLogBroadcast { get; set; } = "<size=30>[<color=red>管理行为监察输出记录</color>] <%Nickname%> 在刚刚使用了指令: %Command%</size>";
        /// <summary>
        /// 
        /// </summary>
        [Description("显示距离启动还剩多少时间和服务器状态")]
        public string TitleText { get; set; } = "<size=50><color=#F0FF00><b>正在等待玩家, {seconds}</b></color></size>";
        [Description("显示玩家数量的文本")]
        public string PlayerCountText { get; set; } = "<size=40><color=#FFA600><i>{players}</i></color></size>";

        [Description("如果大厅被锁定显示的消息")]
        public string ServerPauseText { get; set; } = "服务器已暂停";

        [Description("第二行的消息")]
        public string SecondLeftText { get; set; } = "还剩 {seconds}";

        [Description("多行的消息")]
        public string SecondsLeftText { get; set; } = "还剩 {seconds}";

        [Description("回合开始时显示的消息")]
        public string RoundStartText { get; set; } = "回合开始";

        [Description("当服务器当玩家加入时显示的消息")]
        public string PlayerJoinText { get; set; } = "玩家加入";

        [Description("当服务器上有多个玩家加入时显示的消息")]
        public string PlayersJoinText { get; set; } = "名玩家加入";
        /// <summary>
        /// 
        /// </summary>
        [Description("警卫叛变显示的公告")]
        public string GuardMutinyBroadcast { get; set; } = "<size=60><color=#ff0000ff>[警告]</color>此设施警卫已被混沌分裂者策反</size>";
        [Description("警卫为精英队显示的公告")]
        public string EliteGuardBroadcast { get; set; } = "<size=60><color=#00ffffff>[通知]</color>此设施警卫为九尾狐精英队员，保安实力大增</size>";
        /// <summary>
        /// 
        /// </summary>
        [Description("观战列表标题")]
        public string WatchListTitle { get; set; } = "<align=right><size=45%><color=(COLOR)><b>观察者 ((COUNT)):</b></color></size></align>";
        [Description("观战列表格式")]
        public string Names { get; set; } = "<align=right><size=45%><color=(COLOR)><br>(NAME)</color></size></align>";
        /// <summary>
        /// 
        /// </summary>
        [Description("DNT提醒")]
        public string DNTWarning { get; set; } = "你打开了DNT，请关闭，否则某些插件无法正常运行";
        /// <summary>
        /// 
        /// </summary>
        [Description("SCP-073反伤原因")]
        public string SCP073DamageReason { get; set; } = "SCP073反伤";
        [Description("SCP-029逃离设施Hint")]
        public string SCP029EscapeHint { get; set; } = "成功逃离设施变为混沌得到混沌分裂者的一个遗产";
        [Description("SCP-703逃离设施Hint")]
        public string SCP703EscapeHint { get; set; } = "成功逃离设施成为九尾狐收容专家，获得3x";
        /// <summary>
        /// 
        /// </summary>
        [Description("天网机动特遣队Cassie广播")]
        public string SkynetCassie { get; set; } = "机动特遣队Kappa-10和Mu-7代号天网已经进入设施,他们会帮助舞步者一号收容SCP-079,建议所有幸存人员执行标准撤离方案,直到MTF小队到达你的地点,目前还剩%SCPNum%个SCP";
        [Description("天网机动特遣队技能描述")]
        public List<string> SkynetSkillIntroduction { get; set; } = new List<string>()
        {
            "你是 [天网机动特遣队队员] 珍惜手中的鬼灯",
            "你启动的发电机会<color=red>很快激活</color>"
        };
        [Description("天网机动特遣队新兵广播")]
        public string SkynetPrivateBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-天网 新兵</color></size>";
        [Description("天网机动特遣队新兵自定义信息")]
        public string SkynetPrivateCustomInfo { get; set; } = "天网 新兵";
        [Description("天网机动特遣队中士广播")]
        public string SkynetSergeantBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-天网 中士</color></size>";
        [Description("天网机动特遣队新兵自定义信息")]
        public string SkynetSergeantCustomInfo { get; set; } = "天网 中士";
        [Description("天网机动特遣队指挥官广播")]
        public string SkynetCaptainBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-天网 指挥官</color></size>";
        [Description("天网机动特遣队指挥官自定义信息")]
        public string SkynetCaptainCustomInfo { get; set; } = "天网 指挥官";
        [Description("非礼勿视机动特遣队Cassie广播")]
        public string SeeNoEvilCassie { get; set; } = "机动特遣队Eta-10代号非礼勿视已经进入设施,他们会帮助收容SCP-096,建议所有幸存人员执行标准撤离方案,直到MTF小队到达你的地点,目前还剩%SCPNum%个SCP";
        [Description("非礼勿视机动特遣队技能描述")]
        public List<string> SeeNoEvilSkillIntroduction { get; set; } = new List<string>()
        {
            "你是 [非礼勿视机动特遣队队员] 大胆盯着SCP096吧",
            "他<color=red>很难</color>杀死你,在他还没狂暴的时候你可以随意盯着他"
        };
        [Description("非礼勿视机动特遣队新兵广播")]
        public string SeeNoEvilPrivateBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-非礼勿视 新兵</color></size>";
        [Description("非礼勿视机动特遣队新兵自定义信息")]
        public string SeeNoEvilPrivateCustomInfo { get; set; } = "非礼勿视 新兵";
        [Description("非礼勿视机动特遣队中士广播")]
        public string SeeNoEvilSergeantBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-非礼勿视 中士</color></size>";
        [Description("非礼勿视机动特遣队新兵自定义信息")]
        public string SeeNoEvilSergeantCustomInfo { get; set; } = "非礼勿视 中士";
        [Description("非礼勿视机动特遣队指挥官广播")]
        public string SeeNoEvilCaptainBroadcast { get; set; } = "<size=70><color=#0051FF>你是机动特遣队-非礼勿视 指挥官</color></size>";
        [Description("非礼勿视机动特遣队指挥官自定义信息")]
        public string SeeNoEvilCaptainCustomInfo { get; set; } = "非礼勿视 指挥官";
        /// <summary>
        /// 
        /// </summary>
        [Description("特殊角色被收容Cassie后缀")]
        public string SpecialRoleContainCassie { get; set; } = "成功被消灭，具体原因未知";
        /// <summary>
        /// 
        /// </summary>
        [Description("混沌领导者名字")]
        public string ChaosLeaderRoleName { get; set; } = "混沌领导者";
        [Description("混沌领导者刷新广播")]
        public string ChaosLeaderSpawnBroadcast { get; set; } = "<size=80><color=#00ff00ff>你是混沌分裂者 领导者</color></size>";
        [Description("混沌领导者死亡Cassie")]
        public string ChaosLeaderDeathCassie { get; set; } = "混沌领导者死亡";
        [Description("SCP-2936-1刷新广播")]
        public string SCP29361SpawnBroadcast { get; set; } = "<size=70>你是 <color=red>SCP-2936-1 巨型德国机器人</color></size>";
        [Description("SCP-2936-1技能介绍")]
        public string SCP29361SkillIntroduction { get; set; } = "你是 [SCP-2936-1] 你拥有庞大的身躯和厚实的血量";
        [Description("SCP-073亚伯刷新广播")]
        public string SCP073AbelSpawnBroadcast { get; set; } = "你是<color=green>SCP-073 亚伯</color>";
        [Description("SCP-073亚伯技能介绍")]
        public string SCP073AbelSkillIntroduction { get; set; } = "你是[SCP-073] 你将被动的<color=red>反弹</color>部分伤害给敌人";
        [Description("SCP-073该隐刷新广播")]
        public string SCP073CainSpawnBroadcast { get; set; } = "你是<color=green>SCP-073 该隐</color>";
        [Description("SCP-073该隐技能介绍")]
        public string SCP073CainSkillIntroduction { get; set; } = "你是[SCP-073] 你有<color=red>无限</color>的自愈能力,停下来休息一下吧";
        [Description("SCP-703刷新广播")]
        public string SCP703SpawnBroadcast { get; set; } = "<size=80><color=#00ffffff>你是SCP-703</color></size>";
        [Description("SCP-703技能介绍")]
        public List<string> SCP703SkillIntroduction { get; set; } = new List<string>()
        {
            "你是 [SCP-703] 你每过一段时间你会获得随机物品",
            "距离下次获得物品<color=red>%Time%</color>"
        };
        [Description("SCP-703获得物品Hint提示")]
        public string SCP703ReceivedItemHint { get; set; } = "获得一件物品";
        [Description("SCP-029刷新广播")]
        public string SCP029SpawnBroadcast { get; set; } = "<size=60><color=#ff0000ff>你是SCP-029“暗影之女”</color></size>";
        [Description("SCP-029技能介绍")]
        public string SCP029SkillIntroduction { get; set; } = "你是[SCP-029] 你拥有<color=red>无限</color>的速度加成";
        [Description("SCP-347刷新广播")]
        public string SCP347SpawnBroadcast { get; set; } = "你是<color=red>SCP-347 隐形女</color>";
        [Description("SCP-347技能介绍")]
        public string SCP347SkillIntroduction { get; set; } = "你是[SCP-347] 你<color=red>永远</color>是隐身的,但不要乱动";
        [Description("SCP-1093刷新广播")]
        public string SCP1093SpawnBroadcast { get; set; } = "你是 <color=yellow>SCP-1093 人灯</color>";
        [Description("SCP-1093技能介绍")]
        public List<string> SCP1093SkillIntroduction { get; set; } = new List<string>()
        {
            "你是 [SCP-1093] 持续照亮附近5米范围,并辐射附近1米范围内的人",
            "你的头是虚无的,任何人对你头部没有伤害"
        };
        [Description("警卫队长刷新广播")]
        public string GuardCaptainSpawnBroadcast { get; set; } = "<size=60><color=#E5DADA>你是安保队长</color></size>";
        [Description("SCP-191刷新广播")]
        public string SCP191SpawnBroadcast { get; set; } = "你成为了<color=red>SCP-191 机械少女</color>";
        [Description("SCP-191技能介绍")]
        public List<string> SCP191SkillIntroduction { get; set; } = new List<string>()
        {
            "你是 [SCP-191] 因为你的身体的改造",
            "你对除了电磁和爆炸伤害的<color=red>抗性很高</color>",
            "但别忘了去079收容室充电"
        };
        [Description("SCP-191电量显示")]
        public string SCP191BatteryHintShow { get; set; } = "<align=right><size=40><b>你目前剩余的电量:<color=yellow>%Battery%安</color></size></b></align>";
        [Description("SCP-191电量耗尽死亡原因")]
        public string SCP191BatteryDepletionDeathReason { get; set; } = "电量耗尽";
        /// <summary>
        /// 
        /// </summary>
        [Description("SCP-1056缩小仪拾取Hint")]
        public string SCP1056PickupHint { get; set; } = "你捡起了<color=red>SCP-1056</color> 缩小仪!";
        [Description("SCP-1056缩小仪使用Hint")]
        public string SCP1056UsedHint { get; set; } = "boom!你变小了!!!";
        [Description("SCP-1068无害核弹拾取Hint")]
        public string SCP1068PickupHint { get; set; } = "你捡起了<color=red>SCP-1068</color> 无害核弹!";
        [Description("SCP-1056无害核弹使用广播")]
        public string SCP1068UsedBroadcast { get; set; } = "有人使用了<color=red>SCP-1068</color> 无害核弹!";
        /// <summary>
        /// 
        /// </summary>
        [Description("SCP-029特殊介绍")]
        public string SCP029SpecialIntroduction { get; set; } = "<color=red>SCP-029 暗影之女</color>";
        [Description("SCP-703特殊介绍")]
        public string SCP703SpecialIntroduction { get; set; } = "<color=blue>SCP-703 壁橱之中</color>";
        [Description("SCP-347特殊介绍")]
        public string SCP347SpecialIntroduction { get; set; } = "<color=red>SCP-347 隐形女</color>";
        [Description("SCP-1093特殊介绍")]
        public string SCP1093SpecialIntroduction { get; set; } = "<color=yellow>SCP-1093 人灯</color>";
        [Description("SCP-073亚伯特殊介绍")]
        public string SCP073AbelSpecialIntroduction { get; set; } = "<color=green>SCP-073 亚伯</color>";
        [Description("SCP-073该隐特殊介绍")]
        public string SCP073CainSpecialIntroduction { get; set; } = "<color=green>SCP-073 该隐</color>";
        [Description("SCP-191特殊介绍")]
        public string SCP191SpecialIntroduction { get; set; } = "<color=red>SCP-191 机械少女</color>";
        [Description("SCP-2936特殊介绍")]
        public string SCP2936SpecialIntroduction { get; set; } = "<color=red>SCP-2936-1 巨型德国机器人</color>";
        [Description("天网机动特遣队特殊介绍")]
        public string SkynetSpecialIntroduction { get; set; } = "<color=blue>天网 机动特遣队</color>";
        [Description("非礼勿视机动特遣队特殊介绍")]
        public string SeeNoEvilSpecialIntroduction { get; set; } = "<color=blue>非礼勿视 机动特遣队</color>";
    }
}
