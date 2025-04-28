using CustomPlayerEffects;
using GameCore;
using HarmonyLib;
using HelpSense.API.Serialization;
using HelpSense.ConfigSystem;
using HelpSense.Handler;
using HelpSense.Helper.Chat;
using HelpSense.Helper;
using HelpSense.Helper.Lobby;
using HelpSense.Helper.Misc;
using HelpSense.Helper.SCP;
using HelpSense.Helper.SpecialRole;
using HelpSense.MonoBehaviors;
using HelpSense.SSSS;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using MapGeneration;
using MEC;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.Voice;
using PlayerRoles;
using PlayerStatsSystem;
using Respawning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Broadcast;
using UnityEngine;
using Log = LabApi.Features.Console.Logger;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using HintServiceMeow.UI.Utilities;
using PlayerRoles.PlayableScps.Scp079;
using Respawning.Waves;
using LabApi.Events.Arguments.Scp173Events;
using LabApi.Events.Arguments.Scp096Events;
using LabApi.Events.Arguments.Scp914Events;
using InventorySystem;
using static PlayerList;
using Discord;
using HintServiceMeow.UI.Extension;

namespace HelpSense.API.Events
{
    public class CustomEventHandler : CustomEventsHandler
    {
        public static LobbyLocationType CurLobbyLocationType;

        public static CoroutineHandle LobbyTimer;

        public static string Text;

        public static SCPHelper SCP703;

        public static SCPHelper SCP029;

        public static SCPHelper SCP191;

        public static SCPHelper SCP073;

        public static SCPHelper SCP347;

        public static SCPHelper SCP023;

        public static SCPHelper ChaosLeader;
        public static bool SpawnLeader = false;

        public static SCPHelper SCP2936;
        public static bool SpawnSCP2936 = false;

        public static SCPHelper SCP1093;

        public static HashSet<Player> SkynetPlayers = [];
        public static bool SkynetSpawned = false;

        public static HashSet<Player> SeePlayers = [];
        public static bool SeeSpawned = false;
        public static bool Scp096Enraging = false;

        public static ushort SCP1068Id = 0;
        public static ItemBase SCP1068Base;

        public static ushort SCP1056Id = 0;
        public static Item SCP1056Base;

        public static System.Random Random = new(DateTime.Now.GetHashCode());

        public static Config Config;
        public static TranslateConfig TranslateConfig;
        public static SSSSTranslateConfig SSSSTranslateConfig;
        public static CommandTranslateConfig CommandTranslateConfig;

        public override void OnServerWaitingForPlayers()
        {
            base.OnServerWaitingForPlayers();

            SkynetSpawned = false;
            SeeSpawned = false;
            SpawnLeader = false;
            SpawnSCP2936 = false;

            if (Config.EnableRoundWaitingLobby)
            {
                try
                {
                    Timing.CallDelayed(0.1f, () =>
                    {
                        LobbyHelper.SpawnManager();
                        GameObject.Find("StartRound").transform.localScale = Vector3.zero;

                        if (LobbyTimer.IsRunning)
                            Timing.KillCoroutines(LobbyTimer);

                        if (CurLobbyLocationType == LobbyLocationType.Intercom && Config.DisplayInIcom)
                            LobbyTimer = Timing.RunCoroutine(LobbyHelper.LobbyIcomTimer());
                        else
                            LobbyTimer = Timing.RunCoroutine(LobbyHelper.LobbyTimer());
                    });
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnWaitingForPlayers] " + e);
                }
            }
            if (Config.EnableFriendlyFire)
            {
                AttackerDamageHandler.RefreshConfigs();
                Server.FriendlyFire = false;
            }
        }

        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            base.OnPlayerJoined(ev);

            Player player = ev.Player;
            
            if (player == null || string.IsNullOrEmpty(player.UserId)) return;
            XHelper.PlayerList.Add(player);
            XHelper.SpecialPlayerList.Add(player);

            ChatHelper.InitForPlayer(player);

            if (Config.SavePlayersInfo)
            {
                PlayerLog log = player.GetLog();
                log.NickName = player.Nickname;
                log.LastJoinedTime = DateTime.Now;
                log.UpdateLog();
            }

            if (Config.SavePlayersInfo && player.DoNotTrack)
            {
                player.SendBroadcast(TranslateConfig.DNTWarning, 10);
            }

            if (Config.EnableRoundWaitingLobby)
            {
                try
                {
                    if (!Round.IsRoundStarted && (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2))
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            player.SetRole(Config.LobbyPlayerRole);

                            player.IsGodModeEnabled = true;

                            if (Config.LobbyInventory.Count > 0 && CurLobbyLocationType != LobbyLocationType.Chaos)
                            {
                                foreach (var item in Config.LobbyInventory)
                                {
                                    player.AddItem(item, ItemAddReason.AdminCommand);
                                }
                            }
                            if (CurLobbyLocationType is LobbyLocationType.Chaos)
                            {
                                player.AddItem(ItemType.ArmorHeavy);
                                player.AddItem(ItemType.GunAK, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunCrossvec, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunE11SR, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunFRMG0, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunLogicer, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunRevolver, ItemAddReason.AdminCommand);
                                player.AddItem(ItemType.GunShotgun, ItemAddReason.AdminCommand);
                            }
                        });

                        Timing.CallDelayed(0.6f, () =>
                        {
                            player.Position = LobbyLocationHandler.LobbyPosition;
                            player.Rotation = LobbyLocationHandler.LobbyRotation;

                            player.EnableEffect<MovementBoost>(Config.MovementBoostIntensity);
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnPlayerJoin] " + e);
                }
            }
            if (Config.EnableWelcomeInfo)
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.WelcomeMessage.Replace("%playername%", player.Nickname), Config.WelcomeTime);
                });
            }
        }

        public override void OnPlayerHurting(PlayerHurtingEventArgs ev)
        {
            Player Target = ev.Player;
            Player Attacker = ev.Attacker;
            DamageHandlerBase DamageHandler = ev.DamageHandler;

            if (Target != null && Attacker != null)
            {
                if (Target.GetRoleName() == "SCP-1093" && DamageHandler is AttackerDamageHandler attackerDamageHandler)
                {
                    if (attackerDamageHandler.Hitbox is HitboxType.Headshot)
                    {
                        attackerDamageHandler.Damage = 0;
                    }
                }

                if (Target.GetRoleName() == "SCP-191" && (Attacker.IsHuman && Attacker.CurrentItem.Base.ItemTypeId.IsWeapon()) && DamageHandler is StandardDamageHandler standard)
                {
                    standard.Damage = Config.SCP191Ammo;
                }

                if (Target.GetRoleName() == "SCP-073" && Attacker.Team != Target.Team && Attacker.IsHuman && Target.Team is not Team.ChaosInsurgency)
                {
                    Attacker.Damage(Config.SCP073RRD, TranslateConfig.SCP073DamageReason);
                }
                else if (Attacker.Team != Target.Team && Attacker.IsSCP && Target.GetRoleName() == "SCP-073" && Target.Team is not Team.ChaosInsurgency)
                {
                    Attacker.Damage(Config.SCP073SCPRD, TranslateConfig.SCP073DamageReason);
                }

                if ((Attacker.GetRoleName() == "SCP-191" && Target.Team is Team.SCPs) || (Target.GetRoleName() == "SCP-191" && Attacker.Team is Team.SCPs))
                {
                    ev.IsAllowed = false;
                }

                if (Target.GetRoleName() == "SCP-023" && Attacker.TryGetEffect(out Scp1344 scp1344) && Target.Team.GetFaction() != Attacker.Team.GetFaction())
                {
                    if (scp1344.Intensity > 0)
                    {
                        Attacker.Kill(TranslateConfig.SCP023ReversedCauseOfDeath);
                    }
                }

                if (DamageHandler is StandardDamageHandler standardDamage && Attacker.Team != Target.Team)
                {
                    if (Config.SavePlayersInfo)
                    {
                        var pLog = Attacker.GetLog();
                        pLog.PlayerDamage += standardDamage.Damage;
                        pLog.UpdateLog();
                    }
                    if (Attacker.Role is RoleTypeId.Scp096 && SeePlayers.Contains(Target))
                    {
                        standardDamage.Damage = 40;
                    }
                }
            }
        }

        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            Player player = ev.Player;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            if (Config.SavePlayersInfo)
            {
                PlayerLog log = player.GetLog();
                log.LastLeftTime = DateTime.Now;
                log.UpdateLog();
            }

            XHelper.PlayerList.Remove(player);
            XHelper.SpecialPlayerList.Remove(player);
            API.PlayerDataDic.Remove(player.UserId);
        }


        public override void OnPlayerEscaped(PlayerEscapedEventArgs ev)
        {
            Player player = ev.Player;
            var newRole = ev.NewRole;

            if (player.GetRoleName() == "SCP-029")
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.EnableEffect<MovementBoost>(25);
                    player.EnableEffect<Scp1853>(2);
                    player.EnableEffect<DamageReduction>(15);
                    if (newRole == RoleTypeId.ChaosConscript)
                    {
                        player.AddItem(ItemType.SCP268);
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP029EscapeHint);
                    }
                });
            }
            if (player.GetRoleName() == "SCP-703")
            {
                Timing.CallDelayed(1f, () =>
                {
                    if (newRole == RoleTypeId.NtfSpecialist)
                    {
                        player.AddItem(ItemType.ParticleDisruptor, ItemAddReason.AdminCommand);
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP703EscapeHint);
                    }
                });
            }
        }

        public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
        {
            var Wave = ev.Wave;
            var players = ev.Players;
            var specialPlayers = ev.Players.ToList();

            if (Wave is NtfSpawnWave)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (Config.EnableSkynet && !SkynetSpawned)
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
                            SkynetSpawned = true;
                            Cassie.Clear();
                            XHelper.MessageTranslated($"MTFUnit Kappa , 10 , and , Mu , 7 , designated scan neck , HasEntered , they will help contain scp 0 7 9 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", TranslateConfig.SkynetCassie.Replace("%SCPNum%", XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));

                            foreach (Player player in players)
                            {
                                SkynetPlayers.Add(player);
                                if (player.Role is not RoleTypeId.NtfCaptain)
                                    player.AddItem(ItemType.SCP2176);
                                switch (player.Role)
                                {
                                    case RoleTypeId.NtfPrivate:
                                        player.SendBroadcast($"{TranslateConfig.SkynetPrivateBroadcast}", 5, BroadcastFlags.Normal);
                                        player.CustomInfo = TranslateConfig.SkynetPrivateCustomInfo;
                                        break;
                                    case RoleTypeId.NtfSergeant:
                                        player.SendBroadcast($"{TranslateConfig.SkynetSergeantBroadcast}", 5, BroadcastFlags.Normal);
                                        player.CustomInfo = TranslateConfig.SkynetSergeantCustomInfo;
                                        break;
                                    case RoleTypeId.NtfCaptain:
                                        player.SendBroadcast($"{TranslateConfig.SkynetCaptainBroadcast}", 5, BroadcastFlags.Normal);
                                        player.CustomInfo = TranslateConfig.SkynetCaptainCustomInfo;
                                        break;
                                }
                            }
                            return;
                        }//我搞了半天搞出来的最像的语音
                    }
                    if (Config.EnableSeeNoEvil)
                    {
                        Timing.CallDelayed(1.2f, () =>
                        {
                            if (!SeeSpawned && Random.Next(101) <= Config.SeeNoEvilPer)
                            {
                                if (XHelper.PlayerList.Any(x => x.Role is RoleTypeId.Scp096))
                                {
                                    Cassie.Clear();
                                    XHelper.MessageTranslated($"MTFUnit Eta , 10 , designated see no evil , HasEntered , they will help contain scp 0 9 6 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", TranslateConfig.SeeNoEvilCassie.Replace("%SCPNum%", XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));
                                    SeeSpawned = true;
                                    foreach (Player player in players)
                                    {
                                        SeePlayers.Add(player);
                                        switch (player.Role)
                                        {
                                            case RoleTypeId.NtfPrivate:
                                                player.SendBroadcast($"{TranslateConfig.SeeNoEvilPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                                player.CustomInfo = TranslateConfig.SeeNoEvilPrivateCustomInfo;
                                                break;
                                            case RoleTypeId.NtfSergeant:
                                                player.SendBroadcast($"{TranslateConfig.SeeNoEvilSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                                player.CustomInfo = TranslateConfig.SeeNoEvilSergeantCustomInfo;
                                                break;
                                            case RoleTypeId.NtfCaptain:
                                                player.SendBroadcast($"{TranslateConfig.SeeNoEvilCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                                player.CustomInfo = TranslateConfig.SeeNoEvilCaptainCustomInfo;
                                                break;
                                        }
                                        player.Position = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(x => x.Name is RoomName.Outside).transform.TransformPoint(62.93f, -8.35f, -51.26f);
                                    }
                                }
                            }
                        });
                    }
                });
            }

            if (Config.SpawnHID)
            {
                Timing.CallDelayed(2f, () =>
                {
                    foreach (Player Player in players)
                    {
                        if (Player.Role is RoleTypeId.NtfCaptain)
                        {
                            Player.AddItem(ItemType.MicroHID, ItemAddReason.AdminCommand);
                        }
                    }
                });
            }
            if (Config.EnableChaosLeader && !SpawnLeader)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRifleman, specialPlayers);
                    if (player != null)
                    {
                        specialPlayers.Remove(player);

                        SpawnLeader = true;

                        ChaosLeader = new SCPHelper(player, 150, TranslateConfig.ChaosLeaderRoleName, "green");

                        player.ClearBroadcasts();

                        player.SendBroadcast(TranslateConfig.ChaosLeaderSpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        player.AddItem(ItemType.ParticleDisruptor, ItemAddReason.AdminCommand);

                        player.AddItem(ItemType.SCP1853);
                        player.AddItem(ItemType.SCP268);

                        foreach (Player pl in XHelper.PlayerList)
                        {
                            if (pl.Team is Team.ChaosInsurgency)
                            {
                                pl.EnableEffect<MovementBoost>(5);
                            }
                        }
                    };
                });
            }
            if (Config.SCP2936 && !SpawnSCP2936)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRepressor, specialPlayers);

                    if (player != null)
                    {
                        SpawnSCP2936 = true;

                        specialPlayers.Remove(player);

                        SCP2936 = new SCPHelper(player, 300, "SCP-2936-1", "red");

                        player.SendBroadcast(TranslateConfig.SCP29361SpawnBroadcast, 6);

                        player.SetPlayerScale(2f);
                    }
                });
            }
            if (Config.SCP073 && XHelper.PlayerList.Any(x => x.GetRoleName() == "SCP-073"))
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Player player = XHelper.GetRandomPlayer(specialPlayers);

                    if (player != null)
                    {
                        specialPlayers.Remove(player);

                        SCP073 = new SCPHelper(player, 120, "SCP-073", "green");

                        if (player.Team is Team.ChaosInsurgency)
                        {
                            player.SendBroadcast(TranslateConfig.SCP073AbelSpawnBroadcast, 6);

                            player.ClearInventory(false);
                            for (int i = 0; i < 8; i++)
                            {
                                player.AddItem(ItemType.Jailbird, ItemAddReason.AdminCommand);
                            }

                            Timing.RunCoroutine(XHelper.PositionCheckerCoroutine(player).CancelWith(player.GameObject));
                        }
                        else
                        {
                            player.SendBroadcast(TranslateConfig.SCP073CainSpawnBroadcast, 6);
                        }
                        player.EnableEffect<DamageReduction>(Config.SCP073DD);
                        player.EnableEffect<MovementBoost>(10);
                    }
                });
            }
        }

        public override void OnServerRoundStarting(RoundStartingEventArgs ev)
        {
            if (Config.EnableRoundWaitingLobby)
            {
                try
                {
                    if (!string.IsNullOrEmpty(IntercomDisplay._singleton.Network_overrideText)) IntercomDisplay._singleton.Network_overrideText = "";

                    foreach (Player player in XHelper.PlayerList)
                    {
                        player.SetRole(RoleTypeId.None , RoleChangeReason.RemoteAdmin);

                        Timing.CallDelayed(0.25f, () =>
                        {
                            player.IsGodModeEnabled = false;
                            player.DisableEffect<MovementBoost>();
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnRoundStarted] " + e);
                }
            }
        }

        public override void OnServerRoundStarted()
        {
            if (Config.SavePlayersInfo)
            {
                Timing.RunCoroutine(InfoExtension.CollectInfo());
            }

            if (Config.EnableSCP703)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    Player Player = XHelper.GetRandomSpecialPlayer(RoleTypeId.Scientist);
                    if (Player != null)
                    {
                        SCP703 = new SCPHelper(Player, 120, "SCP-703", "cyan");

                        Player.ClearBroadcasts();

                        Player.SendBroadcast(TranslateConfig.SCP703SpawnBroadcast, 10, BroadcastFlags.Normal);
                    };
                });
            }

            if (Config.EnableSCP029)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);
                    if (player != null)
                    {
                        SCP029 = new SCPHelper(player, 120, "SCP-029", "red");

                        player.ClearBroadcasts();

                        player.SendBroadcast(TranslateConfig.SCP029SpawnBroadcast, 10, BroadcastFlags.Normal);

                        player.ClearInventory(false);

                        player.AddItem(ItemType.KeycardContainmentEngineer);
                        player.AddItem(ItemType.GunCOM18, ItemAddReason.AdminCommand);

                        player.EnableEffect<MovementBoost>(20);
                        player.EnableEffect<Scp1853>(2);
                        player.EnableEffect<DamageReduction>(15);

                        player.Health = 120;
                    };
                });
            }

            if (Config.SCP347)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Player player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (player != null)
                    {
                        SCP347 = new SCPHelper(player, RoleTypeId.Tutorial, "SCP-347", "red", XHelper.GetRandomSpawnLocation(RoleTypeId.FacilityGuard));

                        player.AddItem(ItemType.KeycardGuard);

                        player.SendBroadcast(TranslateConfig.SCP347SpawnBroadcast, 6);

                        player.EnableEffect<Invisible>();

                        player.GameObject.AddComponent<PlayerLightBehavior>();
                    }
                });
            }

            if (Config.SCP1093)
            {
                Timing.CallDelayed(1.4f, () =>
                {
                    Player player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (player != null)
                    {
                        SCP1093 = new SCPHelper(player, "SCP-1093", "yellow");

                        player.GameObject.AddComponent<PlayerGlowBehavior>();

                        player.SendBroadcast(TranslateConfig.SCP1093SpawnBroadcast, 6, BroadcastFlags.Normal);
                        player.GetPlayerUi().CommonHint.ShowOtherHint([.. TranslateConfig.SCP1093SkillIntroduction], 6);
                    }
                });
            }

            if (Config.SCP023)
            {
                Timing.CallDelayed(1.6f, () =>
                {
                    Player player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (player != null)
                    {
                        SCP023 = new SCPHelper(player, 200, "SCP-023", "red");

                        player.SendBroadcast(TranslateConfig.SCP023SpawnBroadcast, 6, BroadcastFlags.Normal);

                        player.GetPlayerUi().CommonHint.ShowOtherHint([.. TranslateConfig.SCP023SkillIntroduction], 6);

                        GhostlyAbility.Load(player);
                    }
                });
            }

            if (Config.EnableBaoAn)
            {
                Timing.CallDelayed(4f, () =>
                {
                    int randomNumber = Random.Next(3);
                    switch (randomNumber)
                    {
                        case 0:
                            foreach (Player player1 in XHelper.PlayerList)
                            {
                                if (player1.Role is RoleTypeId.FacilityGuard)
                                {
                                    player1.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.RemoteAdmin);

                                    player1.ClearInventory(false);
                                    player1.AddItem(ItemType.ArmorCombat);
                                    player1.AddItem(ItemType.KeycardMTFOperative);
                                    player1.AddItem(ItemType.Medkit);

                                    player1.AddItem(ItemType.GunAK, ItemAddReason.AdminCommand);

                                    player1.AddItem(ItemType.Radio);
                                }
                            }
                            Server.SendBroadcast(TranslateConfig.GuardMutinyBroadcast, 10, BroadcastFlags.Normal);
                            break;
                        case 1:
                            {
                                foreach (Player players in XHelper.PlayerList)
                                {
                                    if (players.Role is RoleTypeId.FacilityGuard)
                                    {
                                        players.ClearInventory(false);

                                        players.AddItem(ItemType.KeycardMTFPrivate);

                                        players.AddItem(ItemType.GunCrossvec, ItemAddReason.AdminCommand);

                                        players.AddItem(ItemType.ArmorCombat);
                                        players.AddItem(ItemType.Medkit);
                                        players.AddItem(ItemType.Adrenaline);
                                        players.AddItem(ItemType.GrenadeHE);
                                    }
                                }
                                Server.SendBroadcast(TranslateConfig.EliteGuardBroadcast, 10, BroadcastFlags.Normal);
                                break;
                            }
                        case 2:
                            {
                                Player guardPlayer = XHelper.GetRandomPlayer(RoleTypeId.FacilityGuard);
                                if (guardPlayer != null)
                                {
                                    guardPlayer.ClearInventory(false);

                                    guardPlayer.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.GuardCaptainSpawnBroadcast, 5);

                                    guardPlayer.AddItem(ItemType.ArmorHeavy);
                                    guardPlayer.AddItem(ItemType.Medkit);
                                    guardPlayer.AddItem(ItemType.Adrenaline);
                                    guardPlayer.AddItem(ItemType.GrenadeHE);

                                    guardPlayer.AddItem(ItemType.GunLogicer, ItemAddReason.AdminCommand);

                                    guardPlayer.AddItem(ItemType.KeycardMTFOperative);
                                }
                                break;
                            }
                    }
                });
            }
            if (Config.EnableFriendlyFire)
            {
                Server.FriendlyFire = false;
                Traverse.Create<AttackerDamageHandler>().Method("RefreshConfigs").GetValue();
            }
            Timing.CallDelayed(30f, () =>
            {
                Timing.RunCoroutine(XHelper.AutoXBroadcast());
            });
            Timing.CallDelayed(10f, () =>
            {
                if (Config.EnableAutoServerMessage)
                {
                    Timing.RunCoroutine(XHelper.AutoServerBroadcast());
                }
            });
            if (Config.SCP1068)
            {
                Timing.CallDelayed(2f, () =>
                {
                    foreach (var doors in UnityEngine.Object.FindObjectsOfType<RoomIdentifier>())
                    {
                        if (doors.Name is RoomName.Hcz096)
                        {
                            var item = XHelper.SpawnItem(ItemType.SCP2176, doors.transform.position);
                            SCP1068Id = item.Serial;
                        }
                    }
                });
            }
            if (Config.SCP1056)
            {
                Timing.CallDelayed(1f, () =>
                {
                    RoomIdentifier room173 = RoomIdentifier.AllRoomIdentifiers.Where(x => x.Name == RoomName.Lcz173).First();
                    var item = XHelper.SpawnItem(ItemType.Medkit, room173.transform.TransformPoint(new Vector3(-2.62f, 13.29f, -4.93f)));
                    SCP1056Id = item.Serial;
                });
            }
            Timing.RunCoroutine(SpecialRoleHelper.SpecialRoleInfoHandle());
        }


        public override void OnScp173AddingObserver(Scp173AddingObserverEventArgs ev)
        {
            Player target = ev.Target;
            Player player = ev.Player;
            if (target != null && player != null)
            {
                if (target.GetRoleName() == "SCP-191")
                {
                    ev.IsAllowed = false;
                }
            }
        }


        public override void OnScp096AddingTarget(Scp096AddingTargetEventArgs ev)
        {
            Player target = ev.Target;
            Player player = ev.Player;
            if (target != null && player != null)
            {
                var role = player.ReferenceHub.roleManager.CurrentRole as Scp096Role;
                if (target.GetRoleName() == "SCP-191")
                {
                    ev.IsAllowed = false;
                }
                if (SeePlayers.Contains(target) && !Scp096Enraging && role.IsAbilityState(Scp096AbilityState.None) && (role.IsRageState(Scp096RageState.Docile) || role.IsRageState(Scp096RageState.Distressed)))
                {
                    ev.IsAllowed = false;
                }
            }
        }

        public override void OnScp096Enraging(Scp096EnragingEventArgs ev)
        {
            Scp096Enraging = true;
        }

        public override void OnScp096ChangedState(Scp096ChangedStateEventArgs ev)
        {
            if (ev.State is Scp096RageState.Calming)
            {
                Scp096Enraging = false;
            }
        }


        public override void OnPlayerSearchedPickup(PlayerSearchedPickupEventArgs ev)
        {
            var item = ev.Pickup;
            Player player = ev.Player;

            Timing.CallDelayed(0.5f, () =>
            {
                if (item.Base.Info.Serial == SCP1068Id && item.Base.Info.ItemId is ItemType.SCP2176 && Config.SCP1068)
                {
                    player.RemoveItem(item);
                    var items = player.Inventory.ServerAddItem(ItemType.SCP2176 , ItemAddReason.AdminCommand);
                    SCP1068Base = items;
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1068PickupHint);
                }
                if (item.Base.Info.Serial == SCP1056Id && item.Base.Info.ItemId is ItemType.Medkit && Config.SCP1056)
                {
                    player.RemoveItem(item);
                    var items = player.AddItem(ItemType.Medkit);
                    SCP1056Base = items;
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1056PickupHint);
                }
            });
        }


        public override void OnPlayerSearchingPickup(PlayerSearchingPickupEventArgs ev)
        {
            if (!Round.IsRoundStarted)
            {
                ev.IsAllowed = false;
                return;
            }

            Player Player = ev.Player;
            var Item = ev.Pickup;

            if (Player != null)
            {
                if (Player.GetRoleName() == "SCP-347" && (Item.Base.Info.ItemId.IsWeapon() || (Config.NoT347 && Item.Base.Info.ItemId.IsThrowable())))
                {
                    ev.IsAllowed = false;
                }
                if (Player.GetRoleName() == "SCP-073" && Player.Team is Team.ChaosInsurgency)
                {
                    ev.IsAllowed = false;
                }
            }
        }


        public override void OnScp914ProcessedInventoryItem(Scp914ProcessedInventoryItemEventArgs ev)
        {
            Player player = ev.Player;

            if (player == null)
                return;

            if (player.GetRoleName() == "SCP-347")
            {
                Timing.CallDelayed(3f, () =>
                {
                    foreach (var item in player.ReferenceHub.inventory.UserInventory.Items)
                    {
                        if (item.Value.ItemTypeId.IsWeapon())
                        {
                            player.DropItem(item.Value);
                            player.EnableEffect<Flashed>(5);
                        }
                    }
                });
            }
        }


        public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
        {
            Player player = ev.Player;
            var item = ev.Item;

            if (SCP1056Base != null && item == SCP1056Base)
            {
                player.SetPlayerScale(Config.SCP1056X);
                player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1056UsedHint);
            }
        }

        public override void OnPlayerThrewProjectile(PlayerThrewProjectileEventArgs ev)
        {
            var item = ev.Item;

            if (SCP1068Base != null && item == SCP1068Base)
            {
                Server.SendBroadcast(TranslateConfig.SCP1068UsedBroadcast, 5, BroadcastFlags.Normal);
                Warhead.Shake();
            }
        }


        public override void OnServerRoundEnded(RoundEndedEventArgs ev)
        {
            SCP1068Id = 0;
            SCP1056Id = 0;
            SCP1068Base = null;
            SCP1056Base = null;
            SkynetPlayers.Clear();
            SeePlayers.Clear();

            if (Config.EnableRoundEndInfo)
            {
                foreach (Player player in XHelper.PlayerList)
                {
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.RoundEndInfo, 10);
                }
            }

            if (Config.EnableFriendlyFire)
            {
                Server.FriendlyFire = true;
                ConfigFile.ServerConfig.SetString("friendly_fire_multiplier", Config.OverriddenFriendlyFireMultiplier.ToString());

                if (Config.FriendlyFireNotifyingType is MessageType.Hint)
                {
                    foreach (Player player in XHelper.PlayerList)
                    {
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.FFMessage, 15);
                    }
                }
                else
                {
                    foreach (Player player in XHelper.PlayerList)
                    {
                        player.SendBroadcast(TranslateConfig.FFMessage, (ushort)Config.FFMessageDuration);
                    }
                }

                typeof(AttackerDamageHandler).GetMethod("RefreshConfigs", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, null);

                if (Config.EnableFFRoundEndRoleChange)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        foreach (Player player in XHelper.PlayerList)
                        {
                            player.SetRole(Config.FFRoundEndRole, RoleChangeReason.RemoteAdmin);
                        }
                    });
                }
            }
        }


        public override void OnPlayerSpawned(PlayerSpawnedEventArgs ev)
        {
            Player Player = ev.Player;
            RoleTypeId Role = ev.Role.RoleTypeId;

            if (Config.EnableRoundSupplies)
            {
                if (Role is RoleTypeId.ClassD)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        Player.AddItem(Config.ClassDCard, ItemAddReason.AdminCommand);
                    });
                }
            }

            if (Config.InfiniteAmmo)
            {
                if (Player.IsHuman)
                {
                    Timing.CallDelayed(0.5f, () =>
                    {
                        Player.SetAmmo(ItemType.Ammo9x19, 1);
                        Player.SetAmmo(ItemType.Ammo12gauge, 1);
                        Player.SetAmmo(ItemType.Ammo44cal, 1);
                        Player.SetAmmo(ItemType.Ammo762x39, 1);
                        Player.SetAmmo(ItemType.Ammo556x45, 1);
                    });
                }
            }

            if (Config.EnableChangeSCPHPSystem && Player.IsSCP)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (SCPHPChangeSystem.healthDict.TryGetValue(Role, out var health))
                    {
                        Player.Health = health;
                        Player.MaxHealth = health;
                    }
                });
            }

            if (Config.EnableSpectatorList)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Timing.RunCoroutine(SpectatorHelper.SpectatorList(Player).CancelWith(Player.GameObject));
                });
            }
        }


        public override void OnPlayerShotWeapon(PlayerShotWeaponEventArgs ev)
        {
            Player player = ev.Player;
            var Weapon = ev.Weapon;
            if (player != null && Config.SavePlayersInfo)
            {
                var pLog = player.GetLog();
                pLog.PlayerShot++;
                pLog.UpdateLog();
            }
        }


        public override void OnPlayerDying(PlayerDyingEventArgs ev)
        {
            Player player = ev.Player;
            Player attacker = ev.Attacker;

            if (player == null) return;

            Timing.CallDelayed(1f, () =>
            {
                switch (player.GetRoleName())
                {
                    case "SCP-703":
                        {
                            SCP703.OnPlayerDead(player, "SCP 7 0 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-703{TranslateConfig.SpecialRoleContainCassie}");
                            SCP703 = null;
                            break;
                        }
                    case "SCP-029":
                        {
                            SCP029.OnPlayerDead(player, "SCP 0 2 9 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-029{TranslateConfig.SpecialRoleContainCassie}");
                            SCP029 = null;
                            break;
                        }
                    case "SCP-191":
                        {
                            SCP191.OnPlayerDead(player, "SCP 1 9 1 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-191{TranslateConfig.SpecialRoleContainCassie}");
                            SCP191 = null;
                            break;
                        }
                    case "SCP-073":
                        {
                            SCP073.OnPlayerDead(player, "SCP 0 7 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-073{TranslateConfig.SpecialRoleContainCassie}");
                            SCP073 = null;
                            break;
                        }
                    case "SCP-023":
                        {
                            GhostlyAbility.Unload(player);
                            SCP023.OnPlayerDead(player, "SCP 0 2 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-023{TranslateConfig.SpecialRoleContainCassie}");
                            SCP023 = null;
                            break;
                        }
                    case "SCP-347":
                        {
                            player.DisableAllEffects();
                            UnityEngine.Object.Destroy(player.GameObject.GetComponent<PlayerLightBehavior>());
                            SCP347.OnPlayerDead(player, "SCP 3 4 7 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-347{TranslateConfig.SpecialRoleContainCassie}");
                            SCP347 = null;
                            break;
                        }
                    case "SCP-2936-1":
                        {
                            player.SetPlayerScale(1f);
                            SCP2936.OnPlayerDead(player, "SCP 2 9 3 6 1 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-2936-1{TranslateConfig.SpecialRoleContainCassie}");
                            SCP2936 = null;
                            break;
                        }
                    case "SCP-1093":
                        {
                            UnityEngine.Object.Destroy(player.GameObject.GetComponent<PlayerGlowBehavior>());
                            SCP1093.OnPlayerDead(player, "SCP 1 0 9 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-1093{TranslateConfig.SpecialRoleContainCassie}");
                            SCP1093 = null;
                            break;
                        }
                }

                if (player.GetRoleName() == TranslateConfig.ChaosLeaderRoleName)
                {
                    ChaosLeader.OnPlayerDead(player, "", TranslateConfig.ChaosLeaderDeathCassie);
                    foreach (Player Player in XHelper.PlayerList)
                    {
                        if (Player.Team is Team.ChaosInsurgency)
                        {
                            Player.DisableEffect<MovementBoost>();
                        }
                    }
                    ChaosLeader = null;
                }

                if (SkynetPlayers.Contains(player))
                {
                    SkynetPlayers.Remove(player);
                    player.CustomInfo = "";
                }

                if (SeePlayers.Contains(player))
                {
                    SeePlayers.Remove(player);
                    player.CustomInfo = "";
                }

                if (!player.DoNotTrack)
                {
                    var pLog = player.GetLog();
                    pLog.PlayerDeath++;
                    pLog.UpdateLog();
                }
            });

            if (player != null && attacker != null && Config.SavePlayersInfo && !attacker.DoNotTrack)
            {
                var Log = attacker.GetLog();
                if (player.IsSCP)
                {
                    Log.PlayerSCPKills++;
                    Log.UpdateLog();
                }
                else
                {
                    Log.PlayerKills++;
                    Log.UpdateLog();
                }
            }

            if (player != null && player.GetRoleName() == "SCP-073")
            {
                player.ClearInventory();
            }
        }


        public override void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs ev)
        {
            Player Player = ev.Player;
            DoorVariant Door = ev.Door.Base;

            if (Door.ActiveLocks > 0 && !Player.IsBypassEnabled)
                return;

            if (!Config.AffectDoors || Player.IsSCP || Player.IsWithoutItems() || Player.CurrentItem is KeycardItem)
                return;

            if (!Door.AllowInteracting(Player.ReferenceHub, 0))
                return;

            if (Door.HasKeycardPermission(Player))
            {
                ev.CanOpen = true;

                Door.Toggle(Player.ReferenceHub);
                ev.IsAllowed = false;
            }
        }


        public override void OnPlayerInteractingLocker(PlayerInteractingLockerEventArgs ev)
        {
            if (!Config.AffectLockers || ev.Player.IsSCP || ev.Player.IsWithoutItems() || ev.Player.CurrentItem is KeycardItem)
                return;

            if (ev.Chamber.Base.HasKeycardPermission(ev.Player))
            {
                ev.CanOpen = true;
                ev.Chamber.Base.Toggle(ev.Locker.Base);

                ev.IsAllowed = false;
            }
        }


        public override void OnPlayerInteractingGenerator(PlayerInteractingGeneratorEventArgs ev)
        {
            if (!Config.AffectGenerators || ev.Player.IsSCP || ev.Player.IsWithoutItems() ||
                ev.Player.CurrentItem is KeycardItem || ev.ColliderId != Scp079Generator.GeneratorColliderId.Door) return;

            if (!ev.Generator.Base.HasKeycardPermission(ev.Player) && !ev.Generator.Base.IsUnlocked())
            {
                ev.Generator.Base.Unlock();
                //ev.Generator.ServerGrantTicketsConditionally(new Footprint(ev.Player.ReferenceHub), 0.5f);
                //TODO:unsure
                /*if (ev.Player.Role.GetFaction() == Faction.FoundationStaff)
                {
                    RespawnTokensManager.OnPointsModified(Faction.FoundationStaff, 0.5f);
                }*/
                ev.Generator.Base._cooldownStopwatch.Restart();

                ev.IsAllowed = false;
            }
        }


        public override void OnPlayerDroppingItem(PlayerDroppingItemEventArgs ev)
        {
            if (!Round.IsRoundStarted)
            {
                ev.IsAllowed = false;
                return;
            }

            if (ev.Player.GetRoleName() == "SCP-073" && ev.Player.Team is Team.ChaosInsurgency)
                ev.IsAllowed = false;
        }


        public override void OnPlayerThrowingItem(PlayerThrowingItemEventArgs ev)
        {
            if (!Round.IsRoundStarted)
            {
                ev.IsAllowed = false;
                return;
            }
        }


        public override void OnPlayerDroppingAmmo(PlayerDroppingAmmoEventArgs ev)
        {
            if (Config.InfiniteAmmo)
            {
                ev.IsAllowed = false;
                return;
            }
        }


        public override void OnPlayerInteractedGenerator(PlayerInteractedGeneratorEventArgs ev)
        {
            Player player = ev.Player;
            var generator = ev.Generator;

            if (SkynetPlayers.Contains(player))
            {
                generator.Base.Network_syncTime = 50;
                generator.Base._totalActivationTime = 50;
                generator.Base._totalDeactivationTime = 50;
                generator.Base._prevTime = 50;
            }
        }


        public override void OnServerCommandExecuted(CommandExecutedEventArgs ev)
        {
            var sender = ev.Sender;
            var command = ev.Command.Command;

            Player player = Player.Get(sender);

            if (player != null && !string.IsNullOrEmpty(command))
            {
                if (!player.RemoteAdminAccess) return;

                string note = TranslateConfig.AdminLog.Replace("%Nickname%", player.Nickname).Replace("%Time%", DateTime.Now.ToString()).Replace("%Command%", command).Replace("%UserId%", player.UserId);
                if (Config.AdminLogShow)
                    Server.SendBroadcast(TranslateConfig.AdminLogBroadcast.Replace("%Nickname%", player.Nickname).Replace("%Command%", command), 5, BroadcastFlags.Normal);
                Log.Info(note);
                try
                {
                    if (!File.Exists(Config.AdminLogPath))
                    {
                        FileStream fs1 = new(Config.AdminLogPath, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new(fs1);
                        sw.WriteLine(note);
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs = new(Config.AdminLogPath, FileMode.Append, FileAccess.Write);
                        StreamWriter sr = new(fs);
                        sr.WriteLine(note);
                        sr.Close();
                        fs.Close();
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }

        public override void OnServerRoundRestarted()
        {
            SCP029 = null;
            SCP073 = null;
            SCP1093 = null;
            SCP191 = null;
            SCP2936 = null;
            SCP347 = null;
            SCP703 = null;
            SCP023 = null;

            XHelper.PlayerList.Clear();
            XHelper.SpecialPlayerList.Clear();

            SpecialRoleHelper.Reset();
        }


        public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
        {
            Player player = ev.Player;
            var oldRole = ev.OldRole;
            var newRole = ev.NewRole.RoleTypeId;
            if (player == null || string.IsNullOrEmpty(player.UserId)) return;
            Timing.CallDelayed(3f, () =>
            {
                if (!(oldRole is RoleTypeId.Scp079 && newRole is RoleTypeId.Spectator && Config.SCP191))
                    return;

                SCP191 = new SCPHelper(player, RoleTypeId.Tutorial, 120, "SCP-191", "red");

                player.SetPlayerScale(0.8f);

                player.SendBroadcast(TranslateConfig.SCP191SpawnBroadcast, 6);

                player.AddItem(ItemType.ArmorCombat);
                player.AddItem(ItemType.GunFSP9, ItemAddReason.AdminCommand);
                player.AddItem(ItemType.Medkit);

                Timing.CallDelayed(0.5f, () =>
                {
                    if (XHelper.PlayerList.Count(x => x.Team is Team.SCPs) > 0)
                    {
                        Player scp = XHelper.PlayerList.Where(x => x.IsSCP).ToList().RandomItem();
                        player.Position = scp.Position + Vector3.up * 1;
                    }
                    else
                    {
                        player.Position = XHelper.GetRandomSpawnLocation(RoleTypeId.NtfCaptain);
                    }
                });
            });

            if (Config.SavePlayersInfo && !player.DoNotTrack && newRole is not RoleTypeId.Spectator)
            {
                var pLog = player.GetLog();
                pLog.RolePlayed++;
                pLog.UpdateLog();
            }
        }
    }
}
