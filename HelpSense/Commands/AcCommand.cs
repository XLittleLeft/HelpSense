using CommandSystem;
using HelpSense.Helper;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class AcCommand : ICommand
    {

        public string Command => "AC";

        public string[] Aliases { get; } = new string[]
        {
            "私聊管理"
        };

        public string Description { get; } = "私聊管理";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get((sender as CommandSender).SenderId);
            if (arguments.Count != 0 && !player.IsMuted && Plugin.Instance.Config.EnableAcSystem && player != null)
            {
                if (CollectionExtensions.At(arguments, 0).Contains("<"))
                {
                    response = "包含敏感字符";
                    return false;
                }
                foreach (Player ply in XHelper.PlayerList)
                {
                    if (ply.RemoteAdminAccess)
                    {
                        XHelper.Mybroadcast(ply , $"<size={Plugin.Instance.Config.ChatSystemSize}><color=red>[求助私信]{player.Nickname}: {CollectionExtensions.At(arguments, 0)}</size>" , 4 , Broadcast.BroadcastFlags.Normal);
                    }
                }
                Log.Info(player.Nickname + " 发送了 " + CollectionExtensions.At(arguments, 0));
                response = "发送成功";
                return true;
            }
            else
            {
                response = "发送失败，你被禁言或者信息为空或者聊天系统未启用";
                return false;
            }
        }
    }
}
