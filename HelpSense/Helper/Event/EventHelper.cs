using CustomPlayerEffects;
using HelpSense.Helper.SCP;
using InventorySystem.Items.Firearms;
using MapGeneration;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp079;
using PluginAPI.Core;
using Respawning.Waves;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HelpSense.Helper.Event
{
    public static class EventHelper
    {
        public static Random Random = new Random();

        public static void OnTeamRespawn(SpawnableWaveBase spawnableWaveBase, List<ReferenceHub> referenceHubs)
        {
            List<Player> players = new List<Player>();
            referenceHubs.ForEach(x => players.Add(Player.Get(x)));
            List<Player> specialPlayers = players;

            Timing.CallDelayed(0.5f, () =>
            {
                if (Plugin.Instance.Config.EnableSkynet && !Plugin.Instance.SkynetSpawned && spawnableWaveBase is NtfSpawnWave spawnWave)
                {
                    Player player079 = XHelper.PlayerList.FirstOrDefault(x => x.Role is RoleTypeId.Scp079);
                    int player079Leave = 0;
                    if (player079 != null)
                    {
                        Scp079Role scp079 = player079.ReferenceHub.roleManager.CurrentRole as Scp079Role;
                        scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier);
                        player079Leave = tier.AccessTierLevel;
                    }
                    if (player079Leave >= 3)
                    {
                        Plugin.Instance.SkynetSpawned = true;
                        Cassie.Clear();
                        XHelper.MessageTranslated($"MTFUnit Kappa , 10 , and , Mu , 7 , designated scan neck , HasEntered , they will help contain scp 0 7 9 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", Plugin.Instance.TranslateConfig.SkynetCassie.Replace("%SCPNum%", XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));

                        foreach (Player player in players)
                        {
                            Plugin.Instance.SkynetPlayers.Add(player);
                            if (player.Role is not RoleTypeId.NtfCaptain)
                                player.AddItem(ItemType.SCP2176);
                            switch (player.Role)
                            {
                                case RoleTypeId.NtfPrivate:
                                    player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SkynetPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    player.CustomInfo = Plugin.Instance.TranslateConfig.SkynetPrivateCustomInfo;
                                    break;
                                case RoleTypeId.NtfSergeant:
                                    player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SkynetSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    player.CustomInfo = Plugin.Instance.TranslateConfig.SkynetSergeantCustomInfo;
                                    break;
                                case RoleTypeId.NtfCaptain:
                                    player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SkynetCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    player.CustomInfo = Plugin.Instance.TranslateConfig.SkynetCaptainCustomInfo;
                                    break;
                            }
                        }
                        return;
                    }//我搞了半天搞出来的最像的语音
                }
                if (Plugin.Instance.Config.EnableSeeNoEvil)
                {
                    Timing.CallDelayed(1.2f, () =>
                    {
                        if (!Plugin.Instance.SeeSpawned && Random.Next(101) <= Plugin.Instance.Config.SeeNoEvilPer)
                        {
                            if (XHelper.PlayerList.Any(x => x.Role is RoleTypeId.Scp096))
                            {
                                Cassie.Clear();
                                XHelper.MessageTranslated($"MTFUnit Eta , 10 , designated see no evil , HasEntered , they will help contain scp 0 9 6 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", Plugin.Instance.TranslateConfig.SeeNoEvilCassie.Replace("%SCPNum%", XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));
                                Plugin.Instance.SeeSpawned = true;
                                foreach (Player player in players)
                                {
                                    Plugin.Instance.SeePlayers.Add(player);
                                    switch (player.Role)
                                    {
                                        case RoleTypeId.NtfPrivate:
                                            player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SeeNoEvilPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            player.CustomInfo = Plugin.Instance.TranslateConfig.SeeNoEvilPrivateCustomInfo;
                                            break;
                                        case RoleTypeId.NtfSergeant:
                                            player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SeeNoEvilSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            player.CustomInfo = Plugin.Instance.TranslateConfig.SeeNoEvilSergeantCustomInfo;
                                            break;
                                        case RoleTypeId.NtfCaptain:
                                            player.ShowBroadcast($"{Plugin.Instance.TranslateConfig.SeeNoEvilCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            player.CustomInfo = Plugin.Instance.TranslateConfig.SeeNoEvilCaptainCustomInfo;
                                            break;
                                    }
                                    player.Position = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.Name is RoomName.Outside).transform.TransformPoint(62.93f, -8.35f, -51.26f);
                                }
                            }
                        }
                    });
                }
            });

            if (Plugin.Instance.Config.SpawnHID)
            {
                Timing.CallDelayed(2f, () =>
                {
                    foreach (Player Player in players)
                    {
                        if (Player.Role is RoleTypeId.NtfCaptain)
                        {
                            Player.AddItem(ItemType.MicroHID);
                            //var firaerm = Player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                            //TODO:子弹
                            //firaerm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firaerm.GetCurrentAttachmentsCode());
                        }
                    }
                });
            }
            if (Plugin.Instance.Config.EnableChaosLeader && !Plugin.Instance.SpawnLeader)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRifleman, specialPlayers);
                    if (player != null)
                    {
                        specialPlayers.Remove(player);

                        Plugin.Instance.SpawnLeader = true;

                        Plugin.Instance.ChaosLeader = new SCPHelper(player, 150, Plugin.Instance.TranslateConfig.ChaosLeaderRoleName, "green");

                        player.ClearBroadcasts();

                        player.ShowBroadcast(Plugin.Instance.TranslateConfig.ChaosLeaderSpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        var firearm = player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                        //TODO:子弹
                        //firearm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());

                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.SCP268);

                        foreach (Player pl in XHelper.PlayerList)
                        {
                            if (pl.Team is PlayerRoles.Team.ChaosInsurgency)
                            {
                                pl.EffectsManager.EnableEffect<MovementBoost>();
                                pl.EffectsManager.ChangeState<MovementBoost>(5);
                            }
                        }
                    };
                });
            }
            if (Plugin.Instance.Config.SCP2936 && !Plugin.Instance.SpawnSCP2936)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRepressor, specialPlayers);

                    if (player != null)
                    {
                        Plugin.Instance.SpawnSCP2936 = true;

                        specialPlayers.Remove(player);

                        Plugin.Instance.SCP2936 = new SCPHelper(player, 300, "SCP-2936-1", "red");

                        player.SendBroadcast(Plugin.Instance.TranslateConfig.SCP29361SpawnBroadcast, 6);

                        player.SetPlayerScale(2f);
                    }
                });
            }
            if (Plugin.Instance.Config.SCP073 && XHelper.PlayerList.Any(x => x.RoleName == "SCP-073"))
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Player player = XHelper.GetRandomPlayer(specialPlayers);

                    if (player != null)
                    {
                        specialPlayers.Remove(player);

                        Plugin.Instance.SCP073 = new SCPHelper(player, 120, "SCP-073", "green");

                        if (player.Team is Team.ChaosInsurgency)
                        {
                            player.SendBroadcast(Plugin.Instance.TranslateConfig.SCP073AbelSpawnBroadcast, 6);

                            player.ClearInventory();
                            for (int i = 0; i < 8; i++)
                            {
                                player.AddItem(ItemType.Jailbird);
                            }

                            Timing.RunCoroutine(XHelper.PositionCheckerCoroutine(player).CancelWith(player.GameObject));
                        }
                        else
                        {
                            player.SendBroadcast(Plugin.Instance.TranslateConfig.SCP073CainSpawnBroadcast, 6);
                        }
                        player.EffectsManager.EnableEffect<DamageReduction>();
                        player.EffectsManager.ChangeState<DamageReduction>(Plugin.Instance.Config.SCP073DD);
                        player.EffectsManager.EnableEffect<MovementBoost>();
                        player.EffectsManager.ChangeState<MovementBoost>(10);
                    }
                });
            }
        }
    }
}
