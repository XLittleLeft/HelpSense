using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;

using PlayerRoles;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles.FirstPersonControl;

using PluginAPI.Core;
using PluginAPI.Core.Items;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MEC;
using Respawning;
using UnityEngine;
using CustomPlayerEffects;
using Interactables.Interobjects.DoorUtils;
using Mirror;

using HelpSense.Hint;
using HelpSense.API.Features.Pool;

namespace HelpSense.Helper
{
    public static class XHelper
    {
        public static System.Random Random = new(DateTime.Now.GetHashCode());
        public static HashSet<Player> PlayerList = new();
        public static HashSet<Player> SpecialPlayerList = new();
        public static HashSet<IHintProvider> HintProviderList = new();

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
        public static Player GetRandomPlayer(RoleTypeId roleTypeId , List<Player> playerList)
        {
            List<Player> players = new List<Player>();
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
            List<Player> players = new List<Player>();
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
                ItemPickup.Create(typeid, position, new Quaternion(0, 0, 0, 0)).Spawn();
            }
        }
        public static ItemPickup SpawnItem(ItemType typeid, Vector3 position)
        {
            var item = ItemPickup.Create(typeid, position, new Quaternion(0, 0, 0, 0));
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
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
                Broadcast("<size=35><align=center><color=#F6511D>此服务器在运行X小左的插件，享受你的游戏时间~</color></align></size>", 6, global::Broadcast.BroadcastFlags.Normal);
            }
        }
        public static IEnumerator<float> AutoServerBroadcast()
        {
            while (!Round.IsRoundEnded)
            {
                Broadcast(Plugin.Instance.Config.AutoServerMessageText, Plugin.Instance.Config.AutoServerMessageTimer, global::Broadcast.BroadcastFlags.Normal);
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.AutoServerMessageTime * 60f);
            }
        }

        public static IEnumerator<float> GiveRandomItem(this Player player)
        {
            while (true)
            {
                if (!player.IsAlive || Round.IsRoundEnded)
                {
                    yield break;
                }

                if (!player.IsInventoryFull)
                {
                    ItemType itemType = GetRandomItem();
                    if (itemType.IsWeapon())
                    {
                        var firearm = player.AddItem(itemType);
                        ((Firearm)firearm).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());
                    }
                    else
                    {
                        player.AddItem(itemType);
                    }
                    
                    player.GetHintProvider().ShowHint("获得一件物品", 5);
                }

                yield return Timing.WaitForSeconds(Plugin.Instance.Config.SCP703ItemTIme * 60f);
            }
        }

        public static IEnumerator<float> SCP191CoroutineMethod(Player player)
        {
            int d = 5000;
            while (!Round.IsRoundEnded && player is not null && player.IsAlive)
            {
                player.GetHintProvider().ShowHint($"<align=right><size=60><b>你目前剩余的电量:<color=yellow>{d}安</color></size></b></align>",11);
                if (player.Room.Name is MapGeneration.RoomName.Hcz079)
                {
                    if (d <= 4000)
                        d += 1000;
                    else if (d <= 5000)
                        d = 5100;
                }

                d -= 100;

                if (d <= 0)
                    player.Kill("电量耗尽");

                yield return Timing.WaitForSeconds(10f);
            }
        }
        public static IEnumerator<float> SCP347CoroutineMethod(Player player)
        {
            while (!Round.IsRoundEnded && player is not null && player.IsAlive)
            {
                player.EffectsManager.EnableEffect<Invisible>();

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static bool IsSpecialPlayer(this Player player)
        {
            return player.RoleName is "SCP-029" or "SCP-703" or "混沌领导者" or "SCP-191" or "SCP-073" or "SCP-2936-1";
        }

        public static bool BreakDoor(DoorVariant doorBase, DoorDamageType type = DoorDamageType.ServerCommand)
        {
            if (doorBase is not IDamageableDoor damageableDoor || damageableDoor.IsDestroyed)
                return false;

            damageableDoor.ServerDamage(ushort.MaxValue, type);
            return true;
        }

        public static void ReloadWeapon(this Player player)
        {
            if (player.CurrentItem == null)
                return;

            if (player.CurrentItem is not Firearm firearm)
                return;

            firearm.AmmoManagerModule.ServerTryReload();
            player.Connection.Send(new RequestMessage(firearm.ItemSerial, RequestType.Reload));
        }

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
            switch (typeId)
            {
                case RoleTypeId.ChaosConscript:
                case RoleTypeId.ChaosRifleman:
                case RoleTypeId.ChaosRepressor:
                case RoleTypeId.ChaosMarauder:
                    return Team.ChaosInsurgency;
                case RoleTypeId.Scientist:
                    return Team.Scientists;
                case RoleTypeId.ClassD:
                    return Team.ClassD;
                case RoleTypeId.Scp173:
                case RoleTypeId.Scp106:
                case RoleTypeId.Scp049:
                case RoleTypeId.Scp079:
                case RoleTypeId.Scp096:
                case RoleTypeId.Scp0492:
                case RoleTypeId.Scp939:
                    return Team.SCPs;
                case RoleTypeId.NtfSpecialist:
                case RoleTypeId.NtfSergeant:
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.FacilityGuard:
                    return Team.FoundationForces;
                case RoleTypeId.Tutorial:
                    return Team.OtherAlive;
                default:
                    return Team.Dead;
            }
        }

        public static void Broadcast(string text, ushort time, Broadcast.BroadcastFlags broadcastFlags)
        {
            global::Broadcast.Singleton.GetComponent<Broadcast>().RpcAddElement(text, time, broadcastFlags);
        }

        public static void ShowBroadcast(this Player player, string text, ushort time, Broadcast.BroadcastFlags broadcastFlags)
        {
            global::Broadcast.Singleton.GetComponent<Broadcast>().TargetAddElement(player.ReferenceHub.characterClassManager.connectionToClient, text, time, broadcastFlags);
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
                pl.Connection.Send(new RoleSyncInfo(player.ReferenceHub , type , pl.ReferenceHub));
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

                if (!player.IsAlive || Round.IsRoundEnded)
                {
                    yield break;
                }

                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static bool IsSameTeam(this Player player1, Player player2)
        {
            if (player1.Team is Team.FoundationForces && player2.Team is Team.Scientists)
            {
                return true;
            }

            if (player1.Team is Team.ChaosInsurgency && player2.Team is Team.ClassD)
            {
                return true;
            }

            return false;
        }

        public static IHintProvider GetHintProvider(this Player player)
        {
            var result = HintProviderList.FirstOrDefault(x => x.Player == player);

            if(result == null)
            {
                result = HintProviderHelper.CreateHintProvider(player);
                HintProviderList.Add(result);
            }

            return result;
        }
    }
}
