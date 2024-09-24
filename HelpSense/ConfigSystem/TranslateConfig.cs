using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [Description("SCP-073反伤伤害原因")]
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
        [Description("SCP-073亚伯刷新广播")]
        public string SCP073AbelSpawnBroadcast { get; set; } = "你是<color=green>SCP-073 亚伯</color>";
        [Description("SCP-073刷新广播")]
        public string SCP073CainSpawnBroadcast { get; set; } = "你是<color=green>SCP-073 该隐</color>";
        [Description("SCP-703刷新广播")]
        public string SCP703SpawnBroadcast { get; set; } = "<size=80><color=#00ffffff>你是SCP-703</color></size>";
        [Description("SCP-703技能介绍")]
        public string SCP703SkillIntroduction { get; set; } = "<align=center><voffset=28em><size=70><color=#00ffffff>每过一段时间你会获得随机物品</color></size></voffset></align>";
        [Description("SCP-029刷新广播")]
        public string SCP029SpawnBroadcast { get; set; } = "<size=60><color=#ff0000ff>你是SCP-029“暗影之女”</color></size>";
        [Description("SCP-029技能介绍")]
        public string SCP029SkillIntroduction { get; set; } = "<voffset=28em><size=40><color=#ff0000ff>杀戮所有你敌对的人</color></size></voffset>";
        [Description("SCP-347刷新广播")]
        public string SCP347SpawnBroadcast { get; set; } = "你是<color=red>SCP-347 隐形女</color>";
        [Description("SCP-1093刷新广播")]
        public string SCP1093SpawnBroadcast { get; set; } = "你是 <color=yellow>SCP-1093 人灯</color>";
        [Description("SCP-1093技能介绍")]
        public string SCP1093SkillIntroduction { get; set; } = "持续照亮附近5米范围,并影响附近1米范围内的人受到辐射伤害(每秒扣1血)\n你的头似乎是虚无的,所以任何人对你头部是没有伤害的";
        [Description("警卫队长刷新广播")]
        public string GuardCaptainSpawnBroadcast { get; set; } = "<size=60><color=#E5DADA>你是安保队长</color></size>";
        [Description("SCP-191刷新广播")]
        public string SCP191SpawnBroadcast { get; set; } = "你成为了<color=red>SCP-191 机械少女</color>";
        [Description("SCP-191技能介绍")]
        public string SCP191SkillIntroduction { get; set; } = "因为你的身体的改造，你对除了电磁和爆炸伤害的抗性很高";
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
    }
}
