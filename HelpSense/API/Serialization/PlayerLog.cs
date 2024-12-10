using LiteDB;
using System;

namespace HelpSense.API.Serialization
{
    [Serializable]
    public class PlayerLog
    {
        public string NickName { get; set; }
        public DateTime LastJoinedTime { get; set; }
        public DateTime LastLeftTime { get; set; }
        public int PlayedTimes { get; set; }
        public int PlayerKills { get; set; }
        public int PlayerDeath { get; set; }
        public int PlayerSCPKills { get; set; }
        public float PlayerDamage { get; set; }
        public int RolePlayed { get; set; }
        public int PlayerShot { get; set; }
        [BsonId]
        public string ID { get; set; }
    }
}