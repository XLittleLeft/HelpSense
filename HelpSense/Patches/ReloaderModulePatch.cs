using HarmonyLib;
using HelpSense.ConfigSystem;
using InventorySystem.Items.Autosync;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using MEC;
using Mirror;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventorySystem.Items.Firearms.Modules.AnimatorReloaderModuleBase;
using HelpSense.API.Events;

namespace HelpSense.Patches
{
    [HarmonyPatch(typeof(AnimatorReloaderModuleBase) , nameof(AnimatorReloaderModuleBase.ServerProcessCmd))]
    public static class ReloaderModulePatch
    {
        public static bool Prefix(AnimatorReloaderModuleBase __instance , NetworkReader reader)
        {
            if (!CustomEventHandler.Config.InfiniteAmmo) return true;

            ReloaderMessageHeader header = (ReloaderMessageHeader)reader.ReadByte();
            Firearm Firearm = __instance.Firearm;
            Player Player = Player.Get(Firearm.Owner.authManager.UserId);

            if (!__instance.TryContinueDeserialization(reader, Firearm.ItemSerial, header, AutosyncMessageType.Cmd))
            {
                reader.Position -= sizeof(byte);
                return true;
            }

            if (header is ReloaderMessageHeader.Reload && IReloadUnloadValidatorModule.ValidateReload(Firearm))
            {
                if (Firearm.ItemTypeId is ItemType.ParticleDisruptor)
                {
                    reader.Position -= sizeof(byte);
                    return true;
                }
                if (Firearm.TryGetModule<MagazineModule>(out var MagazineModule))
                {
                    switch (CustomEventHandler.Config.InfiniteAmmoType)
                    {
                        case InfiniteAmmoType.Normal:
                            Player.SetAmmo(MagazineModule.AmmoType, (ushort)(MagazineModule.AmmoMax - MagazineModule.AmmoStored + 1));
                            break;
                        case InfiniteAmmoType.Moment:
                            MagazineModule.ServerSetInstanceAmmo(Firearm.ItemSerial, MagazineModule.AmmoMax);
                            break;
                    }
                }
                else if (Firearm.ItemTypeId is ItemType.GunRevolver && Firearm.TryGetModule<CylinderAmmoModule>(out var CylinderAmmoModule))
                {
                    Player.SetAmmo(CylinderAmmoModule.AmmoType, (ushort)(CylinderAmmoModule.AmmoMax - CylinderAmmoModule.AmmoStored + 1));
                }
            }

            reader.Position -= sizeof(byte);

            return true;
        }
    }
}