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
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, float Health, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            Player.Health = Health;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, float Health, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            Player.Health = Health;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.RoleName;
            OldRoleColor = Player.RoleColor;
            Player.RoleName = NewRoleName;
            Player.RoleColor = NewRoleColor;
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
        }

        public void OnPlayerDead(Player Player, string cassie, string tcassie)
        {
            Player.RoleName = OldRoleName;
            Player.RoleColor = OldRoleColor;
            XHelper.MessageTranslated(cassie, tcassie);
        }

        public void AddItems(Player Player, List<ItemType> Items)
        {
            foreach (ItemType Item in Items)
            {
                Player.AddItem(Item);
            }
        }
    }
}
