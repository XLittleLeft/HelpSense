using MEC;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using PluginAPI.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelpSense.Helper
{
    public static class SpectatorHelper
    {
        public static IEnumerator<float> SpectatorList(Player player)
        {
            while (player.IsAlive)
            {
                yield return Timing.WaitForSeconds(1);

                StringBuilder list = StringBuilderPool.Shared.Rent().Append(Plugin.Instance.Config.Title);

                int count = 0;
                foreach (Player splayer in XHelper.PlayerList.Where(x => x.Team == Team.Dead))
                {
                    if (Plugin.Instance.Config.Names.Contains("(NONE)")) break;

                    if (((SpectatorRole)splayer.ReferenceHub.roleManager.CurrentRole).SyncedSpectatedNetId != player.NetworkId) continue;

                    if (splayer.IsGlobalModerator ||
                        (splayer.IsOverwatchEnabled) ||
                        (splayer.IsNorthwoodStaff) ||
                        Plugin.Instance.Config.IgnoredRoles.Contains(splayer.ReferenceHub.serverRoles.name))
                        continue;

                    list.Append(Plugin.Instance.Config.Names.Replace("(NAME)", splayer.Nickname));
                    count++;
                }

                if (count == 0) StringBuilderPool.Shared.Return(list);

                string spectatorList = StringBuilderPool.Shared.ToStringReturn(list)
                        .Replace("(COUNT)", count.ToString())
                        .Replace("(COLOR)", player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()
                        .Replace("<br>", "\n"));

                player.ReceiveHint(spectatorList, 2f);
            }
        }
    }
}
