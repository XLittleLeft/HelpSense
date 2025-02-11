using HelpSense.API.Events;
using HelpSense.API.Features.Pool;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Configs;
using InventorySystem.Items.Pickups;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Log = LabApi.Features.Console.Logger;

namespace HelpSense.Helper
{
    public static class XHelper
    {
        public static System.Random Random = new(DateTime.Now.GetHashCode());
        public static List<Player> PlayerList = [];
        public static List<Player> SpecialPlayerList = [];

        public static Player GetRandomPlayer(RoleTypeId roleTypeId)
        {
            List<Player> players = new List<Player>();

            foreach (Player player in PlayerList)
            {
                if (player.Role == roleTypeId)
                {
                    players.Add(player);
                }
            }

            if (players.Any())
            {
                return players[Random.Next(0, players.Count() - 1)];
            }

            return null;
        }
        public static Player GetRandomPlayer(RoleTypeId roleTypeId, List<Player> playerList)
        {
            List<Player> players = [];
            foreach (Player player in playerList)
            {
                if (player.Role == roleTypeId)
                {
                    players.Add(player);
                }
            }
            if (players.Any())
            {
                return players[Random.Next(0, players.Count - 1)];
            }

            return null;
        }
        public static Player GetRandomPlayer(List<Player> playerList)
        {
            if (playerList.Any())
            {
                return playerList[Random.Next(0, playerList.Count() - 1)];
            }

            return null;
        }
        public static Player GetRandomSpecialPlayer(RoleTypeId roleTypeId)
        {
            List<Player> players = [];
            foreach (Player player in SpecialPlayerList)
            {
                if (player.Role == roleTypeId)
                {
                    players.Add(player);
                }
            }
            if (players.Any())
            {
                var randomPlayer = players[Random.Next(0, players.Count() - 1)];
                SpecialPlayerList.Remove(randomPlayer);
                return randomPlayer;
            }

            return null;
        }
        public static ItemType GetRandomItem()
        {
            var allItems = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>();

            return allItems[Random.Next(0, allItems.Length - 1)];
        }

        public static void SpawnItem(ItemType typeid, Vector3 position, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Pickup.Create(typeid, position, new Quaternion(0, 0, 0, 0)).Spawn();
            }
        }
        public static Pickup SpawnItem(ItemType typeid, Vector3 position)
        {
            var item = Pickup.Create(typeid, position, new Quaternion(0, 0, 0, 0));
            item.Spawn();
            return item;
        }

        public static void SetPlayerScale(this Player target, Vector3 scale)
        {
            GameObject go = target.GameObject;
            if (go.transform.localScale == scale)
                return;

            try
            {
                NetworkIdentity identity = target.ReferenceHub.networkIdentity;
                go.transform.localScale = scale;
                foreach (Player player in PlayerList)
                {
                    NetworkServer.SendSpawnMessage(identity, player.Connection);
                }
            }
            catch (Exception e)
            {
                Log.Info($"Set Scale error: {e}");
            }
        }
        public static void SetPlayerScale(this Player target, float scale) => SetPlayerScale(target, Vector3.one * scale);
        public static bool PlayerScaleIs(this Player target, Vector3 scale) => target.GameObject.transform.localScale == scale;
        public static bool PlayerScaleIs(this Player target, float scale) => PlayerScaleIs(target, Vector3.one * scale);

        public static void MessageTranslated(string message, string translation, bool isHeld = false, bool isNoisy = true, bool isSubtitles = true)
        {
            StringBuilder announcement = StringBuilderPool.Pool.Get();
            string[] cassies = message.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Length; i++)
                announcement.Append($"{translations[i].Replace(' ', ' ')}<size=0> {cassies[i]} </size><split>");

            RespawnEffectsController.PlayCassieAnnouncement(announcement.ToString(), isHeld, isNoisy, isSubtitles);
            StringBuilderPool.Pool.Return(announcement);
        }

        //防倒卖
        public static IEnumerator<float> AutoXBroadcast()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(6 * 60f);
                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }
                Server.SendBroadcast("<size=35><align=center><color=#F6511D>此服务器在运行X小左的插件，享受你的游戏时间~</color></align></size>", 6, global::Broadcast.BroadcastFlags.Normal);
            }
        }
        public static IEnumerator<float> AutoServerBroadcast()
        {
            while (true)
            {
                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }

                Server.SendBroadcast(CustomEventHandler.TranslateConfig.AutoServerMessageText, CustomEventHandler.Config.AutoServerMessageTimer, global::Broadcast.BroadcastFlags.Normal);
                yield return Timing.WaitForSeconds(CustomEventHandler.Config.AutoServerMessageTime * 60f);
            }
        }

        public static bool IsSpecialPlayer(this Player player)
        {
            return player.ReferenceHub.serverRoles.Network_myText is "SCP-029" or "SCP-703" or "SCP-191" or "SCP-073" or "SCP-2936-1" || player.ReferenceHub.serverRoles.Network_myText == CustomEventHandler.TranslateConfig.ChaosLeaderRoleName;
        }

        public static bool BreakDoor(DoorVariant doorBase, DoorDamageType type = DoorDamageType.ServerCommand)
        {
            if (doorBase is not IDamageableDoor damageableDoor || damageableDoor.IsDestroyed)
                return false;

            damageableDoor.ServerDamage(ushort.MaxValue, type);
            return true;
        }
        //TODO:ReloadWeapon
        /*public static void ReloadWeapon(this Player player)
        {
            if (player.CurrentItem == null)
                return;

            if (player.CurrentItem is not Firearm firearm)
                return;

            firearm.AmmoManagerModule.ServerTryReload();
            player.Connection.Send(new RequestMessage(firearm.ItemSerial, RequestType.Reload));
        }*/

        public static bool IsAmmo(this ItemType item)
        {
            if (item != ItemType.Ammo9x19 && item != ItemType.Ammo12gauge && item != ItemType.Ammo44cal && item != ItemType.Ammo556x45)
            {
                return item == ItemType.Ammo762x39;
            }

            return true;
        }

        public static bool IsWeapon(this ItemType type, bool checkMicro = true)
        {
            switch (type)
            {
                case ItemType.GunCOM15:
                case ItemType.GunE11SR:
                case ItemType.GunCrossvec:
                case ItemType.GunFSP9:
                case ItemType.GunLogicer:
                case ItemType.GunCOM18:
                case ItemType.GunRevolver:
                case ItemType.GunAK:
                case ItemType.GunShotgun:
                case ItemType.ParticleDisruptor:
                case ItemType.GunCom45:
                case ItemType.GunFRMG0:
                case ItemType.Jailbird:
                    return true;
                case ItemType.MicroHID:
                    if (checkMicro)
                    {
                        return true;
                    }

                    break;
            }

            return false;
        }

        public static bool IsScp(this ItemType type)
        {
            return type is ItemType.SCP018 or ItemType.SCP500 or ItemType.SCP268 or ItemType.SCP207 or ItemType.SCP244a or ItemType.SCP244b or ItemType.SCP2176;
        }

        public static bool IsThrowable(this ItemType type)
        {
            return type is ItemType.SCP018 or ItemType.GrenadeHE or ItemType.GrenadeFlash or ItemType.SCP2176;
        }

        public static bool IsMedical(this ItemType type)
        {
            return type is ItemType.Painkillers or ItemType.Medkit or ItemType.SCP500 or ItemType.Adrenaline;
        }

        public static bool IsUtility(this ItemType type)
        {
            return type is ItemType.Flashlight or ItemType.Radio;
        }

        public static bool IsArmor(this ItemType type)
        {
            return type is ItemType.ArmorLight or ItemType.ArmorCombat or ItemType.ArmorHeavy;
        }

        public static bool IsKeycard(this ItemType type)
        {
            var keycardTypes = new HashSet<ItemType>
            {
                ItemType.KeycardScientist,
                ItemType.KeycardResearchCoordinator,
                ItemType.KeycardZoneManager,
                ItemType.KeycardGuard,
                ItemType.KeycardMTFPrivate,
                ItemType.KeycardContainmentEngineer,
                ItemType.KeycardMTFOperative,
                ItemType.KeycardMTFCaptain,
                ItemType.KeycardFacilityManager,
                ItemType.KeycardChaosInsurgency,
                ItemType.KeycardO5
            };

            return keycardTypes.Contains(type);
        }

        public static Team GetTeam2(this RoleTypeId typeId)
        {
            return typeId switch
            {
                RoleTypeId.ChaosConscript or RoleTypeId.ChaosRifleman or RoleTypeId.ChaosRepressor or RoleTypeId.ChaosMarauder => Team.ChaosInsurgency,
                RoleTypeId.Scientist => Team.Scientists,
                RoleTypeId.ClassD => Team.ClassD,
                RoleTypeId.Scp173 or RoleTypeId.Scp106 or RoleTypeId.Scp049 or RoleTypeId.Scp079 or RoleTypeId.Scp096 or RoleTypeId.Scp0492 or RoleTypeId.Scp939 or RoleTypeId.Scp3114 => Team.SCPs,
                RoleTypeId.NtfSpecialist or RoleTypeId.NtfSergeant or RoleTypeId.NtfCaptain or RoleTypeId.NtfPrivate or RoleTypeId.FacilityGuard => Team.FoundationForces,
                RoleTypeId.Tutorial => Team.OtherAlive,
                _ => Team.Dead,
            };
        }

        public static Vector3 GetRandomSpawnLocation(this RoleTypeId roleType)
        {
            if (!PlayerRoleLoader.TryGetRoleTemplate(roleType, out PlayerRoleBase @base))
                return Vector3.zero;

            if (@base is not IFpcRole fpc)
                return Vector3.zero;

            ISpawnpointHandler spawn = fpc.SpawnpointHandler;
            if (spawn is null)
                return Vector3.zero;

            if (!spawn.TryGetSpawnpoint(out Vector3 pos, out float _))
                return Vector3.zero;

            return pos;
        }

        public static void ChangeAppearance(this Player player, RoleTypeId type)
        {
            foreach (var pl in PlayerList.Where(x => x.PlayerId != player.PlayerId && x.IsReady))
            {
                pl.Connection.Send(new RoleSyncInfo(player.ReferenceHub, type, pl.ReferenceHub));
            }
        }

        public static bool TryGetRoleBase(this RoleTypeId roleType, out PlayerRoleBase roleBase)
        {
            return PlayerRoleLoader.TryGetRoleTemplate(roleType, out roleBase);
        }

        public static IEnumerator<float> PositionCheckerCoroutine(Player player)
        {
            Vector3 position = player.Position;
            float health = player.Health;
            int timeChecker = 0;

            while (true)
            {
                if (player is null || !player.IsAlive || Round.IsRoundEnded || player.Team is not Team.ChaosInsurgency)
                {
                    yield break;
                }

                if (position != player.Position || health.Equals(player.Health))
                {
                    timeChecker = 0;
                    position = player.Position;
                    health = player.Health;
                }
                else
                {
                    timeChecker++;

                    if (timeChecker >= 2)
                    {
                        timeChecker = 0;
                        player.Heal(5f);
                    }
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static string GetRoleName(this Player player)
        {
            return player.ReferenceHub.serverRoles.Network_myText;
        }
        public static void SetRoleName(this Player player , string i)
        {
            player.ReferenceHub.serverRoles.SetText(i);
        }
        public static string GetRoleColor(this Player player)
        {
            return player.ReferenceHub.serverRoles.Network_myColor;
        }
        public static void SetRoleColor(this Player player , string i)
        {
            player.ReferenceHub.serverRoles.SetColor(i);
        }
    }
}
