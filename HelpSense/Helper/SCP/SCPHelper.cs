using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HelpSense.Helper.SCP
{
    public class SCPHelper
    {
        public Player Player;
        public string OldRoleName;
        public string OldRoleColor;

        public SCPHelper(Player player)
        {
            this.Player = player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
        }

        public void OnPlayerDead(Player player, string cassieMessage, string translatedCassieMessage)
        {
            player.RoleName = OldRoleName;
            player.RoleColor = OldRoleColor;
            XHelper.MessageTranslated(cassieMessage, translatedCassieMessage);
        }
    }
}
