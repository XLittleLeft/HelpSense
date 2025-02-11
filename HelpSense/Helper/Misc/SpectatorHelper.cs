using MEC;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.Spectating;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HelpSense.API.Events;

namespace HelpSense.Helper.Misc
{
    public static class SpectatorHelper
    {
        public static IEnumerator<float> SpectatorList(Player player)
        {
            while (player.IsAlive)
            {
                yield return Timing.WaitForSeconds(1);

                StringBuilder list = StringBuilderPool.Shared.Rent().Append(CustomEventHandler.TranslateConfig.WatchListTitle);

                int count = 0;
                foreach (Player deadPlayers in XHelper.PlayerList.Where(x => x.Team == Team.Dead))
                {
                    if (CustomEventHandler.TranslateConfig.Names.Contains("(NONE)")) break;

                    if (((SpectatorRole)deadPlayers.ReferenceHub.roleManager.CurrentRole).SyncedSpectatedNetId != player.NetworkId) continue;

                    if (deadPlayers.IsGlobalModerator ||
                        (deadPlayers.IsOverwatchEnabled) ||
                        (deadPlayers.IsNorthwoodStaff) ||
                        CustomEventHandler.Config.IgnoredRoles.Contains(deadPlayers.ReferenceHub.serverRoles.name))
                        continue;

                    list.Append(CustomEventHandler.TranslateConfig.Names.Replace("(NAME)", deadPlayers.Nickname));
                    count++;
                }

                if (count == 0) StringBuilderPool.Shared.Return(list);

                string spectatorList = StringBuilderPool.Shared.ToStringReturn(list)
                        .Replace("(COUNT)", count.ToString())
                        .Replace("(COLOR)", player.ReferenceHub.roleManager.CurrentRole.RoleColor.ToHex()
                        .Replace("<br>", "\n"));

                player.SendHint(spectatorList, 2f); //Use compatibility adapter
            }
        }
    }
}
