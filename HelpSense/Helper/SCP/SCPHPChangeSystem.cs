using PlayerRoles;
using System.Collections.Generic;

namespace HelpSense.Helper.SCP
{
    public class SCPHPChangeSystem
    {
        public static Dictionary<RoleTypeId, float> healthDict = new()
        {
            [RoleTypeId.Scp173] = Plugin.Instance.Config.SCPsHP[0],
            [RoleTypeId.Scp939] = Plugin.Instance.Config.SCPsHP[1],
            [RoleTypeId.Scp049] = Plugin.Instance.Config.SCPsHP[2],
            [RoleTypeId.Scp0492] = Plugin.Instance.Config.SCPsHP[3],
            [RoleTypeId.Scp096] = Plugin.Instance.Config.SCPsHP[4],
            [RoleTypeId.Scp106] = Plugin.Instance.Config.SCPsHP[5]
        };
    }
}
