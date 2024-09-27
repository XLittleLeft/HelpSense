using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using PluginAPI.Core;
using PluginAPI.Events;
using PluginAPI.Core.Attributes;

using PlayerRoles.Voice;
using PlayerRoles.PlayableScps.Scp079;
using PlayerRoles.PlayableScps.Scp096;

using HelpSense.API;
using HelpSense.MonoBehaviors;
using HelpSense.Helper;
using HelpSense.API.Features;
using HelpSense.Handler;
using HelpSense.ConfigSystem;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Firearms.Attachments;

using GameCore;
using PlayerStatsSystem;
using MapGeneration.Distributors;
using Respawning;
using MapGeneration;
using Footprinting;
using LiteDB;
using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using UnityEngine;
using HarmonyLib;

using static Broadcast;
using Log = PluginAPI.Core.Log;
using HintServiceMeow.UI.Extension;
using HelpSense.Helper.SpecialRole;

namespace HelpSense
{
    public class Plugin
    {
        private Harmony _harmony = new("cn.xlittleleft.plugin");

        public LiteDatabase Database;

        [PluginConfig]
        public Config Config;

        [PluginConfig("TranslateConfig.yml")]
        public TranslateConfig TranslateConfig;

        public System.Random Random = new(DateTime.Now.GetHashCode());

        public static string RespawnTimerDirectoryPath { get; private set; }

        public static LobbyLocationType CurLobbyLocationType;

        public CoroutineHandle LobbyTimer;

        public string Text;

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

        public HashSet<Player> SkynetPlayers = new();
        public bool SkynetSpawned = false;

        public HashSet<Player> SeePlayers = new();
        public bool SeeSpawned = false;
        public bool Scp096Enraging = false;

        public ushort SCP1068Id = 0;
        public ItemBase SCP1068Base;

        public ushort SCP1056Id = 0;
        public ItemBase SCP1056Base;
      
        public static System.Version PluginVersion => new(1, 3, 6);
        public static DateTime LastUpdateTime => new(2024, 09, 27, 14, 47, 20);
        public static System.Version RequiredGameVersion => new(13, 5, 1);
      
        [PluginEntryPoint("HelpSense", "1.3.6", "HelpSense综合服务器插件", "X小左")]
        private void LoadPlugin()
        {
            Instance = this;

            if (Config.SavePlayersInfo)
            {
                Database = new LiteDatabase(Config.SavePath);
            }

            EventManager.RegisterEvents(this);

            _harmony.PatchAll();

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
            var player = ev.Player;

            if (player == null || string.IsNullOrEmpty(player.UserId)) return;
            XHelper.PlayerList.Add(player);
            XHelper.SpecialPlayerList.Add(player);

            ChatHelper.InitForPlayer(player);

            if (Config.SavePlayersInfo)
            {
                var log = player.GetLog();
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
                    if (IsLobby && (RoundStart.singleton.NetworkTimer > 1 || RoundStart.singleton.NetworkTimer == -2))
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            player.SetRole(Config.LobbyPlayerRole);

                            player.IsGodModeEnabled = true;

                            if (Config.LobbyInventory.Count > 0 && CurLobbyLocationType != LobbyLocationType.Chaos)
                            {
                                foreach (var item in Config.LobbyInventory)
                                {
                                    player.AddItem(item);
                                }
                            }
                            if (CurLobbyLocationType is LobbyLocationType.Chaos)
                            {
                                player.AddItem(ItemType.ArmorHeavy);
                                player.AddItem(ItemType.GunAK);
                                player.AddItem(ItemType.GunCrossvec);
                                player.AddItem(ItemType.GunE11SR);
                                player.AddItem(ItemType.GunFRMG0);
                                player.AddItem(ItemType.GunLogicer);
                                player.AddItem(ItemType.GunRevolver);
                                player.AddItem(ItemType.GunShotgun);
                                player.SetAmmo(ItemType.Ammo9x19, (ushort)player.GetAmmoLimit(ItemType.Ammo9x19));
                                player.SetAmmo(ItemType.Ammo12gauge, (ushort)player.GetAmmoLimit(ItemType.Ammo12gauge));
                                player.SetAmmo(ItemType.Ammo762x39, (ushort)player.GetAmmoLimit(ItemType.Ammo762x39));
                                player.SetAmmo(ItemType.Ammo556x45, (ushort)player.GetAmmoLimit(ItemType.Ammo556x45));
                                player.SetAmmo(ItemType.Ammo44cal, (ushort)player.GetAmmoLimit(ItemType.Ammo44cal));
                            }
                        });

                        Timing.CallDelayed(0.6f, () =>
                        {
                            player.Position = LobbyLocationHandler.LobbyPosition;
                            player.Rotation = LobbyLocationHandler.LobbyRotation.eulerAngles;

                            player.EffectsManager.EnableEffect<MovementBoost>();
                            player.EffectsManager.ChangeState<MovementBoost>(Config.MovementBoostIntensity);
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

        [PluginEvent]
        bool OnDamage(PlayerDamageEvent ev)
        {
            var target = ev.Target;
            var attacker = ev.Player;
            var damageHandler = ev.DamageHandler;
            if (target != null && attacker != null)
            {
                if (target.RoleName == "SCP-1093" && damageHandler is AttackerDamageHandler attackerDamageHandler)
                {
                    if (attackerDamageHandler.Hitbox is HitboxType.Headshot)
                    {
                        attackerDamageHandler.Damage = 0;
                    }
                }
                if (target.RoleName == "SCP-191" && (attacker.IsHuman && attacker.CurrentItem.ItemTypeId.IsWeapon()) && damageHandler is StandardDamageHandler standard)
                {
                    standard.Damage = Config.SCP191Ammo;
                }
                if (target.RoleName == "SCP-073" && attacker.Team != target.Team && attacker.IsHuman && target.Team is not Team.ChaosInsurgency)
                {
                    attacker.Damage(Config.SCP073RRD, TranslateConfig.SCP073DamageReason);
                }
                else if (attacker.Team != target.Team && attacker.IsSCP && target.RoleName == "SCP-073" && target.Team is not Team.ChaosInsurgency)
                {
                    attacker.Damage(Config.SCP073SCPRD, TranslateConfig.SCP073DamageReason);
                }
                if ((attacker.RoleName == "SCP-191" && target.Team is Team.SCPs) || (target.RoleName == "SCP-191" && attacker.Team is Team.SCPs))
                {
                    return false;
                }
                if (damageHandler is StandardDamageHandler standardDamage && attacker.Team != target.Team)
                {
                    if (Config.SavePlayersInfo)
                    {
                        var pLog = attacker.GetLog();
                        pLog.PlayerDamage += standardDamage.Damage;
                        pLog.UpdateLog();
                    }
                    if (attacker.Role is RoleTypeId.Scp096 && SeePlayers.Contains(target))
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
            var player = ev.Player;
            
            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            if (Config.SavePlayersInfo)
            {
                var log = player.GetLog();
                log.LastLeftTime = DateTime.Now;
                log.UpdateLog();
            }

            XHelper.PlayerList.Remove(player);
            XHelper.SpecialPlayerList.Remove(player);
        }

        [PluginEvent]
        void OnPlayerEscape(PlayerEscapeEvent ev)
        {
            var player = ev.Player;
            var newRole = ev.NewRole;

            if (player.RoleName == "SCP-029")
            {
                Timing.CallDelayed(1f, () =>
                {
                    player.EffectsManager.ChangeState<MovementBoost>(25);
                    player.EffectsManager.ChangeState<Scp1853>(2);
                    player.EffectsManager.ChangeState<DamageReduction>(15);
                    if (newRole == RoleTypeId.ChaosConscript)
                    {
                        player.AddItem(ItemType.SCP268);
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP029EscapeHint);
                    }
                });
            }
            if (player.RoleName == "SCP-703")
            {
                Timing.CallDelayed(1f, () =>
                {
                    if (newRole == RoleTypeId.NtfSpecialist)
                    {
                        var firearm = player.ReferenceHub.inventory.ServerAddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                        firearm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP703EscapeHint);
                    }
                });
            }
        }

        [PluginEvent]
        void OnSpecialTeamRespawn(TeamRespawnEvent ev)
        {
            var team = ev.Team;
            var players = ev.Players;

            Timing.CallDelayed(0.5f, () =>
            {
                if (Config.EnableSkynet && !SkynetSpawned && team == SpawnableTeamType.NineTailedFox)
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
                        XHelper.MessageTranslated($"MTFUnit Kappa , 10 , and , Mu , 7 , designated scan neck , HasEntered , they will help contain scp 0 7 9 , AllRemaining , AwaitingRecontainment {XHelper.PlayerList.Where(x => x.IsSCP).Count()} SCPSubjects", TranslateConfig.SkynetCassie.Replace("%SCPNum%" , XHelper.PlayerList.Where(x => x.IsSCP).Count().ToString()));

                        foreach (Player player in players)
                        {
                            SkynetPlayers.Add(player);
                            if (player.Role is not RoleTypeId.NtfCaptain)
                                player.AddItem(ItemType.SCP2176);
                            switch (player.Role)
                            {
                                case RoleTypeId.NtfPrivate:
                                    player.ShowBroadcast($"{TranslateConfig.SkynetPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    player.CustomInfo = TranslateConfig.SkynetPrivateCustomInfo;
                                    break;
                                case RoleTypeId.NtfSergeant:
                                    player.ShowBroadcast($"{TranslateConfig.SkynetSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                    player.CustomInfo = TranslateConfig.SkynetSergeantCustomInfo;
                                    break;
                                case RoleTypeId.NtfCaptain:
                                    player.ShowBroadcast($"{TranslateConfig.SkynetCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
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
                                            player.ShowBroadcast($"{TranslateConfig.SeeNoEvilPrivateBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            player.CustomInfo = TranslateConfig.SeeNoEvilPrivateCustomInfo;
                                            break;
                                        case RoleTypeId.NtfSergeant:
                                            player.ShowBroadcast($"{TranslateConfig.SeeNoEvilSergeantBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
                                            player.CustomInfo = TranslateConfig.SeeNoEvilSergeantCustomInfo;
                                            break;
                                        case RoleTypeId.NtfCaptain:
                                            player.ShowBroadcast($"{TranslateConfig.SeeNoEvilCaptainBroadcast}", 5, Broadcast.BroadcastFlags.Normal);
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

        [PluginEvent]
        void OnTeamRespawn(TeamRespawnEvent ev)
        {
            var team = ev.Team;
            var players = ev.Players;

            var specialPlayers = ev.Players;

            if (Config.SpawnHID)
            {
                Timing.CallDelayed(2f, () =>
                {
                    foreach (Player Player in players)
                    {
                        if (Player.Role is RoleTypeId.NtfCaptain)
                        {
                            var firaerm = Player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                            firaerm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firaerm.GetCurrentAttachmentsCode());
                        }
                    }
                });
            }
            if (Config.EnableChaosLeader && !SpawnLeader)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var player = XHelper.GetRandomPlayer(RoleTypeId.ChaosRifleman, specialPlayers);
                    if (player != null)
                    {
                        specialPlayers.Remove(player);

                        SpawnLeader = true;

                        ChaosLeader = new SCPHelper(player, 150, TranslateConfig.ChaosLeaderRoleName, "green");

                        player.ClearBroadcasts();

                        player.ShowBroadcast(TranslateConfig.ChaosLeaderSpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        var firearm = player.AddItem(ItemType.ParticleDisruptor) as ParticleDisruptor;
                        firearm.Status = new FirearmStatus(5, FirearmStatusFlags.MagazineInserted, firearm.GetCurrentAttachmentsCode());

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
            if (Config.SCP073 && XHelper.PlayerList.Any(x => x.RoleName == "SCP-073"))
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

                            player.ClearInventory();
                            for (int i = 0; i < 8; i++)
                            {
                                player.AddItem(ItemType.Jailbird);
                            }

                            Timing.RunCoroutine(XHelper.PositionCheckerCoroutine(player).CancelWith(player.GameObject));
                        }
                        else
                        {
                            player.SendBroadcast(TranslateConfig.SCP073CainSpawnBroadcast, 6);
                        }
                        player.EffectsManager.EnableEffect<DamageReduction>();
                        player.EffectsManager.ChangeState<DamageReduction>(Config.SCP073DD);
                        player.EffectsManager.EnableEffect<MovementBoost>();
                        player.EffectsManager.ChangeState<MovementBoost>(10);
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
                Log.Debug("开始记录玩家信息");
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

                        Player.ShowBroadcast(TranslateConfig.SCP703SpawnBroadcast, 10, BroadcastFlags.Normal);
                    };
                });
            }

            if (Config.EnableSCP029)
            {
                Timing.CallDelayed(1f, () =>
                {
                    var player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);
                    if (player != null)
                    {
                        SCP029 = new SCPHelper(player, 120, "SCP-029", "red");

                        player.ClearBroadcasts();

                        player.ShowBroadcast(TranslateConfig.SCP029SpawnBroadcast, 10, Broadcast.BroadcastFlags.Normal);

                        player.ClearInventory();

                        player.AddItem(ItemType.KeycardContainmentEngineer);
                        var firearm = player.ReferenceHub.inventory.ServerAddItem(ItemType.GunCOM18);
                        ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                        player.AddAmmo(ItemType.Ammo9x19, 30);

                        player.EffectsManager.EnableEffect<MovementBoost>();
                        player.EffectsManager.ChangeState<MovementBoost>(20);
                        player.EffectsManager.EnableEffect<Scp1853>();
                        player.EffectsManager.ChangeState<Scp1853>(2);
                        player.EffectsManager.EnableEffect<DamageReduction>();
                        player.EffectsManager.ChangeState<DamageReduction>(15);

                        player.Health = 120;
                    };
                });
            }

            if (Config.SCP347)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    var player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (player != null)
                    {
                        SCP347 = new SCPHelper(player, RoleTypeId.Tutorial, "SCP-347", "red", XHelper.GetRandomSpawnLocation(RoleTypeId.FacilityGuard));

                        player.AddItem(ItemType.KeycardGuard);

                        player.SendBroadcast(TranslateConfig.SCP347SpawnBroadcast, 6);

                        player.EffectsManager.EnableEffect<Invisible>();

                        player.GameObject.AddComponent<PlayerLightBehavior>();
                    }
                });
            }

            if (Config.SCP1093)
            {
                Timing.CallDelayed(1.4f, () =>
                {
                    var player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

                    if (player != null)
                    {
                        SCP1093 = new SCPHelper(player , "SCP-1093" , "yellow");

                        player.GameObject.AddComponent<PlayerGlowBehavior>();

                        player.SendBroadcast(TranslateConfig.SCP1093SpawnBroadcast, 6, BroadcastFlags.Normal);
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1093SkillIntroduction.ToArray(), 6);
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
                            XHelper.Broadcast(TranslateConfig.GuardMutinyBroadcast, 10, BroadcastFlags.Normal);
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
                                XHelper.Broadcast(TranslateConfig.EliteGuardBroadcast, 10, BroadcastFlags.Normal);
                                break;
                            }
                        case 2:
                            {
                                Player guardPlayer = XHelper.GetRandomPlayer(RoleTypeId.FacilityGuard);
                                if (guardPlayer != null)
                                {
                                    guardPlayer.ClearInventory();

                                    guardPlayer.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.GuardCaptainSpawnBroadcast, 5);

                                    guardPlayer.AddItem(ItemType.ArmorHeavy);
                                    guardPlayer.AddItem(ItemType.Medkit);
                                    guardPlayer.AddItem(ItemType.Adrenaline);
                                    guardPlayer.AddItem(ItemType.GrenadeHE);

                                    var firearm = guardPlayer.AddItem(ItemType.GunLogicer);
                                    ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());

                                    guardPlayer.SetAmmo(ItemType.Ammo762x39, (ushort)guardPlayer.GetAmmoLimit(ItemType.Ammo762x39));

                                    guardPlayer.AddItem(ItemType.KeycardMTFOperative);
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
                    Log.Error("[HelpSense] [Event: OnRoundStarted] " + e);
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

        [PluginEvent]
        bool OnScp173(Scp173NewObserverEvent ev)
        {
            var target = ev.Target;
            var player = ev.Player;
            if (target != null && player != null)
            {
                if (target.RoleName == "SCP-191")
                {
                    return false;
                }
            }
            return true;
        }

        [PluginEvent]
        bool OnScp096(Scp096AddingTargetEvent ev)
        {
            var target = ev.Target;
            var player = ev.Player;
            if (target != null && player != null)
            {
                var role = player.ReferenceHub.roleManager.CurrentRole as Scp096Role;
                if (target.RoleName == "SCP-191")
                {
                    return false;
                }
                if (SeePlayers.Contains(target) && !Scp096Enraging && role.IsAbilityState(Scp096AbilityState.None) && (role.IsRageState(Scp096RageState.Docile) || role.IsRageState(Scp096RageState.Distressed)))
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
            var item = ev.Item;
            var player = ev.Player;

            Timing.CallDelayed(0.5f, () =>
            {
                if (item.Info.Serial == SCP1068Id && item.Info.ItemId is ItemType.SCP2176 && Config.SCP1068)
                {
                    player.RemoveItem(item);
                    var items = player.AddItem(ItemType.SCP2176);
                    SCP1068Base = items;
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1068PickupHint);
                }
                if (item.Info.Serial == SCP1056Id && item.Info.ItemId is ItemType.Medkit && Config.SCP1056)
                {
                    player.RemoveItem(item);
                    var items = player.AddItem(ItemType.Medkit);
                    SCP1056Base = items;
                    player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1056PickupHint);
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
            var player = ev.Player;

            if (player == null)
                return;

            if (player.RoleName == "SCP-347")
            {
                Timing.CallDelayed(3f, () =>
                {
                    foreach (var item in player.ReferenceHub.inventory.UserInventory.Items)
                    {
                        if (item.Value.ItemTypeId.IsWeapon())
                        {
                            player.DropItem(item.Value);
                            player.EffectsManager.EnableEffect<Flashed>(5);
                        }
                    }
                });
            }
        }

        [PluginEvent]
        void OnPlayerUsedItem(PlayerUsedItemEvent ev)
        {
            var player = ev.Player;
            var item = ev.Item;

            if (SCP1056Base != null && item == SCP1056Base)
            {
                player.SetPlayerScale(Config.SCP1056X);
                player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP1056UsedHint);
            }
        }

        [PluginEvent]
        void OnPlayerThrowProjectile(PlayerThrowProjectileEvent ev)
        {
            var item = ev.Item;

            if (SCP1068Base != null && item == SCP1068Base)
            {
                XHelper.Broadcast(TranslateConfig.SCP1068UsedBroadcast, 5, BroadcastFlags.Normal);
                Server.Instance.GetComponent<AlphaWarheadController>(globalSearch: true).RpcShake(true);
            }//沙比NW写空壳核弹抖动我直接自己写一个
        }

        [PluginEvent]
        void OnRoundEnd(RoundEndEvent ev)
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
                    Server.Init();

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

        [PluginEvent]
        void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            var player = ev.Player;
            var role = ev.Role;

            if (player.ReferenceHub.isLocalPlayer && Server.Instance == null)
            {
                _ = new Server(player.ReferenceHub);
            }

            if (Config.EnableRoundSupplies)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (player.Role is RoleTypeId.ClassD)
                    {
                        player.ReferenceHub.inventory.ServerAddItem(Config.ClassDCard , 1);
                    }
                });
            }

            if (Config.EnableChangeSCPHPSystem && player.IsSCP)
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    if (SCPHPChangeSystem.healthDict.TryGetValue(role, out var health))
                        player.Health = SCPHPChangeSystem.healthDict[role];
                });
            }

            if (Config.EnableSpectatorList)
            {
                Timing.CallDelayed(1.2f, () =>
                {
                    Timing.RunCoroutine(SpectatorHelper.SpectatorList(player).CancelWith(player.GameObject));
                });
            }
        }

        [PluginEvent]
        bool OnPlayerReloadWeapon(PlayerReloadWeaponEvent ev)
        {
            var player = ev.Player;
            var firearm = ev.Firearm;

            if (Config.InfiniteAmmo)
            {
                if (firearm.ItemTypeId is ItemType.ParticleDisruptor) return true;
                switch (Config.InfiniteAmmoType)
                {
                    case InfiniteAmmoType.Old:
                        player.SetAmmo(firearm.AmmoType, (ushort)player.GetAmmoLimit(firearm.AmmoType));
                        Timing.CallDelayed(4f, () =>
                            {
                                player.SetAmmo(firearm.AmmoType, (ushort)player.GetAmmoLimit(firearm.AmmoType));
                            });
                        break;
                    case InfiniteAmmoType.Normal:
                        if (firearm.Status.Ammo != firearm.AmmoManagerModule.MaxAmmo)
                        {
                            player.ReloadWeapon();
                            firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearm.Status.Flags, firearm.GetCurrentAttachmentsCode());
                            return false;
                        }
                        break;
                    case InfiniteAmmoType.Moment:
                        firearm.Status = new FirearmStatus(firearm.AmmoManagerModule.MaxAmmo, firearm.Status.Flags, firearm.GetCurrentAttachmentsCode());
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
                var pLog = player.GetLog();
                pLog.PlayerShot++;
                pLog.UpdateLog();
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
            var player = ev.Player;

            if (player == null) return;

            Timing.CallDelayed(1f, () =>
            {
                switch (player.RoleName)
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
                    case "SCP-347":
                        {
                            player.EffectsManager.DisableAllEffects();
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

                if (player.RoleName == TranslateConfig.ChaosLeaderRoleName)
                {
                    ChaosLeader.OnPlayerDead(player, "", TranslateConfig.ChaosLeaderDeathCassie);
                    foreach (Player Player in XHelper.PlayerList)
                    {
                        if (Player.Team is Team.ChaosInsurgency)
                        {
                            Player.EffectsManager.DisableEffect<MovementBoost>();
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
        }

        [PluginEvent]
        void OnPlayerDyingInfo(PlayerDyingEvent ev)
        {
            var player = ev.Player;
            var attacker = ev.Attacker;

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

            if (player != null && player.RoleName == "SCP-073")
            {
                player.ClearInventory();
            }
        }

        [PluginEvent]
        public bool OnPlayerInteractDoor(PlayerInteractDoorEvent ev)
        {
            if (ev.Door.ActiveLocks > 0 && !ev.Player.IsBypassEnabled)
                return true;

            if (!Config.AffectDoors || ev.Player.IsSCP || ev.Player.IsWithoutItems() || ev.Player.CurrentItem is KeycardItem)
                return true;

            if (!ev.Door.AllowInteracting(ev.Player.ReferenceHub, 0))
                return false;

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
            if (!Config.AffectLockers || ev.Player.IsSCP || ev.Player.IsWithoutItems() || ev.Player.CurrentItem is KeycardItem)
                return true;

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

            if (!ev.Generator.HasKeycardPermission(ev.Player) && !ev.Generator.IsUnlocked())
            {
                ev.Generator.Unlock();
                ev.Generator.ServerGrantTicketsConditionally(new Footprint(ev.Player.ReferenceHub), 0.5f);
                ev.Generator._cooldownStopwatch.Restart();

                return false;
            }

            return true;
        }

        [PluginEvent]
        public bool OnSearchPickup(PlayerSearchPickupEvent ev)
        {
            return !IsLobby;
        }

        [PluginEvent]
        public bool OnPlayerDroppedItem(PlayerDropItemEvent ev)
        {
            if (IsLobby)
                return false;

            if (ev.Player.RoleName == "SCP-073" && ev.Player.Team is Team.ChaosInsurgency)
                return false;

            return true;
        }

        [PluginEvent]
        public bool OnThrowItem(PlayerThrowItemEvent ev)
        {
            if (IsLobby)
                return false;

            return true;
        }

        [PluginEvent]
        public bool OnPlayerDroppedAmmo(PlayerDropAmmoEvent ev)
        {
            if (Config.InfiniteAmmo)
                return false;

            return true;
        }

        [PluginEvent]
        void OnSkynetPlayerInteractGenerator(PlayerInteractGeneratorEvent ev)
        {
            var player = ev.Player;
            var generator = ev.Generator;

            if (SkynetPlayers.Contains(player))
            {
                generator.Network_syncTime = 50;
                generator._totalActivationTime = 50;
                generator._totalDeactivationTime = 50;
                generator._prevTime = 50;
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
                string note = TranslateConfig.AdminLog.Replace("%Nickname%" , player.Nickname).Replace("%Time%" , DateTime.Now.ToString()).Replace("%Command%" , command).Replace("%UserId%" , player.UserId);
                if (Config.AdminLogShow)
                    XHelper.Broadcast(TranslateConfig.AdminLogBroadcast.Replace("%Nickname%" , player.Nickname).Replace("%Command%" , command), 5, BroadcastFlags.Normal);
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
        void OnRoundRestart(RoundRestartEvent _)
        {
            SCP029 = null;
            SCP073 = null;
            SCP1093 = null;
            SCP191 = null;
            SCP2936 = null;
            SCP347 = null;
            SCP703 = null;

            XHelper.PlayerList.Clear();
            XHelper.SpecialPlayerList.Clear();
        }

        [PluginEvent]
        void OnChangeRole(PlayerChangeRoleEvent ev)
        {
            var player = ev.Player;
            var oldRole = ev.OldRole.RoleTypeId;
            var newRole = ev.NewRole;
            if (player == null || string.IsNullOrEmpty(player.UserId)) return;
            Timing.CallDelayed(3f, () =>
            {
                if (!(oldRole is RoleTypeId.Scp079 && newRole is RoleTypeId.Spectator && Config.SCP191))
                    return;

                SCP191 = new SCPHelper(player, RoleTypeId.Tutorial, 120, "SCP-191", "red");

                player.SetPlayerScale(0.8f);

                player.SendBroadcast(TranslateConfig.SCP191SpawnBroadcast, 6);

                player.AddItem(ItemType.ArmorCombat);
                player.AddItem(ItemType.GunFSP9);
                player.AddAmmo(ItemType.Ammo9x19, 60);
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

                Timing.RunCoroutine(XHelper.SCP191CoroutineMethod(player).CancelWith(player.GameObject));
            });

            if (Config.SavePlayersInfo && !player.DoNotTrack && newRole is not RoleTypeId.Spectator)
            {
                var pLog = player.GetLog();
                pLog.RolePlayed++;
                pLog.UpdateLog();
            }
        }

        [PluginUnload]
        public void OnDisabled()
        {
            _harmony.UnpatchAll(_harmony.Id);

            Instance = null;
            _harmony = null;
            Database.Dispose();
            Database = null;
        }
    }
}
