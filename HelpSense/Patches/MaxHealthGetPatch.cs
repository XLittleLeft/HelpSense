using HarmonyLib;
using HelpSense.Helper;
using HelpSense.Helper.SCP;
using PlayerStatsSystem;
using PluginAPI.Core;

namespace HelpSense.Patches
{
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
    public static class MaxHealthGetPatch
    {
        public static bool Prefix(ref float __result, HealthStat __instance)
        {
            if (SCPHPChangeSystem.healthDict.TryGetValue(Player.Get(__instance.Hub).Role, out var health))
            {
                __result = health;
                return false;
            }
            if (!Player.Get(__instance.Hub).IsSpecialPlayer())
            {
                return true;
            }
            if (Player.Get(__instance.Hub).RoleName == "SCP-029")
            {
                __result = 120;
                return false;
            }
            if (Player.Get(__instance.Hub).RoleName == "SCP-703")
            {
                __result = 120;
                return false;
            }
            if (Player.Get(__instance.Hub).RoleName == "SCP-191")
            {
                __result = 120;
                return false;
            }
            if (Player.Get(__instance.Hub).RoleName == "SCP-073")
            {
                __result = 120;
                return false;
            }
            if (Player.Get(__instance.Hub).RoleName == "SCP-2936-1")
            {
                __result = 300;
                return false;
            }
            if (Player.Get(__instance.Hub).RoleName == Plugin.Instance.TranslateConfig.ChaosLeaderRoleName)
            {
                __result = 150;
                return false;
            }
            return false;
        }
    }
}