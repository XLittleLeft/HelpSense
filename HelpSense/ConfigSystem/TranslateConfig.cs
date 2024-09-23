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
        [Description("保安叛变显示的公告")]
        public string BaoAnPB { get; set; } = "<size=60><color=#ff0000ff>[警告]</color>此设施警卫已被混沌分裂者策反</size>";
        [Description("保安为精英队显示的公告")]
        public string BaoAnJY { get; set; } = "<size=60><color=#00ffffff>[通知]</color>此设施警卫为九尾狐精英队员，保安实力大增</size>";
        [Description("观战列表标题")]
        public string Title { get; set; } = "<align=right><size=45%><color=(COLOR)><b>观察者 ((COUNT)):</b></color></size></align>";

        [Description("观战列表格式")]
        public string Names { get; set; } = "<align=right><size=45%><color=(COLOR)><br>(NAME)</color></size></align>";
        [Description("DNT提醒")]
        public string DNTWarning { get; set; } = "你打开了DNT，请关闭，否则某些插件无法正常运行";
        [Description("SCP-073反伤伤害原因")]
        public string SCP073DamageReason { get; set; } = "SCP073反伤";
        [Description("SCP-029逃离设施Hint")]
        public string SCP029EscapeHint { get; set; } = "成功逃离设施变为混沌得到混沌分裂者的一个遗产";
        [Description("SCP-703逃离设施Hint")]
        public string SCP703EscapeHint { get; set; } = "成功逃离设施成为九尾狐收容专家，获得3x";
        [Description("天网机动特遣队Cassie广播")]
        public string SkynetCassie { get; set; } = "机动特遣队Kappa-10和Mu-7代号天网已经进入设施,他们会帮助舞步者一号收容SCP-079,建议所有幸存人员执行标准撤离方案,直到MTF小队到达你的地点,目前还剩%SCPNum%个SCP";
        [Description("天网激动特遣队新兵广播")]
        public string SkynetPrivateBroadcast { get; set; } = "你是机动特遣队-天网 新兵";
        [Description("天网激动特遣队新兵自定义信息")]
        public string SkynetPrivateCustomInfo { get; set; } = "天网 新兵";
        [Description("天网激动特遣队中士广播")]
        public string SkynetSergeantBroadcast { get; set; } = "你是机动特遣队-天网 中士";
        [Description("天网激动特遣队新兵自定义信息")]
        public string SkynetSergeantCustomInfo { get; set; } = "天网 中士";
        [Description("天网激动特遣队指挥官广播")]
        public string SkynetCaptainBroadcast { get; set; } = "你是机动特遣队-天网 指挥官";
        [Description("天网激动特遣队指挥官自定义信息")]
        public string SkynetCaptainCustomInfo { get; set; } = "天网 指挥官";
    }
}
