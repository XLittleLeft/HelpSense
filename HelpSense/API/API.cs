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
            using LiteDatabase database = new(Plugin.Instance.Config.SavePath);
            if (PlayerDataDic.TryGetValue(id,out log))
            {
                return true;
            }
            log = database.GetCollection<PlayerLog>("Players")?.FindById(id); 
            return log != null;
        }
    }
}
