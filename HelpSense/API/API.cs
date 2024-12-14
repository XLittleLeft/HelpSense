using HelpSense.API.Serialization;
using LiteDB;
using System.Collections.Generic;

namespace HelpSense.API
{
    public static class API
    {
        public static List<string> TimerHidden { get; } = [];
        public static Dictionary<string, PlayerLog> PlayerDataDic = [];

        public static bool TryGetLog(string id, out PlayerLog log)
        {
            if (PlayerDataDic.TryGetValue(id,out log))
            {
                return true;
            }
            using LiteDatabase database = new(Plugin.Instance.Config.SavePath);
            log = database.GetCollection<PlayerLog>("Players")?.FindById(id); 
            if (log != null)
            {
                PlayerDataDic.Add(id, log);
                return true;
            }
            return false;
        }
    }
}
