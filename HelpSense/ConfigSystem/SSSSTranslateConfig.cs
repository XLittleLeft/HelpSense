using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.ConfigSystem
{
    public class SSSSTranslateConfig
    {
        [Description("穿门技能")]
        public string DoorPiercingAbility { get; set; } = "穿门技能";
        [Description("穿门技能按键")]
        public string DoorPiercingAbilityKey { get; set; } = "穿门技能按键";
        [Description("穿门技能按键说明")]
        public string DoorPiercingAbilityKeyDescription { get; set; } = "按一下或长按就可以穿门了";
        [Description("穿门技能发动模式")]
        public string PiercingSkillActivationMode { get; set; } = "穿门技能发动模式";
        [Description("长按")]
        public string Hold { get; set; } = "长按";
        [Description("开关")]
        public string Toggle { get; set; } = "开关";
    }
}
