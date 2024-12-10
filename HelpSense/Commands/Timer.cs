using CommandSystem;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Timer : ICommand
    {
        public string Command => "timer";

        public string[] Aliases => [];

        public string Description => "显示或者关闭刷新时间";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string senderId = ((CommandSender)sender).SenderId;

            if (!API.API.TimerHidden.Remove(senderId))
            {
                API.API.TimerHidden.Add(senderId);
                response = "<color=red>刷新时间显示</color>";
                return true;
            }

            response = "<color=green>刷新时间消失</color>";
            return true;
        }
    }
}
