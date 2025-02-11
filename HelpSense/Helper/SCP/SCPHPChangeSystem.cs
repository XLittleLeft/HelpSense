using HelpSense.API.Events;
using PlayerRoles;
using System.Collections.Generic;

namespace HelpSense.Helper.SCP
{
    public class SCPHPChangeSystem
    {
        public static Dictionary<RoleTypeId, float> healthDict = new()
        {
            [RoleTypeId.Scp173] = CustomEventHandler.Config.SCPsHP[0],
            [RoleTypeId.Scp939] = CustomEventHandler.Config.SCPsHP[1],
            [RoleTypeId.Scp049] = CustomEventHandler.Config.SCPsHP[2],
            [RoleTypeId.Scp0492] = CustomEventHandler.Config.SCPsHP[3],
            [RoleTypeId.Scp096] = CustomEventHandler.Config.SCPsHP[4],
            [RoleTypeId.Scp106] = CustomEventHandler.Config.SCPsHP[5]
        };
    }
}
