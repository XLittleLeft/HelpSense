using HelpSense.Handler;
using MEC;
using PlayerRoles.Voice;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpSense.Helper.Lobby
{
    public class LobbyHelper
    {
        public static void SpawnManager()
        {
            try
            {
                if (Plugin.Instance.Config.PracticeHall)
                {
                    LobbyLocationHandler.ChaosLocation();
                    Plugin.CurLobbyLocationType = LobbyLocationType.Chaos;
                    return;
                }

                if (Plugin.Instance.Config.LobbyLocation.Count <= 0)
                {
                    LobbyLocationHandler.TowerLocation();
                    return;
                }

                Plugin.CurLobbyLocationType = Plugin.Instance.Config.LobbyLocation.RandomItem();

                switch (Plugin.CurLobbyLocationType)
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
                Plugin.Instance.Text = string.Empty;

                Plugin.Instance.Text += Plugin.Instance.TranslateConfig.TitleText;

                Plugin.Instance.Text += "\n" + Plugin.Instance.TranslateConfig.PlayerCountText;

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                Plugin.Instance.Text = NetworkTimer switch
                {
                    -2 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.ServerPauseText),
                    -1 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.RoundStartText),
                    1 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.SecondLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                    0 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.RoundStartText),
                    _ => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.SecondsLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                };
                if (XHelper.PlayerList.Count() == 1)
                {
                    Plugin.Instance.Text = Plugin.Instance.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + Plugin.Instance.TranslateConfig.PlayerJoinText);
                }
                else
                {
                    Plugin.Instance.Text = Plugin.Instance.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + Plugin.Instance.TranslateConfig.PlayersJoinText);
                }

                for (int i = 0; i < 25; i++)
                {
                    Plugin.Instance.Text += "\n";
                }

                foreach (Player ply in XHelper.PlayerList)
                {
                    ply.ReceiveHint(Plugin.Instance.Text.ToString(), 1f);//Use compatibility adapter
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static IEnumerator<float> LobbyIcomTimer()
        {
            while (!Round.IsRoundStarted)
            {
                Plugin.Instance.Text = string.Empty;

                Plugin.Instance.Text += Plugin.Instance.TranslateConfig.TitleText;

                Plugin.Instance.Text += "\n" + Plugin.Instance.TranslateConfig.PlayerCountText;

                short NetworkTimer = GameCore.RoundStart.singleton.NetworkTimer;

                Plugin.Instance.Text = NetworkTimer switch
                {
                    -2 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.ServerPauseText),
                    -1 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.RoundStartText),
                    1 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.SecondLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                    0 => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.RoundStartText),
                    _ => Plugin.Instance.Text.Replace("{seconds}", Plugin.Instance.TranslateConfig.SecondsLeftText.Replace("{seconds}", NetworkTimer.ToString())),
                };
                if (XHelper.PlayerList.Count() == 1)
                {
                    Plugin.Instance.Text = Plugin.Instance.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + Plugin.Instance.TranslateConfig.PlayerJoinText);
                }
                else
                {
                    Plugin.Instance.Text = Plugin.Instance.Text.Replace("{players}", $"{XHelper.PlayerList.Count()} " + Plugin.Instance.TranslateConfig.PlayersJoinText);
                }

                if (25 != 0 && 25 > 0)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        Plugin.Instance.Text += "\n";
                    }
                }

                IntercomDisplay._singleton.Network_overrideText = $"<size={Plugin.Instance.Config.IcomTextSize}>" + Plugin.Instance.Text + "</size>";

                yield return Timing.WaitForSeconds(1f);
            }
        }
    }
}
