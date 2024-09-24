using CommandSystem;
using HelpSense.Helper;
using PlayerRoles;
using PluginAPI.Core;
using System;

namespace HelpSense.Commands.ChatCommand
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class CCommand : ICommand
    {
        public string Command => "C";

        public string[] Aliases => new[]{"队伍广播"};

        public string Description => "队伍聊天";

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

            if (arguments.At(0).Contains("<"))
            {
                response = "包含敏感字符";
                return false;
            }

            foreach (Player ply in XHelper.PlayerList)
            {
                if (player.Team == ply.Team || player.IsSameTeam(ply))
                {
                    string color = player.Team switch
                    {
                        Team.SCPs => "red",
                        Team.ChaosInsurgency => "green",
                        Team.Scientists => "yellow",
                        Team.ClassD => "orange",
                        Team.Dead => "white",
                        Team.FoundationForces => "#4EFAFF",
                        _ => "white"
                    };

                    ply.ShowBroadcast($"<size={Plugin.Instance.Config.ChatSystemSize}>[<color={color}>{player.Team}</color>][队伍]{player.Nickname}: {CollectionExtensions.At(arguments, 0)}</size>", 4, Broadcast.BroadcastFlags.Normal);
                }
            }

            Log.Info(player.Nickname + " 发送了 " + arguments.At(0));

            response = "发送成功";
            return true;
        }
    }
}

