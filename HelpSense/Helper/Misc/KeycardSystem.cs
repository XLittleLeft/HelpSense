using HelpSense.API.Events;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using System.Linq;
using static InventorySystem.Items.Firearms.Modules.CylinderAmmoModule;
using KeycardItem = InventorySystem.Items.Keycards.KeycardItem;
using Locker = LabApi.Features.Wrappers.Locker;
using LockerChamber = MapGeneration.Distributors.LockerChamber;
using Player = LabApi.Features.Wrappers.Player;

namespace HelpSense.Helper.Misc
{
    public static class KeycardSystem
    {
        public static void Toggle(this LockerChamber chamber, Locker locker)
        {
            chamber.SetDoor(!chamber.IsOpen, locker.Base._grantedBeep);
            locker.Base.RefreshOpenedSyncvar();
        }

        public static bool HasKeycardPermission(this DoorVariant door, Player player)
        {
            if (CustomEventHandler.Config.AffectAmnesia &&
                player.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled)
            {
                return false;
            }
        
            foreach (var keycard in player.ReferenceHub.inventory.UserInventory.Items.Values.Where(t => t is KeycardItem))
            {
                if (keycard is ChaosKeycardItem chaosKeycard && door.CheckPermissions(chaosKeycard, out var __))
                {
                    return true;
                }
                if (keycard is KeycardItem keycardItem && door.CheckPermissions(keycardItem, out var __2))
                {
                    return true;
                }
            }
        
            return false;
        
        }
        
        public static bool HasKeycardPermission(this LockerChamber chamber, Player player)
        {
            if (CustomEventHandler.Config.AffectAmnesia &&
                player.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled)
            {
                return false;
            }
        
            foreach (var keycard in player.ReferenceHub.inventory.UserInventory.Items.Values.Where(t => t is KeycardItem))
            {
                if (keycard is ChaosKeycardItem chaosKeycard && chamber.CheckPermissions(chaosKeycard, out var __))
                {
                    return true;
                }
                if (keycard is KeycardItem keycardItem && chamber.CheckPermissions(keycardItem, out var __2))
                {
                    return true;
                }
            }
        
            return false;
        }
        
        public static bool HasKeycardPermission(this Scp079Generator generator, Player player)
        {
            if (CustomEventHandler.Config.AffectAmnesia &&
                                player.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled && !player.IsBypassEnabled)
            {
                return false;
            }
        
            foreach (var keycard in player.ReferenceHub.inventory.UserInventory.Items.Values.Where(t => t is KeycardItem))
            {
                if (keycard is ChaosKeycardItem chaosKeycard && generator.CheckPermissions(chaosKeycard, out var __))
                {
                    return true;
                }
                if (keycard is KeycardItem keycardItem && generator.CheckPermissions(keycardItem, out var __2))
                {
                    return true;
                }
            }
        
            return false;
        }

        #region Toggle Door/Locker
        public static void Toggle(this DoorVariant door, ReferenceHub ply)
        {
            door.NetworkTargetState = !door.TargetState;
            door._triggerPlayer = ply;
            switch (door.NetworkTargetState)
            {
                case false:
                    DoorEvents.TriggerAction(door, DoorAction.Closed, ply);
                    break;
                case true:
                    DoorEvents.TriggerAction(door, DoorAction.Opened, ply);
                    break;
            }
        }
        #endregion

        public static bool IsWithoutItems(this Player ply) =>
            ply.ReferenceHub.inventory.UserInventory.Items.Count == 0;

        public static bool IsUnlocked(this Scp079Generator gen)
        {
            return gen.HasFlag(gen._flags, Scp079Generator.GeneratorFlags.Unlocked);
        }

        public static void Unlock(this Scp079Generator gen)
        {
            gen.ServerSetFlag(Scp079Generator.GeneratorFlags.Unlocked, true);
        }
    }
}
