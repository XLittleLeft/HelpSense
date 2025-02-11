using System.Collections.Generic;

namespace HelpSense.API
{
    using HelpSense.API.Serialization;
    using HelpSense.Helper;
    using LiteDB;
    using MEC;
    using LabApi.Features.Wrappers;
    using System;
    using HelpSense.API.Events;

    public static class InfoExtension
    {
        public static PlayerLog GetLog(this Player ply)
        {
            PlayerLog toInsert = null;
            if (!API.TryGetLog(ply.UserId, out PlayerLog log))
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
                using LiteDatabase database = new(CustomEventHandler.Config.SavePath);
                database.GetCollection<PlayerLog>("Players").Insert(toInsert);
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
            if (!API.TryGetLog(ply.authManager.UserId, out PlayerLog log))
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
                using LiteDatabase database = new(CustomEventHandler.Config.SavePath);
                database.GetCollection<PlayerLog>("Players").Insert(toInsert);
            }

            if (log is null)
                return toInsert;
            return log;
        }

        public static void UpdateLog(this PlayerLog log)
        {
            using LiteDatabase database = new(CustomEventHandler.Config.SavePath);
            database.GetCollection<PlayerLog>("Players").Update(log);
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