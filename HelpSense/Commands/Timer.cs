using CommandSystem;
using HelpSense.Helper;
using PluginAPI.Core;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Timer : ICommand
    {

        public string Command
        {
            get
            {
                return "timer";
            }
        }

        public string[] Aliases
        {
            get
            {
                return Array.Empty<string>();
            }
        }

        public string Description
        {
            get
            {
                return "显示或者关闭刷新时间";
            }
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string senderId = ((CommandSender)sender).SenderId;
            bool flag = !API.API.TimerHidden.Remove(senderId);
            bool result;
            if (flag)
            {
                API.API.TimerHidden.Add(senderId);
                response = "<color=red>刷新时间显示</color>";
                result = true;
            }
            else
            {
                response = "<color=green>刷新时间消失</color>";
                result = true;
            }
            return result;
        }
    }
}
