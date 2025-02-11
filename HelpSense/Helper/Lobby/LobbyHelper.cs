using HelpSense.Handler;
using MEC;
using PlayerRoles.Voice;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using HelpSense.API.Events;

using Log = LabApi.Features.Console.Logger;

namespace HelpSense.Helper.Lobby
{
    public class LobbyHelper
    {
        public static void SpawnManager()
        {
            try
            {
                if (CustomEventHandler.Config.PracticeHall)
                {
                    LobbyLocationHandler.ChaosLocation();
                    CustomEventHandler.CurLobbyLocationType = LobbyLocationType.Chaos;
                    return;
                }

                if (CustomEventHandler.Config.LobbyLocation.Count <= 0)
                {
                    LobbyLocationHandler.TowerLocation();
                    return;
                }

                CustomEventHandler.CurLobbyLocationType = CustomEventHandler.Config.LobbyLocation.RandomItem();

                switch (CustomEventHandler.CurLobbyLocationType)
                {
                    case LobbyLocationType.Tower:
                        LobbyLocationHandler.TowerLocation();
                        break;
                    case LobbyLocationType.Intercom:
                        LobbyLocationHandler.IntercomLocation();
                        break;
                    case LobbyLocationType.Mountain:
                        LobbyLocationHandler.MountainLocation();
                        break;
                    case LobbyLocationType.Chaos:
                        LobbyLocationHandler.ChaosLocation();
                        break;
                    default:
                        LobbyLocationHandler.TowerLocation();
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error("[HelpSense] [Method: SpawnManager] " + e.ToString());
            }
        }
        public static IEnumerator<float> LobbyTimer()
        {
            while (!Round.IsRoundStarted)
            {
                CustomEventHandler.Text = string.Empty;

                CustomEventHandler.Text += CustomEventHandler.TranslateConfig.TitleText;

                CustomEventHandler.Text += "\n" + CustomEventHandler.TranslateConfig.PlayerCountText;

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                CustomEventHandler.Text = NetworkTimer switch
                {
                    -2 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.ServerPauseText),
                    -1 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.RoundStartText),
                    1 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.SecondLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                    0 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.RoundStartText),
                    _ => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.SecondsLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                };
                if (XHelper.PlayerList.Count() == 1)
                {
                    CustomEventHandler.Text = CustomEventHandler.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + CustomEventHandler.TranslateConfig.PlayerJoinText);
                }
                else
                {
                    CustomEventHandler.Text = CustomEventHandler.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + CustomEventHandler.TranslateConfig.PlayersJoinText);
                }

                for (int i = 0; i < 25; i++)
                {
                    CustomEventHandler.Text += "\n";
                }

                foreach (Player ply in XHelper.PlayerList)
                {
                    ply.SendHint(CustomEventHandler.Text.ToString(), 1f);//Use compatibility adapter
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> LobbyIcomTimer()
        {
            while (!Round.IsRoundStarted)
            {
                CustomEventHandler.Text = string.Empty;

                CustomEventHandler.Text += CustomEventHandler.TranslateConfig.TitleText;

                CustomEventHandler.Text += "\n" + CustomEventHandler.TranslateConfig.PlayerCountText;

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                CustomEventHandler.Text = NetworkTimer switch
                {
                    -2 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.ServerPauseText),
                    -1 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.RoundStartText),
                    1 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.SecondLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                    0 => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.RoundStartText),
                    _ => CustomEventHandler.Text.Replace("{seconds}", CustomEventHandler.TranslateConfig.SecondsLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                };
                if (XHelper.PlayerList.Count() == 1)
                {
                    CustomEventHandler.Text = CustomEventHandler.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + CustomEventHandler.TranslateConfig.PlayerJoinText);
                }
                else
                {
                    CustomEventHandler.Text = CustomEventHandler.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + CustomEventHandler.TranslateConfig.PlayersJoinText);
                }

                if (25 != 0 && 25 > 0)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        CustomEventHandler.Text += "\n";
                    }
                }

                IntercomDisplay._singleton.Network_overrideText = $"<size={CustomEventHandler.Config.IcomTextSize}>" + CustomEventHandler.Text + "</size>";

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
