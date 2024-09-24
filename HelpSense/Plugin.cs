using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using HarmonyLib;
using GameCore;
using InventorySystem;
using PlayerStatsSystem;
using PluginAPI.Events;
using System;
using Log = PluginAPI.Core.Log;
using InventorySystem.Items.Firearms;
using MapGeneration.Distributors;
using Respawning;
using HelpSense.Helper;
using System.Reflection;
using InventorySystem.Items.Pickups;
using Mirror;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items.Keycards;
using Serialization;
using System.IO;
using HelpSense.API.Features;
using InventorySystem.Items;
using HelpSense.Handler;
using Footprinting;
using InventorySystem.Items.Firearms.Attachments;
using PlayerRoles.Voice;
using MapGeneration;
using PluginAPI.Core.Zones;
using System.Threading.Tasks;
using PlayerRoles.PlayableScps.Scp079;
using Interactables.Interobjects;
using InventorySystem.Items.ThrowableProjectiles;
using HelpSense.ConfigSystem;
using UnityEngine.UIElements;
using PlayerRoles.Ragdolls;
using CommandSystem;
using static Broadcast;
using InventorySystem.Items.Firearms.BasicMessages;
using HelpSense.Patches;
using InventorySystem.Items.Usables;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.PlayableScps.Scp106;
using PlayerRoles.PlayableScps.Scp173;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using LiteDB;
using PluginAPI.Commands;
using HelpSense.API;
using HelpSense.MonoBehaviors;

namespace HelpSense
{
    public class Plugin
    {
        Harmony Harmony = new("cn.xlittleleft.plugin");

        public LiteDatabase db;

        public System.Random Random = new System.Random();

        public static string RespawnTimerDirectoryPath { get; private set; }

        public Vector3 LobbyPos;

        public static LobbyLocationType curLobbyLocationType;

        public CoroutineHandle lobbyTimer;

        public string text;

        public static bool IsLobby = false;

        public SCPHelper SCP703;

        public SCPHelper SCP029;

        public SCPHelper SCP191;

        public SCPHelper SCP073;

        public SCPHelper SCP347;

        public SCPHelper ChaosLeader;
        public bool SpawnLeader = false;

        public SCPHelper SCP2936;
        public bool SpawnSCP2936 = false;

        public SCPHelper SCP1093;

        public HashSet<Player> SkynetPlayers = new HashSet<Player>();
        public bool SkynetSpawned = false;

        public HashSet<Player> SeePlayers = new HashSet<Player>();
        public bool SeeSpawned = false;
        public bool Scp096Enraging = false;

        public ushort scp1068id = 0;
        public ItemBase scp1068base;

        public ushort scp1056id = 0;
        public ItemBase scp1056base;
      
        public static System.Version PluginVersion => new System.Version(1, 3, 4);
        public static DateTime LastUpdateTime => new DateTime(2024, 08, 07, 8, 8, 0);
        public static System.Version RequiredGameVersion => new System.Version(13, 5, 1);
      
        [PluginEntryPoint("HelpSense", "1.3.5", "HelpSense综合服务器插件", "X小左")]
      
        void LoadPlugin()
        {
            Instance = this;

            if (Config.SavePlayersInfo)
            {
                db = new LiteDatabase(Config.SavePath);
            }

            EventManager.RegisterEvents(this);

            Harmony.PatchAll();

            if (Config.EnableRespawnTimer)
            {
                RespawnTimerDirectoryPath = PluginHandler.Get(this).PluginDirectoryPath;
            }
        }

        public static Plugin Instance { get; private set; }

        [PluginEvent]
        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
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
                        IsLobby = true;

                        LobbyHelper.SpawnManager();
                        GameObject.Find("StartRound").transform.localScale = Vector3.zero;

                        if (lobbyTimer.IsRunning)
                        {
                            Timing.KillCoroutines(lobbyTimer);
                        }

                        if (curLobbyLocationType == LobbyLocationType.Intercom && Config.DisplayInIcom) lobbyTimer = Timing.RunCoroutine(LobbyHelper.LobbyIcomTimer());
                        else lobbyTimer = Timing.RunCoroutine(LobbyHelper.LobbyTimer());
                    });
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnWaitingForPlayers] " + e.ToString());
                }
            }
            if (Config.EnableFriendlyFire)
            {
                AttackerDamageHandler.RefreshConfigs();
                Server.FriendlyFire = false;
            }
            if (Config.EnableRespawnTimer)
            {
                if (Config.Timers.IsEmpty())
                {
                    Log.Error("列表为空");
                    return;
                }

                string chosenTimerName = Config.Timers[UnityEngine.Random.Range(0, Config.Timers.Count)];
                TimerView.GetNew(chosenTimerName);
            }
        }

        [PluginEvent]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
            var Player = ev.Player;

            if (Player == null || string.IsNullOrEmpty(Player.UserId)) return;
            XHelper.PlayerList.Add(Player);
            XHelper.SpecialPlayerList.Add(Player);
            XHelper.HintProviderList.Add(HintProviderHelper.CreateHintProvider(Player));

            if (Config.SavePlayersInfo)
            {
                var Log = Player.GetLog();
                Log.NickName = Player.Nickname;
                Log.LastJoinedTime = DateTime.Now;
                Log.UpdateLog();
            }

            if (Config.SavePlayersInfo && Player.DoNotTrack)
            {
                Player.SendBroadcast(TranslateConfig.DNTWarning, 10);
            }

            if (Config.EnableRoundWaitingLobby)
            {
                try
                {
                    if (IsLobby && (RoundStart.singleton.NetworkTimer > 1 || GameCore.RoundStart.singleton.NetworkTimer == -2))
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            Player.SetRole(Config.LobbyPlayerRole);

                            Player.IsGodModeEnabled = true;

                            if (Config.LobbyInventory.Count > 0 && curLobbyLocationType != LobbyLocationType.Chaos)
                            {
                                foreach (var item in Config.LobbyInventory)
                                {
                                    Player.AddItem(item);
                                }
                            }
                            if (curLobbyLocationType is LobbyLocationType.Chaos)
                            {
                                Player.AddItem(ItemType.ArmorHeavy);
                                Player.AddItem(ItemType.GunAK);
                                Player.AddItem(ItemType.GunCrossvec);
                                Player.AddItem(ItemType.GunE11SR);
                                Player.AddItem(ItemType.GunFRMG0);
                                Player.AddItem(ItemType.GunLogicer);
                                Player.AddItem(ItemType.GunRevolver);
                                Player.AddItem(ItemType.GunShotgun);
                                Player.SetAmmo(ItemType.Ammo9x19, (ushort)Player.GetAmmoLimit(ItemType.Ammo9x19));
                                Player.SetAmmo(ItemType.Ammo12gauge, (ushort)Player.GetAmmoLimit(ItemType.Ammo12gauge));
                                Player.SetAmmo(ItemType.Ammo762x39, (ushort)Player.GetAmmoLimit(ItemType.Ammo762x39));
                                Player.SetAmmo(ItemType.Ammo556x45, (ushort)Player.GetAmmoLimit(ItemType.Ammo556x45));
                                Player.SetAmmo(ItemType.Ammo44cal, (ushort)Player.GetAmmoLimit(ItemType.Ammo44cal));
                            }
                        });

                        Timing.CallDelayed(0.6f, () =>
                        {
                            Player.Position = LobbyLocationHandler.LobbyPosition;
                            Player.Rotation = LobbyLocationHandler.LobbyRotation.eulerAngles;

                            Player.EffectsManager.EnableEffect<MovementBoost>();
                            Player.EffectsManager.ChangeState<MovementBoost>(Config.MovementBoostIntensity);
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnPlayerJoin] " + e.ToString());
                }
            }
            if (Config.EnableWelcomeInfo)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player.GetHintProvider().ShowHint(TranslateConfig.WelcomeMessage.Replace("%playername%", Player.Nickname), Config.WelcomeTime);
                });
            }
        }

        [PluginEvent]
        bool OnDamage(PlayerDamageEvent ev)
        {
            var Target = ev.Target;
            var Attacker = ev.Player;
            var DamageHandler = ev.DamageHandler;
            if (Target != null && Attacker != null)
            {
                if (Target.RoleName == "SCP-1093" && DamageHandler is AttackerDamageHandler attackerDamageHandler)
                {
                    if (attackerDamageHandler.Hitbox is HitboxType.Headshot)
                    {
                        attackerDamageHandler.Damage = 0;
                    }
                }
                if (Target.RoleName == "SCP-191" && (Attacker.IsHuman && Attacker.CurrentItem.ItemTypeId.IsWeapon()) && DamageHandler is StandardDamageHandler standard)
                {
                    standard.Damage = Config.SCP191Ammo;
                }
                if (Target.RoleName == "SCP-073" && Attacker.Team != Target.Team && Attacker.IsHuman && Target.Team is not Team.ChaosInsurgency)
                {
                    Attacker.Damage(Config.SCP073RRD, TranslateConfig.SCP073DamageReason);
                }
                else if (Attacker.Team != Target.Team && Attacker.IsSCP && Target.RoleName == "SCP-073" && Target.Team is not Team.ChaosInsurgency)
                {
                    Attacker.Damage(Config.SCP073SCPRD, TranslateConfig.SCP073DamageReason);
                }
                if ((Attacker.RoleName == "SCP-191" && Target.Team is Team.SCPs) || (Target.RoleName == "SCP-191" && Attacker.Team is Team.SCPs))
                {
                    return false;
                }
                if (DamageHandler is StandardDamageHandler standardDamage && Attacker.Team != Target.Team)
                {
                    if (Config.SavePlayersInfo)
                    {
                        var PLog = Attacker.GetLog();
                        PLog.PlayerDamage += standardDamage.Damage;
                        PLog.UpdateLog();
                    }
                    if (Attacker.Role is RoleTypeId.Scp096 && SeePlayers.Contains(Target))
                    {
                        standardDamage.Damage = 40;
                    }
                }
            }
            return true;
        }

        [PluginEvent]
        void OnPlayerLeft(PlayerLeftEvent ev)
        {
            var Player = ev.Player;
            
            if (Player == null || string.IsNullOrEmpty(Player.UserId)) return;

            if (Config.SavePlayersInfo)
            {
                var Log = Player.GetLog();
                Log.LastLeftTime = DateTime.Now;
                Log.UpdateLog();
            }

            XHelper.PlayerList.Remove(Player);
            XHelper.SpecialPlayerList.Remove(Player);
        }

        [PluginEvent]
        void OnPlayerEscape(PlayerEscapeEvent ev)
        {
            var Player = ev.Player;
            var NewRole = ev.NewRole;

            if (Player.RoleName == "SCP-029")
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player.EffectsManager.ChangeState<MovementBoost>(25);
                    Player.EffectsManager.ChangeState<Scp1853>(2);
                    Player.EffectsManager.ChangeState<DamageReduction>(15);
                    if (NewRole == RoleTypeId.ChaosConscript)
                    {
                        Player.AddItem(ItemType.SCP268);
                        Player.GetHintProvider().ShowHint(TranslateConfig.SCP029EscapeHint, 3f);
                    }
                });
            }
            if (Player.RoleName == "SCP-703")
            {
                Timing.CallDelayed(1f, () =>
                {
                    if (NewRole == RoleTypeId.NtfSpecialist)
                    {
                        var firearm = Player.ReferenceHub.inventory.ServerAddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                        firearm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());
                        Player.GetHintProvider().ShowHint(TranslateConfig.SCP703EscapeHint, 3f);
                    }
                });
            }
        }

        [PluginEvent]
        void OnSpecialTeamRespawn(TeamRespawnEvent ev)
        {
            var Team = ev.Team;
            var Players = ev.Players;

            Timing.CallDelayed(0.5f, () =>
            {
                if (Config.EnableSkynet && !SkynetSpawned && Team == SpawnableTeamType.NineTailedFox)
                {
                    Player player079 = XHelper.PlayerList.Where(x => x.Role is RoleTypeId.Scp079).FirstOrDefault();
                    int player079leave = 0;
                    if (player079 != null)
                    {
                        Scp079Role scp079 = player079.ReferenceHub.roleManager.CurrentRole as Scp079Role;
                        scp079.SubroutineModule.TryGetSubroutine(out Scp079TierManager tier);
                        player079leave = tier.AccessTierLevel;
                    }
                    if (player079leave >= 3)
                    {
                        SkynetSpawned = true;
                        Cassie.Clear();
                        XHelper.MessageTranslated($"MTFUnit Kappa , 10 , and , Mu , 7 , designated scan neck , HasEntered , they will help contain scp 0 7 9 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", TranslateConfig.SkynetCassie.Replace("%SCPNum%" , XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));

                        foreach (Player Player in Players)
                        {
                            SkynetPlayers.Add(Player);
                            if (Player.Role is not RoleTypeId.NtfCaptain)
                                Player.AddItem(ItemType.SCP2176);
                            switch (Player.Role)
                            {
                                case RoleTypeId.NtfPrivate:
                                    Player.Mybroadcast($"{TranslateConfig.SkynetPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    Player.CustomInfo = TranslateConfig.SkynetPrivateCustomInfo;
                                    break;
                                case RoleTypeId.NtfSergeant:
                                    Player.Mybroadcast($"{TranslateConfig.SkynetSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    Player.CustomInfo = TranslateConfig.SkynetSergeantCustomInfo;
                                    break;
                                case RoleTypeId.NtfCaptain:
                                    Player.Mybroadcast($"{TranslateConfig.SkynetCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    Player.CustomInfo = TranslateConfig.SkynetCaptainCustomInfo;
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
                            if (XHelper.PlayerList.Where(x => x.Role is RoleTypeId.Scp096).FirstOrDefault() != null)
                            {
                                Cassie.Clear();
                                XHelper.MessageTranslated($"MTFUnit Eta , 10 , designated see no evil , HasEntered , they will help contain scp 0 9 6 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", TranslateConfig.SeeNoEvilCassie.Replace("%SCPNum%", XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));
                                SeeSpawned = true;
                                foreach (Player Player in Players)
                                {
                                    SeePlayers.Add(Player);
                                    switch (Player.Role)
                                    {
                                        case RoleTypeId.NtfPrivate:
                                            Player.Mybroadcast($"{TranslateConfig.SeeNoEvilPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            Player.CustomInfo = TranslateConfig.SeeNoEvilPrivateCustomInfo;
                                            break;
                                        case RoleTypeId.NtfSergeant:
                                            Player.Mybroadcast($"{TranslateConfig.SeeNoEvilSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            Player.CustomInfo = TranslateConfig.SeeNoEvilSergeantCustomInfo;
                                            break;
                                        case RoleTypeId.NtfCaptain:
                                            Player.Mybroadcast($"{TranslateConfig.SeeNoEvilCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            Player.CustomInfo = TranslateConfig.SeeNoEvilCaptainCustomInfo;
                                            break;
                                    }
                                    Player.Position = RoomIdentifier.AllRoomIdentifiers.Where(x => x.Name is RoomName.Outside).FirstOrDefault().transform.TransformPoint(62.93f, -8.35f, -51.26f);
                                }
                            }
                        }
                    });
                }
            });
        }

        [PluginEvent]
        void OnTeamRespawn(TeamRespawnEvent ev)
        {
            var Team = ev.Team;
            var Players = ev.Players;

            var SpecialPlayers = ev.Players;

            if (Config.SpawnHID)
            {
                Timing.CallDelayed(2f, () =>
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Role is RoleTypeId.NtfCaptain)
                        {
                            var Firaerm = Player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                            Firaerm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, Firaerm.GetCurrentAttachmentsCode());
                        }
                    }
                });
            }
            if (Config.EnableChaosLeader && !SpawnLeader)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var Player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRifleman, SpecialPlayers);
                    if (Player != null)
                    {
                        SpecialPlayers.Remove(Player);

                        SpawnLeader = true;

                        ChaosLeader = new SCPHelper(Player, 150, TranslateConfig.ChaosLeaderRoleName, "green");

                        Player.ClearBroadcasts();

                        XHelper.Mybroadcast(Player, TranslateConfig.ChaosLeaderSpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        var firearm = Player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                        firearm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());

                        Player.AddItem(ItemType.SCP1853);
                        Player.AddItem(ItemType.SCP268);

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
            if (Config.SCP2936 && !SpawnSCP2936)
            {
                Timing.CallDelayed(1f, () =>
                {
                    Player Player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRepressor, SpecialPlayers);
                    
                    if (Player != null)
                    {
                        SpawnSCP2936 = true;

                        SpecialPlayers.Remove(Player);

                        SCP2936 = new SCPHelper(Player, 300, "SCP-2936-1", "red");

                        Player.SendBroadcast(TranslateConfig.SCP29361SpawnBroadcast, 6);

                        Player.SetPlayerScale(2f);
                    }
                });
            }
            if (Config.SCP073 && XHelper.PlayerList.Where(x => x.RoleName == "SCP-073").FirstOrDefault() == null)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Player Player = XHelper.GetRandomPlayer(SpecialPlayers);

                    if (Player != null)
                    {
                        SpecialPlayers.Remove(Player);

                        SCP073 = new SCPHelper(Player, 120, "SCP-073", "green");

                        if (Player.Team is PlayerRoles.Team.ChaosInsurgency)
                        {
                            Player.SendBroadcast(TranslateConfig.SCP073AbelSpawnBroadcast, 6);

                            Player.ClearInventory();
                            for (int i = 0; i < 8; i++)
                            {
                                Player.AddItem(ItemType.Jailbird);
                            }

                            Timing.RunCoroutine(XHelper.PositionCheckerCoroutine(Player).CancelWith(Player.GameObject));
                        }
                        else
                        {
                            Player.SendBroadcast(TranslateConfig.SCP073CainSpawnBroadcast, 6);
                        }
                        Player.EffectsManager.EnableEffect<DamageReduction>();
                        Player.EffectsManager.ChangeState<DamageReduction>(Config.SCP073DD);
                        Player.EffectsManager.EnableEffect<MovementBoost>();
                        Player.EffectsManager.ChangeState<MovementBoost>(10);
                    }
                });
            }
        }

        [PluginEvent]
        public void OnRoundStarted(RoundStartEvent ev)
        {
            if (Config.SavePlayersInfo)
            {
                Timing.RunCoroutine(InfoExtension.CollectInfo());
                Log.Debug("开始收集玩家信息");
            }
            if (Config.EnableSCP703)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    var Player = XHelper.GetRandomSpecialPlayer(RoleTypeId.Scientist);
                    if (Player != null)
                    {
                        SCP703 = new SCPHelper(Player, 120, "SCP-703", "cyan");

                        Player.ClearBroadcasts();

                        XHelper.Mybroadcast(Player, TranslateConfig.SCP703SpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        Player.GetHintProvider().ShowHint(TranslateConfig.SCP703SkillIntroduction, 10);

                        Timing.RunCoroutine(XHelper.RandomItem(Player).CancelWith(Player.GameObject));
                    };
                });
            }
            if (Config.EnableSCP029)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var Player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);
                    if (Player != null)
                    {
                        SCP029 = new SCPHelper(Player, 120, "SCP-029", "red");

                        Player.ClearBroadcasts();

                        XHelper.Mybroadcast(Player, TranslateConfig.SCP029SpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        Player.GetHintProvider().ShowHint(TranslateConfig.SCP029SkillIntroduction, 10);

                        Player.ClearInventory();

                        Player.AddItem(ItemType.KeycardContainmentEngineer);
                        var firearm = Player.ReferenceHub.inventory.ServerAddItem(ItemType.GunCOM18);
                        ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                        Player.AddAmmo(ItemType.Ammo9x19, 30);

                        Player.EffectsManager.EnableEffect<MovementBoost>();
                        Player.EffectsManager.ChangeState<MovementBoost>(20);
                        Player.EffectsManager.EnableEffect<Scp1853>();
                        Player.EffectsManager.ChangeState<Scp1853>(2);
                        Player.EffectsManager.EnableEffect<DamageReduction>();
                        Player.EffectsManager.ChangeState<DamageReduction>(15);

                        Player.Health = 120;
                    };
                });
            }
            if (Config.SCP347)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    var Player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (Player != null)
                    {
                        SCP347 = new SCPHelper(Player, RoleTypeId.Tutorial, "SCP-347", "red", XHelper.GetRandomSpawnLocation(RoleTypeId.FacilityGuard));

                        Player.AddItem(ItemType.KeycardGuard);

                        Player.SendBroadcast(TranslateConfig.SCP347SpawnBroadcast, 6);

                        Player.EffectsManager.EnableEffect<Invisible>();

                        Timing.RunCoroutine(XHelper.SCP347Handle(Player).CancelWith(Player.GameObject));
                        Player.GameObject.AddComponent<PlayerLightBehavior>();
                    }
                });
            }
            if (Config.SCP1093)
            {
                Timing.CallDelayed(1.4f, () =>
                {
                    var Player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (Player != null)
                    {
                        SCP1093 = new SCPHelper(Player , "SCP-1093" , "yellow");

                        Player.GameObject.AddComponent<PlayerGlowBehavior>();

                        Player.SendBroadcast(TranslateConfig.SCP1093SpawnBroadcast, 6, BroadcastFlags.Normal);
                        Player.GetHintProvider().ShowHint(TranslateConfig.SCP1093SkillIntroduction, 6);
                    }
                });
            }

            if (Config.EnableBaoAn)
            {
                Timing.CallDelayed(4f, () =>
                {
                    int RandomNumber = Random.Next(3);
                    switch (RandomNumber)
                    {
                        case 0:
                            foreach (Player player1 in XHelper.PlayerList)
                            {
                                if (player1.Role is RoleTypeId.FacilityGuard)
                                {
                                    player1.SetRole(RoleTypeId.ChaosConscript, RoleChangeReason.RemoteAdmin);

                                    player1.ClearInventory();
                                    player1.AddItem(ItemType.ArmorCombat);
                                    player1.AddItem(ItemType.KeycardMTFOperative);
                                    player1.AddItem(ItemType.Medkit);

                                    var firearm = player1.AddItem(ItemType.GunAK);
                                    ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                                    player1.AddAmmo(ItemType.Ammo762x39, (ushort)player1.GetAmmoLimit(ItemType.Ammo762x39));

                                    player1.AddItem(ItemType.Radio);
                                }
                            }
                            XHelper.Allbroadcast(TranslateConfig.GuardMutinyBroadcast, 10, BroadcastFlags.Normal);
                            break;
                        case 1:
                            {
                                foreach (Player players in XHelper.PlayerList)
                                {
                                    if (players.Role is RoleTypeId.FacilityGuard)
                                    {
                                        players.ClearInventory();

                                        players.AddItem(ItemType.KeycardMTFPrivate);

                                        var firearm = players.AddItem(ItemType.GunCrossvec);
                                        ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                                        players.AddAmmo(ItemType.Ammo9x19, (ushort)players.GetAmmoLimit(ItemType.Ammo9x19));

                                        players.AddItem(ItemType.ArmorCombat);
                                        players.AddItem(ItemType.Medkit);
                                        players.AddItem(ItemType.Adrenaline);
                                        players.AddItem(ItemType.GrenadeHE);
                                    }
                                }
                                XHelper.Allbroadcast(TranslateConfig.EliteGuardBroadcast, 10, BroadcastFlags.Normal);
                                break;
                            }
                        case 2:
                            {
                                Player anbaoplayer = XHelper.GetRandomPlayer(RoleTypeId.FacilityGuard);
                                if (anbaoplayer != null)
                                {
                                    anbaoplayer.ClearInventory();

                                    anbaoplayer.GetHintProvider().ShowHint(TranslateConfig.GuardCaptainSpawnBroadcast, 5);

                                    anbaoplayer.AddItem(ItemType.ArmorHeavy);
                                    anbaoplayer.AddItem(ItemType.Medkit);
                                    anbaoplayer.AddItem(ItemType.Adrenaline);
                                    anbaoplayer.AddItem(ItemType.GrenadeHE);

                                    var firearm = anbaoplayer.AddItem(ItemType.GunLogicer);
                                    ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                                    anbaoplayer.SetAmmo(ItemType.Ammo762x39, (ushort)anbaoplayer.GetAmmoLimit(ItemType.Ammo762x39));

                                    anbaoplayer.AddItem(ItemType.KeycardMTFOperative);
                                }
                                break;
                            }
                    }
                });
            }
            if (Config.EnableRoundWaitingLobby)
            {
                try
                {
                    IsLobby = false;

                    if (!string.IsNullOrEmpty(IntercomDisplay._singleton.Network_overrideText)) IntercomDisplay._singleton.Network_overrideText = "";

                    foreach (var player in XHelper.PlayerList)
                    {
                        player.ClearInventory();

                        player.SetRole(RoleTypeId.None);

                        Timing.CallDelayed(0.25f, () =>
                        {
                            player.IsGodModeEnabled = false;
                            player.EffectsManager.DisableEffect<MovementBoost>();
                        });
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[HelpSense] [Event: OnRoundStarted] " + e.ToString());
                }
            }
            if (Config.EnableFriendlyFire)
            {
                Server.FriendlyFire = false;
                Traverse.Create<AttackerDamageHandler>().Method("RefreshConfigs").GetValue();
            }
            if (Config.EnableRespawnTimer)
            {
                Timing.RunCoroutine(RespawnHelper.TimerCoroutine());
            }
            Timing.CallDelayed(30f, () =>
            {
                Timing.RunCoroutine(XHelper.AutoX());
            });
            Timing.CallDelayed(10f, () =>
            {
                if (Config.EnableAutoServerMessage)
                {
                    Timing.RunCoroutine(XHelper.AutoSX());
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
                            scp1068id = item.Serial;
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
                    scp1056id = item.Serial;
                });
            }
        }

        [PluginEvent]
        bool OnScp173(Scp173NewObserverEvent ev)
        {
            var Target = ev.Target;
            var Player = ev.Player;
            if (Target != null && Player != null)
            {
                if (Target.RoleName == "SCP-191")
                {
                    return false;
                }
            }
            return true;
        }

        [PluginEvent]
        bool OnScp096(Scp096AddingTargetEvent ev)
        {
            var Target = ev.Target;
            var Player = ev.Player;
            if (Target != null && Player != null)
            {
                var Role = Player.ReferenceHub.roleManager.CurrentRole as Scp096Role;
                if (Target.RoleName == "SCP-191")
                {
                    return false;
                }
                if (SeePlayers.Contains(Target) && !Scp096Enraging && Role.IsAbilityState(Scp096AbilityState.None) && (Role.IsRageState(Scp096RageState.Docile) || Role.IsRageState(Scp096RageState.Distressed)))
                {
                    return false;
                }
            }
            return true;
        }

        [PluginEvent]
        void OnScp096Enraging(Scp096EnragingEvent ev)
        {
            Scp096Enraging = true;
        }

        [PluginEvent]
        void OnScp096StopCrying(Scp096ChangeStateEvent ev)
        {
            if (ev.RageState is Scp096RageState.Calming)
            {
                Scp096Enraging = false;
            }
        }

        [PluginEvent]
        bool OnPlayerSearchedPickup(PlayerSearchedPickupEvent ev)
        {
            var Item = ev.Item;
            var Player = ev.Player;

            Timing.CallDelayed(0.5f, () =>
            {
                if (Item.Info.Serial == scp1068id && Item.Info.ItemId is ItemType.SCP2176 && Config.SCP1068)
                {
                    Player.RemoveItem(Item);
                    var items = Player.AddItem(ItemType.SCP2176);
                    scp1068base = items;
                    Player.GetHintProvider().ShowHint(TranslateConfig.SCP1068PickupHint);
                }
                if (Item.Info.Serial == scp1056id && Item.Info.ItemId is ItemType.Medkit && Config.SCP1056)
                {
                    Player.RemoveItem(Item);
                    var items = Player.AddItem(ItemType.Medkit);
                    scp1056base = items;
                    Player.GetHintProvider().ShowHint(TranslateConfig.SCP1056PickupHint);
                }
            });
            return true;
        }

        [PluginEvent]
        bool OnPlayerPick(PlayerSearchPickupEvent ev)
        {
            var Player = ev.Player;
            var Item = ev.Item;

            if (Player != null)
            {
                if (Player.RoleName == "SCP-347" && (Item.Info.ItemId.IsWeapon() || (Config.NoT347 && Item.Info.ItemId.IsThrowable())))
                {
                    return false;
                }
                if (Player.RoleName == "SCP-073" && Player.Team is Team.ChaosInsurgency)
                {
                    return false;
                }
            }
            return true;
        }

        [PluginEvent]
        void OnScp914(Scp914InventoryItemUpgradedEvent ev)
        {
            var Player = ev.Player;
            var Item = ev.Item;

            if (Player == null) return;

            if (Player.RoleName == "SCP-347")
            {
                Timing.CallDelayed(3f, () =>
                {
                    foreach (var Items in Player.ReferenceHub.inventory.UserInventory.Items)
                    {
                        if (Items.Value.ItemTypeId.IsWeapon())
                        {
                            Player.DropItem(Items.Value);
                            Player.EffectsManager.EnableEffect<Flashed>(5);
                        }
                    }
                });
            }
        }

        [PluginEvent]
        void OnPlayerUsedItem(PlayerUsedItemEvent ev)
        {
            var Player = ev.Player;
            var Item = ev.Item;

            if (scp1056base != null && Item == scp1056base)
            {
                Player.SetPlayerScale(Config.SCP1056X);
                Player.GetHintProvider().ShowHint(TranslateConfig.SCP1056UsedHint);
            }
        }

        [PluginEvent]
        void OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
        {
            var Player = ev.Thrower;
            var Item = ev.Item;

            if (scp1068base != null && Item == scp1068base)
            {
                XHelper.Allbroadcast(TranslateConfig.SCP1068UsedBroadcast, 5, BroadcastFlags.Normal);
                Server.Instance.GetComponent<AlphaWarheadController>(globalSearch: true).RpcShake(true);
            }//沙比NW写空壳核弹抖动我直接自己写一个
        }

        [PluginEvent]
        void OnRoundEnd(RoundEndEvent ev)
        {
            scp1068id = 0;
            scp1056id = 0;
            scp1068base = null;
            scp1056base = null;
            SkynetPlayers.Clear();
            SeePlayers.Clear();
            if (Config.EnableRoundEndInfo)
            {
                foreach (Player player in XHelper.PlayerList)
                {
                    player.GetHintProvider().ShowHint(TranslateConfig.RoundEndInfo, 10);
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
                        player.GetHintProvider().ShowHint(TranslateConfig.FFMessage, 15);
                    }
                }
                else
                {
                    Server.Init();

                    foreach (Player player in XHelper.PlayerList)
                    {
                        player.SendBroadcast(TranslateConfig.FFMessage, (ushort)Config.FFMessageDuration);
                    }
                }
                typeof(AttackerDamageHandler).GetMethod("RefreshConfigs", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
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

        [PluginEvent]
        void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            var Player = ev.Player;
            var Role = ev.Role;

            if (Player.ReferenceHub.isLocalPlayer && Server.Instance == null)
            {
                new Server(Player.ReferenceHub);
            }
            if (Config.EnableRoundSupplies)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (Player.Role is RoleTypeId.ClassD)
                    {
                        Player.ReferenceHub.inventory.ServerAddItem(Config.ClassDCard , 1);
                    }
                });
            }
            if (Config.EnableChangeSCPHPSystem && Player.IsSCP)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (SCPHPChangeSystem.healthDict.TryGetValue(Role, out var health))
                        Player.Health = SCPHPChangeSystem.healthDict[Role];
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

        [PluginEvent]
        bool OnPlayerReloadWeapon(PlayerReloadWeaponEvent ev)
        {
            var Player = ev.Player;
            var Firearm = ev.Firearm;

            if (Config.InfiniteAmmo)
            {
                if (Firearm.ItemTypeId is ItemType.ParticleDisruptor) return true;
                switch (Config.InfiniteAmmoType)
                {
                    case InfiniteAmmoType.Old:
                        Player.SetAmmo(Firearm.AmmoType, (ushort)Player.GetAmmoLimit(Firearm.AmmoType));
                        Timing.CallDelayed(4f, () =>
                            {
                                Player.SetAmmo(Firearm.AmmoType, (ushort)Player.GetAmmoLimit(Firearm.AmmoType));
                            });
                        break;
                    case InfiniteAmmoType.Normal:
                        if (Firearm.Status.Ammo != Firearm.AmmoManagerModule.MaxAmmo)
                        {
                            Player.ReloadWeapen();
                            Firearm.Status = new FirearmStatus(Firearm.AmmoManagerModule.MaxAmmo, Firearm.Status.Flags, Firearm.GetCurrentAttachmentsCode());
                            return false;
                        }
                        break;
                    case InfiniteAmmoType.Moment:
                        Firearm.Status = new FirearmStatus(Firearm.AmmoManagerModule.MaxAmmo, Firearm.Status.Flags, Firearm.GetCurrentAttachmentsCode());
                        return false;
                }
            }
            return true;
        }

        [PluginEvent]
        void OnPlayerShotWeapon(PlayerShotWeaponEvent ev)
        {
            var player = ev.Player;
            var firearm = ev.Firearm;
            if (player != null && Config.SavePlayersInfo)
            {
                var PLog = player.GetLog();
                PLog.PlayerShot++;
                PLog.UpdateLog();
            }
            if (firearm.ItemTypeId is ItemType.ParticleDisruptor) return;
            if (Config.InfiniteAmmoType is InfiniteAmmoType.Infinite && Config.InfiniteAmmo)
            {
                firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearm.Status.Flags, firearm.GetCurrentAttachmentsCode());
            }
        }

        [PluginEvent]
        void OnPlayerDying(PlayerDyingEvent ev)
        {
            var Player = ev.Player;
            var Attacker = ev.Attacker;
            var DamageHandler = ev.DamageHandler;

            if (Player == null) return;

            Timing.CallDelayed(1f, () =>
            {
                switch (Player.RoleName)
                {
                    case "SCP-703":
                        {
                            SCP703.OnPlayerDead(Player, "SCP 7 0 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-703{TranslateConfig.SpecialRoleContainCassie}");
                            SCP703 = null;
                            break;
                        }
                    case "SCP-029":
                        {
                            SCP029.OnPlayerDead(Player, "SCP 0 2 9 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-029{TranslateConfig.SpecialRoleContainCassie}");
                            SCP029 = null;
                            break;
                        }
                    case "SCP-191":
                        {
                            SCP191.OnPlayerDead(Player, "SCP 1 9 1 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-191{TranslateConfig.SpecialRoleContainCassie}");
                            SCP191 = null;
                            break;
                        }
                    case "SCP-073":
                        {
                            SCP073.OnPlayerDead(Player, "SCP 0 7 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-073{TranslateConfig.SpecialRoleContainCassie}");
                            SCP073 = null;
                            break;
                        }
                    case "SCP-347":
                        {
                            Player.EffectsManager.DisableAllEffects();
                            UnityEngine.Object.Destroy(Player.GameObject.GetComponent<PlayerLightBehavior>());
                            SCP347.OnPlayerDead(Player, "SCP 3 4 7 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-347{TranslateConfig.SpecialRoleContainCassie}");
                            SCP347 = null;
                            break;
                        }
                    case "SCP-2936-1":
                        {
                            Player.SetPlayerScale(1f);
                            SCP2936.OnPlayerDead(Player, "SCP 2 9 3 6 1 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-2936-1{TranslateConfig.SpecialRoleContainCassie}");
                            SCP2936 = null;
                            break;
                        }
                    case "SCP-1093":
                        {
                            UnityEngine.Object.Destroy(Player.GameObject.GetComponent<PlayerGlowBehavior>());
                            SCP1093.OnPlayerDead(Player, "SCP 1 0 9 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-1093{TranslateConfig.SpecialRoleContainCassie}");
                            SCP1093 = null;
                            break;
                        }
                }
                if (Player.RoleName == TranslateConfig.ChaosLeaderRoleName)
                {
                    ChaosLeader.OnPlayerDead(Player, "", TranslateConfig.ChaosLeaderDeathCassie);
                    foreach (Player Player in XHelper.PlayerList)
                    {
                        if (Player.Team is Team.ChaosInsurgency)
                        {
                            Player.EffectsManager.DisableEffect<MovementBoost>();
                        }
                    }
                    ChaosLeader = null;
                }
                if (SkynetPlayers.Contains(Player))
                {
                    SkynetPlayers.Remove(Player);
                    Player.CustomInfo = "";
                }
                if (SeePlayers.Contains(Player))
                {
                    SeePlayers.Remove(Player);
                    Player.CustomInfo = "";
                }
                if (!Player.DoNotTrack)
                {
                    var PLog = Player.GetLog();
                    PLog.PlayerDeath++;
                    PLog.UpdateLog();
                }
            });
        }

        [PluginEvent]
        void OnPlayerDyingInfo(PlayerDyingEvent ev)
        {
            var Player = ev.Player;
            var Attacker = ev.Attacker;
            var DamageHandler = ev.DamageHandler;

            if (Player != null && Attacker != null && Config.SavePlayersInfo && !Attacker.DoNotTrack)
            {
                var Log = Attacker.GetLog();
                if (Player.IsSCP)
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
            if (Player != null && Player.RoleName == "SCP-073")
            {
                Player.ClearInventory();
            }
        }

        [PluginEvent]
        public bool OnPlayerInteractDoor(PlayerInteractDoorEvent ev)
        {
            if (ev.Door.ActiveLocks > 0 && !ev.Player.IsBypassEnabled) return true;

            if (!Config.AffectDoors || ev.Player.IsSCP || ev.Player.IsWithoutItems() || ev.Player.CurrentItem is KeycardItem) return true;

            if (!ev.Door.AllowInteracting(ev.Player.ReferenceHub, 0)) return false;

            if (ev.Door.HasKeycardPermission(ev.Player))
            {
                ev.CanOpen = true;

                ev.Door.Toggle(ev.Player.ReferenceHub);
                return false;
            }
            return true;
        }

        [PluginEvent]
        public bool OnPlayerInteractLocker(PlayerInteractLockerEvent ev)
        {
            if (!Config.AffectLockers || ev.Player.IsSCP || ev.Player.IsWithoutItems() || ev.Player.CurrentItem is KeycardItem) return true;

            if (ev.Chamber.HasKeycardPermission(ev.Player))
            {
                ev.CanOpen = true;

                ev.Chamber.Toggle(ev.Locker);
                return false;
            }
            return true;
        }

        [PluginEvent]
        public bool OnPlayerInteractGenerator(PlayerInteractGeneratorEvent ev)
        {
            if (!Config.AffectGenerators || ev.Player.IsSCP || ev.Player.IsWithoutItems() ||
                ev.Player.CurrentItem is KeycardItem || ev.GeneratorColliderId != Scp079Generator.GeneratorColliderId.Door) return true;

            if (ev.Generator.HasKeycardPermission(ev.Player))
            {
                if (!ev.Generator.IsUnlocked())
                {
                    ev.Generator.Unlock();

                    ev.Generator.ServerGrantTicketsConditionally(new Footprint(ev.Player.ReferenceHub), 0.5f);

                    ev.Generator._cooldownStopwatch.Restart();

                    return false;
                }
            }

            return true;
        }

        [PluginEvent]
        public bool OnSearchPickup(PlayerSearchPickupEvent ev)
        {
            if (IsLobby)
            {
                return false;
            }

            return true;

        }

        [PluginEvent]
        public bool OnPlayerDroppedItem(PlayerDropItemEvent ev)
        {
            var Player = ev.Player;
            var Item = ev.Item;

            if (IsLobby)
            {
                return false;
            }
            if (Player.RoleName == "SCP-073" && Player.Team is Team.ChaosInsurgency)
            {
                return false;
            }

            return true;
        }

        [PluginEvent]
        public bool OnThrowItem(PlayerThrowItemEvent ev)
        {
            if (IsLobby)
            {
                return false;
            }

            return true;
        }

        [PluginEvent]
        public bool OnPlayerDroppedAmmo(PlayerDropAmmoEvent ev)
        {
            if (Config.InfiniteAmmo)
                return false;
            else
                return true;
        }

        [PluginEvent]
        void OnSkynetPlayerInteractGenerator(PlayerInteractGeneratorEvent ev)
        {
            var Player = ev.Player;
            var Generator = ev.Generator;

            if (SkynetPlayers.Contains(Player))
            {
                Generator.Network_syncTime = 50;
                Generator._totalActivationTime = 50;
                Generator._totalDeactivationTime = 50;
                Generator._prevTime = 50;
            }
        }

        [PluginEvent]
        void OnRemoteAdminCommandExecuted(RemoteAdminCommandExecutedEvent ev)
        {
            var sender = ev.Sender;
            var command = ev.Command;
            Player player = Player.Get(sender);
            if (player != null && !string.IsNullOrEmpty(command))
            {
                string note = TranslateConfig.AdminLog.Replace("%Nickname%" , player.Nickname).Replace("%Time%" , DateTime.Now.ToString()).Replace("%Command%" , command.ToString()).Replace("%UserId%" , player.UserId);
                if (Config.AdminLogShow)
                    XHelper.Allbroadcast(TranslateConfig.AdminLogBroadcast.Replace("%Nickname%" , player.Nickname).Replace("%Command%" , command.ToString()), 5, BroadcastFlags.Normal);
                Log.Info(note);
                try
                {
                    if (!File.Exists(Config.AdminLogPath))
                    {
                        FileStream fs1 = new FileStream(Config.AdminLogPath, FileMode.Create, FileAccess.Write);
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(note);
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs = new FileStream(Config.AdminLogPath, FileMode.Append, FileAccess.Write);
                        StreamWriter sr = new StreamWriter(fs);
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

        [PluginEvent]
        void OnPlayerChangeSpectator(PlayerChangeSpectatorEvent ev)
        {
            if (!Config.HideTutorials) return;

            if (HideTutorialsHelper.IsWhitelisted(ev.Player.ReferenceHub))
                return;

            if (ev.OldTarget?.Role is RoleTypeId.Tutorial)
                HideTutorialsHelper.ResyncSpectator(ev.Player, ev.OldTarget);

            if (ev.NewTarget?.Role is not RoleTypeId.Tutorial)
                return;

            Timing.RunCoroutine(HideTutorialsHelper.DesyncSpectator(ev.Player, ev.NewTarget));
        }

        [PluginEvent]
        void OnRoundRestart(RoundRestartEvent ev)
        {
            XHelper.PlayerList.Clear();
            XHelper.SpecialPlayerList.Clear();
        }

        [PluginEvent]
        void OnChangeRole(PlayerChangeRoleEvent ev)
        {
            var Player = ev.Player;
            var OldRole = ev.OldRole.RoleTypeId;
            var NewRole = ev.NewRole;
            if (Player == null) return;
            Timing.CallDelayed(3f, () =>
            {
                if (OldRole is RoleTypeId.Scp079 && NewRole is RoleTypeId.Spectator && Config.SCP191)
                {
                    SCP191 = new SCPHelper(Player,RoleTypeId.Tutorial , 120 , "SCP-191", "red");

                    Player.SetPlayerScale(0.8f);

                    Player.SendBroadcast(TranslateConfig.SCP191SpawnBroadcast, 6);
                    Player.GetHintProvider().ShowHint(TranslateConfig.SCP191SkillIntroduction, 6);

                    Player.AddItem(ItemType.ArmorCombat);
                    Player.AddItem(ItemType.GunFSP9);
                    Player.AddAmmo(ItemType.Ammo9x19, 60);
                    Player.AddItem(ItemType.Medkit);

                    Player Scp = XHelper.PlayerList.Where(x => x.IsSCP).ToList().RandomItem();
                    if (Scp != null)
                    {
                        Player.Position = Scp.Position + Vector3.up * 1;
                    }
                    else
                    {
                        Player.Position = XHelper.GetRandomSpawnLocation(RoleTypeId.NtfCaptain);
                    }

                    Timing.RunCoroutine(XHelper.SCP191Handle(Player).CancelWith(Player.GameObject));
                }
            });

            if (Config.SavePlayersInfo && !Player.DoNotTrack && NewRole is not RoleTypeId.Spectator)
            {
                var PLog = Player.GetLog();
                PLog.RolePlayed++;
                PLog.UpdateLog();
            }
        }

        [PluginUnload]
        public void OnDisabled()
        {
            Harmony.UnpatchAll(Harmony.Id);

            Instance = null;
            Harmony = null;
            db.Dispose();
            db = null;
        }

        [PluginConfig]
        public Config Config;
        [PluginConfig("TranslateConfig.yml")]
        public TranslateConfig TranslateConfig;
    }
}
