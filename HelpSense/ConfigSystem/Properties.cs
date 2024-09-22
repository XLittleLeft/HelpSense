using System.ComponentModel;

namespace HelpSense.ConfigSystem
{
    public sealed class Properties
    {
        [Description("如果数字小于10，是否应在分钟和秒内添加前零")]
        public bool LeadingZeros { get; private set; } = true;
        [Description("计时器是否应该添加时间，取决于MTF/CI的刷新")]
        public bool TimerOffset { get; private set; } = true;
        [Description("更改自定义提示的频率(以秒计)")]
        public int HintInterval { get; private set; } = 10;
        [Description("九尾狐显示的名称")]
        public string Ntf { get; private set; } = "<color=blue>九尾狐机动特遣队</color>";
        [Description("混沌分裂者显示的名字")]
        public string Ci { get; private set; } = "<color=green>混沌分裂者</color>";
    }
}
