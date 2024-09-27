using HintServiceMeow.UI.Extension;
using MEC;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpSense.Helper.Misc
{
    public static class SpectatorHelper
    {
        public static IEnumerator<float> SpectatorList(Player player)
        {
            while (player.IsAlive)
            {
                yield return Timing.WaitForSeconds(1);

                StringBuilder list = StringBuilderPool.Shared.Rent().Append(Plugin.Instance.TranslateConfig.WatchListTitle);

                int count = 0;
                foreach (Player deadPlayers in XHelper.PlayerList.Where(x => x.Team == Team.Dead))
                {
                    if (Plugin.Instance.TranslateConfig.Names.Contains("(NONE)")) break;

                    if (((SpectatorRole)deadPlayers.ReferenceHub.roleManager.CurrentRole).SyncedSpectatedNetId != player.NetworkId) continue;

                    if (deadPlayers.IsGlobalModerator ||
                        (deadPlayers.IsOverwatchEnabled) ||
                        (deadPlayers.IsNorthwoodStaff) ||
                        Plugin.Instance.Config.IgnoredRoles.Contains(deadPlayers.ReferenceHub.serverRoles.name))
                        continue;

                    list.Append(Plugin.Instance.TranslateConfig.Names.Replace("(NAME)", deadPlayers.Nickname));
                    count++;
                }

                if (count == 0) StringBuilderPool.Shared.Return(list);

                string spectatorList = StringBuilderPool.Shared.ToStringReturn(list)
                        .Replace("(COUNT)", count.ToString())
                        .Replace("(COLOR)", player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()
                        .Replace("<br>", "\n"));

                player.ReceiveHint(spectatorList, 2f); //Use compatibility adapter
            }
        }
    }
}
