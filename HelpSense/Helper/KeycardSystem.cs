using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using PlayerRoles;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;

namespace HelpSense.Helper
{
    public static class LazySystem
    {
        public static void Toggle(this LockerChamber chamber, Locker locker)
        {
            chamber.SetDoor(!chamber.IsOpen, locker._grantedBeep);
            locker.RefreshOpenedSyncvar();
        }

        public static void SetupBlacklistedDoors()
        {
            DoorsUtils.AddBlacklistedDoor("HCZ");
            DoorsUtils.AddBlacklistedDoor("LCZ");
            DoorsUtils.AddBlacklistedDoor("EZ");
            DoorsUtils.AddBlacklistedDoor("Prison BreakableDoor");
            DoorsUtils.AddBlacklistedDoor("Unsecured Pryable GateDoor");
            DoorsUtils.AddBlacklistedDoor("Pryable 173 GateDoor");
        }

        public static void SetupBlacklistedLockers()
        {
            LockerUtils.AddBlacklistedLocker("MiscLocker");
            LockerUtils.AddBlacklistedLocker("Adrenaline");
            LockerUtils.AddBlacklistedLocker("Medkit");
        }

        public static bool HasKeycardPermission(this DoorVariant door, Player player)
        {
            if (Plugin.Instance.Config.AffectAmnesia &&
                player.EffectsManager.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled)
            {
                return false;
            }

            foreach (var keycard in player.Items.Where(t => t is KeycardItem))
            {
                if (door.RequiredPermissions.CheckPermissions(keycard, player.ReferenceHub))
                {
                    return true;
                }
            }

            return false;

        }

        public static bool HasKeycardPermission(this LockerChamber chamber, Player player)
        {
            if (Plugin.Instance.Config.AffectAmnesia &&
                player.EffectsManager.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled)
            {
                return false;
            }

            foreach (var keycard in player.Items.Where(t => t is KeycardItem))
            {
                if (((KeycardItem)keycard).Permissions.HasFlagFast(chamber.RequiredPermissions))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasKeycardPermission(this Scp079Generator generator, Player player)
        {
            if (Plugin.Instance.Config.AffectAmnesia &&
                                player.EffectsManager.GetEffect<CustomPlayerEffects.AmnesiaItems>().IsEnabled && !player.IsBypassEnabled)
            {
                return false;
            }

            foreach (var keycard in player.Items.Where(t => t is KeycardItem))
            {
                if (((KeycardItem)keycard).Permissions.HasFlagFast(generator._requiredPermission))
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

    public static class DoorsUtils
    {
        private static readonly List<string> BlacklistedDoors = new List<string>();

        public static List<string> GetBlacklistedDoors()
        {
            return BlacklistedDoors;
        }

        public static void AddBlacklistedDoor(string doorName)
        {
            BlacklistedDoors.Add(doorName);
        }
    }

    public static class LockerUtils
    {
        private static readonly List<string> BlacklistedLockers = new List<string>();

        public static List<string> GetBlacklistedLockers()
        {
            return BlacklistedLockers;
        }

        public static void AddBlacklistedLocker(string lockerName)
        {
            BlacklistedLockers.Add(lockerName);
        }
    }