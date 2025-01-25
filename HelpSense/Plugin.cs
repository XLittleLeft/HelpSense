using CustomPlayerEffects;
using GameCore;
using HarmonyLib;
using HelpSense.API;
using HelpSense.API.Serialization;
using HelpSense.ConfigSystem;
using HelpSense.Handler;
using HelpSense.Helper;
using HelpSense.Helper.Chat;
using HelpSense.Helper.Event;
using HelpSense.Helper.Lobby;
using HelpSense.Helper.Misc;
using HelpSense.Helper.SCP;
using HelpSense.Helper.SpecialRole;
using HelpSense.MonoBehaviors;
using HelpSense.SSSS;
using HintServiceMeow.UI.Extension;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using LiteDB;
using MapGeneration;
using MapGeneration.Distributors;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.Voice;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using Respawning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UserSettings.ServerSpecific;
using static Broadcast;
using Log = PluginAPI.Core.Log;

namespace HelpSense
{
    public class Plugin
    {
        private Harmony _harmony = new("cn.xlittleleft.plugin");

        [PluginConfig]
        public Config Config;

        [PluginConfig("TranslateConfig.yml")]
        public TranslateConfig TranslateConfig;

        [PluginConfig("SSSSTranslateConfig.yml")]
        public SSSSTranslateConfig SSSSTranslateConfig;

        public System.Random Random = new(DateTime.Now.GetHashCode());

        public static string RespawnTimerDirectoryPath { get; private set; }

        public static LobbyLocationType CurLobbyLocationType;

        public CoroutineHandle LobbyTimer;

        public string Text;

        public SCPHelper SCP703;

        public SCPHelper SCP029;

        public SCPHelper SCP191;

        public SCPHelper SCP073;

        public SCPHelper SCP347;

        public SCPHelper SCP023;

        public SCPHelper ChaosLeader;
        public bool SpawnLeader = false;

        public SCPHelper SCP2936;
        public bool SpawnSCP2936 = false;

        public SCPHelper SCP1093;

        public HashSet<Player> SkynetPlayers = [];
        public bool SkynetSpawned = false;

        public HashSet<Player> SeePlayers = [];
        public bool SeeSpawned = false;
        public bool Scp096Enraging = false;

        public ushort SCP1068Id = 0;
        public ItemBase SCP1068Base;

        public ushort SCP1056Id = 0;
        public ItemBase SCP1056Base;

        public static System.Version PluginVersion => new(1, 3, 9);
        public static DateTime LastUpdateTime => new(2025, 1, 25, 17, 00, 00);
        public static System.Version RequiredGameVersion => new(14, 0, 2);

        [PluginEntryPoint("HelpSense", "1.3.9", "HelpSense综合服务器插件", "X小左")]
        private void LoadPlugin()
        {
            Instance = this;

            EventManager.RegisterEvents(this);

            _harmony.PatchAll();
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

        [PluginEvent]
        public void OnPlayerJoin(PlayerJoinedEvent ev)
        {
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
                                    player.AddItem(item , ItemAddReason.AdminCommand);
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
            Player Target = ev.Target;
            Player Attacker = ev.Player;
            DamageHandlerBase DamageHandler = ev.DamageHandler;

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

                if (Target.RoleName == "SCP-023" && Attacker.EffectsManager.TryGetEffect(out Scp1344 scp1344) && !Target.IsSameTeam(Attacker))
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
            return true;
        }

        [PluginEvent]
        void OnPlayerLeft(PlayerLeftEvent ev)
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
            API.API.PlayerDataDic.Remove(player.UserId);
        }

        [PluginEvent]
        void OnPlayerEscape(PlayerEscapeEvent ev)
        {
            Player player = ev.Player;
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
                        player.AddItem(ItemType.ParticleDisruptor, ItemAddReason.AdminCommand);
                        player.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.SCP703EscapeHint);
                    }
                });
            }
        }

        [PluginEvent]
        public void OnRoundStarted(RoundStartEvent ev)
        {
            WaveManager.OnWaveSpawned += EventHelper.OnTeamRespawn;
            Log.Debug("订阅OnWaveSpawned事件", Config.Debug);

            if (Config.SavePlayersInfo)
            {
                Timing.RunCoroutine(InfoExtension.CollectInfo());
                Log.Debug("开始记录玩家信息", Config.Debug);
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

                        Player.ShowBroadcast(TranslateConfig.SCP703SpawnBroadcast, 10, BroadcastFlags.Normal);
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

                        player.ShowBroadcast(TranslateConfig.SCP029SpawnBroadcast, 10, BroadcastFlags.Normal);

                        player.ClearInventory(false);

                        player.AddItem(ItemType.KeycardContainmentEngineer);
                        player.AddItem(ItemType.GunCOM18, ItemAddReason.AdminCommand);

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
                    Player player = XHelper.GetRandomSpecialPlayer(RoleTypeId.ClassD);

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
                        SCP023 = new SCPHelper(player, 200 , "SCP-023", "red");

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
                                        players.ClearInventory(false);

                                        players.AddItem(ItemType.KeycardMTFPrivate);

                                        players.AddItem(ItemType.GunCrossvec , ItemAddReason.AdminCommand);

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
                                    guardPlayer.ClearInventory(false);

                                    guardPlayer.GetPlayerUi().CommonHint.ShowOtherHint(TranslateConfig.GuardCaptainSpawnBroadcast, 5);

                                    guardPlayer.AddItem(ItemType.ArmorHeavy);
                                    guardPlayer.AddItem(ItemType.Medkit);
                                    guardPlayer.AddItem(ItemType.Adrenaline);
                                    guardPlayer.AddItem(ItemType.GrenadeHE);

                                    guardPlayer.AddItem(ItemType.GunLogicer, ItemAddReason.AdminCommand);

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
                    if (!string.IsNullOrEmpty(IntercomDisplay._singleton.Network_overrideText)) IntercomDisplay._singleton.Network_overrideText = "";

                    foreach (Player player in XHelper.PlayerList)
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
            Player target = ev.Target;
            Player player = ev.Player;
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
            Player target = ev.Target;
            Player player = ev.Player;
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
            Player player = ev.Player;

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
            Player Player = ev.Player;
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
            Player player = ev.Player;

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
            Player player = ev.Player;
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

            WaveManager.OnWaveSpawned -= EventHelper.OnTeamRespawn;
            Log.Debug("取消订阅OnWaveSpawned事件", Config.Debug);

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
            Player Player = ev.Player;
            RoleTypeId Role = ev.Role;

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
        //TODO:子弹Event
        /*[PluginEvent]
        bool OnPlayerReloadWeapon(PlayerReloadWeaponEvent ev)
        {
            Player player = ev.Player;
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
        }*/

        [PluginEvent]
        void OnPlayerShotWeapon(PlayerShotWeaponEvent ev)
        {
            Player player = ev.Player;
            var firearm = ev.Firearm;
            if (player != null && Config.SavePlayersInfo)
            {
                var pLog = player.GetLog();
                pLog.PlayerShot++;
                pLog.UpdateLog();
            }
        }

        [PluginEvent]
        void OnPlayerDying(PlayerDyingEvent ev)
        {
            Player player = ev.Player;

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
                    case "SCP-023":
                        {
                            GhostlyAbility.Unload(player);
                            SCP023.OnPlayerDead(player, "SCP 0 2 3 SUCCESSFULLY TERMINATED . TERMINATION CAUSE UNSPECIFIED", $"SCP-023{TranslateConfig.SpecialRoleContainCassie}");
                            SCP023 = null;
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
            Player player = ev.Player;
            Player attacker = ev.Attacker;

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
            Player Player = ev.Player;
            DoorVariant Door = ev.Door;

            if (Door.ActiveLocks > 0 && !Player.IsBypassEnabled)
                return true;

            if (!Config.AffectDoors || Player.IsSCP || Player.IsWithoutItems() || Player.CurrentItem is KeycardItem)
                return true;

            if (!Door.AllowInteracting(Player.ReferenceHub, 0))
                return false;

            if (Door.HasKeycardPermission(Player))
            {
                ev.CanOpen = true;

                Door.Toggle(Player.ReferenceHub);
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
                //ev.Generator.ServerGrantTicketsConditionally(new Footprint(ev.Player.ReferenceHub), 0.5f);
                //TODO:unsure
                /*if (ev.Player.Role.GetFaction() == Faction.FoundationStaff)
                {
                    RespawnTokensManager.OnPointsModified(Faction.FoundationStaff, 0.5f);
                }*/
                ev.Generator._cooldownStopwatch.Restart();

                return false;
            }

            return true;
        }

        [PluginEvent]
        public bool OnSearchPickup(PlayerSearchPickupEvent ev)
        {
            return Round.IsRoundStarted;
        }

        [PluginEvent]
        public bool OnPlayerDroppedItem(PlayerDropItemEvent ev)
        {
            if (!Round.IsRoundStarted)
                return false;

            if (ev.Player.RoleName == "SCP-073" && ev.Player.Team is Team.ChaosInsurgency)
                return false;

            return true;
        }

        [PluginEvent]
        public bool OnThrowItem(PlayerThrowItemEvent ev)
        {
            if (!Round.IsRoundStarted)
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
            Player player = ev.Player;
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
                string note = TranslateConfig.AdminLog.Replace("%Nickname%", player.Nickname).Replace("%Time%", DateTime.Now.ToString()).Replace("%Command%", command).Replace("%UserId%", player.UserId);
                if (Config.AdminLogShow)
                    XHelper.Broadcast(TranslateConfig.AdminLogBroadcast.Replace("%Nickname%", player.Nickname).Replace("%Command%", command), 5, BroadcastFlags.Normal);
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
            SCP023 = null;

            XHelper.PlayerList.Clear();
            XHelper.SpecialPlayerList.Clear();

            SpecialRoleHelper.Reset();
        }

        [PluginEvent]
        void OnChangeRole(PlayerChangeRoleEvent ev)
        {
            Player player = ev.Player;
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
                player.AddItem(ItemType.GunFSP9 , ItemAddReason.AdminCommand);
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
        }
    }
}
