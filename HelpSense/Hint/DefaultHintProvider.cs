using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Hint
{
    public class DefaultHintProvider : IHintProvider
    {
        public Player Player { get; }

        internal DefaultHintProvider(Player player)
        {
            this.Player = player;
        }

        public void ShowHint(string message, float duration = 3) => Player.ReceiveHint(message, duration);
    }
}
