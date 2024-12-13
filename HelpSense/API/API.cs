using HelpSense.API.Serialization;
using LiteDB;
using System.Collections.Generic;

namespace HelpSense.API
{
    public static class API
    {
        public static List<string> TimerHidden { get; } = [];

        public static bool TryGetLog(string id, out PlayerLog log)
        {
            using LiteDatabase database = new(Plugin.Instance.Config.SavePath);
            log = database.GetCollection<PlayerLog>("Players")?.FindById(id);
            return log != null;
        }
    }
}
