using MEC;
using HelpSense.API.Features.Pool;
using PlayerRoles;
using PluginAPI.Core;

using System;
using System.Collections.Generic;

using HintServiceMeow.Core.Utilities;
using System.Linq;

namespace HelpSense.Helper
{
    public class ChatMessage
    {
        public enum MessageType
        {
            /// <summary>
            /// Chat privately with admins
            /// </summary>
            AdminPrivateChat,
            /// <summary>
            /// Chat with all players
            /// </summary>
            BroadcastChat,
            /// <summary>
            /// Chat with all teammates
            /// </summary>
            TeamChat,
        }

        public DateTime TimeSent { get; }

        public MessageType Type { get; }
        public string Message { get; }

        public string SenderName { get; }
        public Team SenderTeam { get; }

        public ChatMessage(Player sender, MessageType type, string message)
        {
            this.TimeSent = DateTime.Now;
            this.SenderName = sender.DisplayNickname;
            this.SenderTeam = sender.Team;
            this.Type = type;
            this.Message = message;
        }
    }

    public static class ChatHelper
    {
        private static CoroutineHandle _coroutine;

        private static readonly LinkedList<ChatMessage> MessageList= new();

        private static readonly Dictionary<Player, HintServiceMeow.Core.Models.Hints.Hint> MessageSlot = new();

        private static bool HaveAccess(Player player, ChatMessage message)
        {
            if ((DateTime.Now - message.TimeSent).TotalSeconds > Plugin.Instance.Config.MessageTime)
                return false;

            switch (message.Type)
            {
                case ChatMessage.MessageType.AdminPrivateChat:
                    return player.RemoteAdminAccess;
                case ChatMessage.MessageType.BroadcastChat:
                    return true;
                case ChatMessage.MessageType.TeamChat:
                    return player.Team == message.SenderTeam;
            }

            return false;
        }

        private static IEnumerator<float> MessageCoroutineMethod()
        {
            while (true)
            {
                var sb = StringBuilderPool.Pool.Get();

                foreach (var messageSlot in MessageSlot)
                {
                    if(!MessageList.Any(x => HaveAccess(messageSlot.Key, x)))
                    {
                        messageSlot.Value.Text = string.Empty;
                        continue;
                    }

                    sb.AppendLine(Plugin.Instance.TranslateConfig.ChatMessageTitle);

                    foreach (var message in MessageList)
                    {
                        if (HaveAccess(messageSlot.Key, message))
                        {
                            string messageStr = Plugin.Instance.Config.MessageTemplate
                                .Replace("{Message}", message.Message)
                                .Replace("{MessageType}", Plugin.Instance.TranslateConfig.MessageTypeName[message.Type])
                                .Replace("{MessageTypeColor}", message.Type switch
                                {
                                    ChatMessage.MessageType.AdminPrivateChat => "red",
                                    _ => "{SenderTeamColor}",//Replace by sender's team color later
                                })
                                .Replace("{SenderNickname}", message.SenderName)
                                .Replace("{SenderTeam}", message.SenderTeam.ToString())
                                .Replace("{SenderTeamColor}", message.SenderTeam switch
                                {
                                    Team.SCPs => "red",
                                    Team.ChaosInsurgency => "green",
                                    Team.Scientists => "yellow",
                                    Team.ClassD => "orange",
                                    Team.Dead => "white",
                                    Team.FoundationForces => "#4EFAFF",
                                    _ => "white"
                                })
                                .Replace("{CountDown}", (Plugin.Instance.Config.MessageTime - (int)(DateTime.Now - message.TimeSent).TotalSeconds).ToString());


                            sb.AppendLine(messageStr);
                        }
                    }

                    messageSlot.Value.Text = sb.ToString();
                    sb.Clear();
                }

                yield return Timing.WaitForSeconds(0.5f);
            }
        }

        public static void InitForPlayer(Player player)
        {
            if (!_coroutine.IsRunning)
                _coroutine = Timing.RunCoroutine(MessageCoroutineMethod());

            if (MessageSlot.ContainsKey(player))
            {
                return;
            }

            MessageSlot[player] = new HintServiceMeow.Core.Models.Hints.Hint
            {
                Alignment = HintServiceMeow.Core.Enum.HintAlignment.Left,
                YCoordinate = 250,
                FontSize = Plugin.Instance.Config.ChatSystemSize,
                LineHeight = 5
            };

            PlayerDisplay.Get(player.ReferenceHub).AddHint(MessageSlot[player]);
        }

        public static void SendMessage(Player sender, ChatMessage.MessageType type, string message) => SendMessage(new ChatMessage(sender, type, message));

        public static void SendMessage(ChatMessage message)
        {
            MessageList.AddFirst(message);
        }
    }
}
