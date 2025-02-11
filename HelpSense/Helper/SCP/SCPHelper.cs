using InventorySystem.Items;
using PlayerRoles;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
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
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, float Health, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            Player.Health = Health;
            Player.MaxHealth = Health;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, float Health, string NewRoleName, string NewRoleColor, Vector3 Vector3)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            Player.Health = Health;
            Player.MaxHealth = Health;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.Position = Vector3;
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, List<ItemType> Items, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            AddItems(Player, Items);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }
        public SCPHelper(Player Player, float Health, RoleTypeId NewRole, string NewRoleName, string NewRoleColor, string Broadcast, ushort Time)
        {
            this.Player = Player;
            Player.SetRole(NewRole);
            OldRoleName = Player.GetRoleName();
            OldRoleColor = Player.GetRoleColor();
            Player.SetRoleName(NewRoleName);
            Player.SetRoleColor(NewRoleColor);
            Player.SendBroadcast(Broadcast, Time);
            Player.Health = Health;
            Player.MaxHealth = Health;
        }

        public void OnPlayerDead(Player Player, string cassie, string tcassie)
        {
            Player.SetRoleName(OldRoleName);
            Player.SetRoleColor(OldRoleColor);
            XHelper.MessageTranslated(cassie, tcassie);
        }

        public void AddItems(Player Player, List<ItemType> Items)
        {
            foreach (ItemType Item in Items)
            {
                Player.AddItem(Item , ItemAddReason.AdminCommand);
            }
        }
    }
}
