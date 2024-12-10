using HelpSense.API.Serialization;
using System.Collections.Generic;

namespace HelpSense.API
{
    public static class API
    {
        public static List<string> TimerHidden { get; } = [];

        public static bool TryGetLog(string id, out PlayerLog log)
        {
            log = Plugin.Instance.Database.GetCollection<PlayerLog>("Players")?.FindById(id);
            return log != null;
        }
    }
}
