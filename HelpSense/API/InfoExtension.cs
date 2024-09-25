using System.Collections.Generic;

namespace HelpSense.API
{
    using System;
    using System.Linq;
    using HelpSense.API.Serialization;
    using HelpSense.ConfigSystem;
    using HelpSense.Helper;
    using MEC;
    using PlayerRoles;
    using PluginAPI.Core;

    public static class InfoExtension
    {
        public static PlayerLog GetLog(this Player ply)
        {
            PlayerLog toInsert = null;
            if (!API.TryGetLog(ply.UserId, out var log))
            {
                toInsert = new PlayerLog()
                {
                    ID = ply.UserId,
                    NickName = "",
                    LastJoinedTime = DateTime.Now,
                    LastLeftTime = DateTime.Now,
                    PlayedTimes = 0,
                    PlayerKills = 0,
                    PlayerDeath = 0,
                    PlayerSCPKills = 0,
                    PlayerDamage = 0,
                    RolePlayed = 0,
                    PlayerShot = 0,
                };
                Plugin.Instance.Database.GetCollection<PlayerLog>("Players").Insert(toInsert);
            }

            if (log is null)
                return toInsert;
            return log;
        }
        public static PlayerLog GetLog(this ReferenceHub ply)
        {
            PlayerLog toInsert = null;
            if (string.IsNullOrWhiteSpace(ply.authManager.UserId))
                throw new ArgumentNullException(nameof(ply));
            if (!API.TryGetLog(ply.authManager.UserId, out var log))
            {
                toInsert = new PlayerLog()
                {
                    ID = ply.authManager.UserId,
                    NickName = "",
                    LastJoinedTime = DateTime.Now,
                    LastLeftTime = DateTime.Now,
                    PlayedTimes = 0,
                    PlayerKills = 0,
                    PlayerDeath = 0,
                    PlayerSCPKills = 0,
                    PlayerDamage = 0,
                    RolePlayed = 0,
                    PlayerShot = 0,
                };
                Plugin.Instance.Database.GetCollection<PlayerLog>("Players").Insert(toInsert);
            }

            if (log is null)
                return toInsert;
            return log;
        }

        public static void UpdateLog(this PlayerLog log)
        {
            Plugin.Instance.Database.GetCollection<PlayerLog>("Players").Update(log);
        }

        public static IEnumerator<float> CollectInfo()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(60f);

                foreach (Player Player in XHelper.PlayerList)
                {
                    if (Player != null && !Player.DoNotTrack)
                    {
                        var pLog = Player.GetLog();
                        pLog.PlayedTimes += 60;
                        pLog.UpdateLog();
                    }
                }
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
            }
        }
    }
}