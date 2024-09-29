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
            Player player = Player.Get(__instance.Hub);

            if (player.IsSCP && SCPHealthSystem.HealthDict.TryGetValue(player.Role, out var health))
            {
                __result = health;
                return false;
            }

            if (!player.IsSpecialPlayer())
            {
                return true;
            }

            __result = player.RoleName switch
            {
                "SCP-029" => 120,
                "SCP-703" => 120,
                "SCP-191" => 120,
                "SCP-073" => 120,
                "SCP-2936-1" => 300,
                "混沌领导者" => 150,
                _ => 100
            };

            return false;
        }
    }
}