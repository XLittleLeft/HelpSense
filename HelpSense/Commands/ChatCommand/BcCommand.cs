using CommandSystem;
using HelpSense.Helper.Chat;
using PluginAPI.Core;
using System;

namespace HelpSense.Commands.ChatCommand
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class BcCommand : ICommand
    {
        public string Command => "BC";

        public string[] Aliases => ["广播"];

        public string Description => "全服聊天";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player;

            if (sender is null || (player = Player.Get(sender)) is null)
            {
                response = "发送消息时出现错误，请稍后重试";
                return false;
            }

            if (arguments.Count == 0 || player.IsMuted || !Plugin.Instance.Config.EnableChatSystem)
            {
                response = "发送失败，你被禁言或者信息为空或者聊天系统未启用";
                return false;
            }

            ChatHelper.SendMessage(player, ChatMessage.MessageType.BroadcastChat, $"<noparse>{string.Join(" ", arguments)}</noparse>");

            Log.Info(player.Nickname + " 发送了 " + arguments.At(0));
            response = "发送成功";
            return true;
        }
    }
}
