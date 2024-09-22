using HarmonyLib;
using HelpSense.Helper;
using PlayerStatsSystem;
using PluginAPI.Core;

namespace HelpSense.Patches
{
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.MaxValue), MethodType.Getter)]
    public static class MaxHealthGetPatch
    {
        public static bool Prefix(ref float __result, HealthStat __instance)
        {
            Player Player = Player.Get(__instance.Hub);
            if (Player.IsSCP && SCPHPChangeSystem.healthDict.TryGetValue(Player.Role, out var health))
            {
                __result = health;
                return false;
            }
            if (!Player.IsSpecialPlayer())
            {
                return true;
            }
            switch (Player.RoleName)
            {
                case "SCP-029":
                    __result = 120;
                    return false;
                case "SCP-703":
                    __result = 120;
                    return false;
                case "SCP-191":
                    __result = 120;
                    return false;
                case "SCP-073":
                    __result = 120;
                    return false;
                case "SCP-2936-1":
                    __result = 300;
                    return false;
                case "混沌领导者":
                    __result = 150;
                    return false;
            }
            return false;
        }
    }
}