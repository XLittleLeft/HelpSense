using HelpSense.API.Features.Pool;
using Hints;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Spawnpoints;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Items;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;
using CustomPlayerEffects;
using static Broadcast;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using RelativePositioning;
using UnityEngine.UIElements;
using Interactables.Interobjects.DoorUtils;
using AdminToys;
using InventorySystem.Items;
using HelpSense.Hint;

namespace HelpSense.Helper
{
    public static class XHelper
    {
        public static Random Random = new Random();
        public static HashSet<Player> PlayerList = new HashSet<Player>();
        public static HashSet<Player> SpecialPlayerList = new HashSet<Player>();
        public static HashSet<IHintProvider> HintProviderList = new HashSet<IHintProvider>();
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
            if (players.Count() >= 1)
            {
                return players[new Random().Next(0, players.Count() - 1)];
            }
            else
            {
                return null;
            }
        }
        public static Player GetRandomPlayer(RoleTypeId roleTypeId , List<Player> playerlist)
        {
            List<Player> players = new List<Player>();
            foreach (Player player in playerlist)
            {
                if (player.Role == roleTypeId)
                {
                    players.Add(player);
                }
            }
            if (players.Count() >= 1)
            {
                return players[new Random().Next(0, players.Count() - 1)];
            }
            else
            {
                return null;
            }
        }
        public static Player GetRandomPlayer(List<Player> playerlist)
        {
            if (playerlist.Count() >= 1)
            {
                return playerlist[new Random().Next(0, playerlist.Count() - 1)];
            }
            else
            {
                return null;
            }
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
            if (players.Count() >= 1)
            {
                var randomplayer = players[new Random().Next(0, players.Count() - 1)];
                SpecialPlayerList.Remove(randomplayer);
                return randomplayer;
            }
            else
            {
                return null;
            }
        }
        public static ItemType GetRadomItem()
        {
            List<ItemType> itemTypes = new List<ItemType>();
            for (int i = 1; i <= 51; i++)
            {
                if (!((ItemType)(i)).IsAmmo())
                {
                    itemTypes.Add((ItemType)(i));
                }
            }
            return itemTypes[new Random().Next(0, itemTypes.Count() - 1)];
        }

        public static void SpawnItem(ItemType typeid, Vector3 Position, int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                ItemPickup.Create(typeid, Position, new Quaternion(0, 0, 0, 0)).Spawn();
            }
        }

        public static ItemPickup SpawnItem(ItemType typeid, Vector3 Position)
        {
            var item = ItemPickup.Create(typeid, Position, new Quaternion(0, 0, 0, 0));
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
        public static TimeSpan TimeUntilSpawnWave => TimeSpan.FromSeconds(RespawnManager.Singleton._timeForNextSequence - (float)RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);
        //防倒
        public static IEnumerator<float> AutoX()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(6 * 60f);
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
                Allbroadcast("<size=35><align=center><color=#F6511D>此服务器在运行X小左的插件，享受你的游戏时间~</color></align></size>", 6, Broadcast.BroadcastFlags.Normal);
            }
        }
        public static IEnumerator<float> AutoSX()
        {
            while (true)
            {
                if (Round.IsRoundEnded)
                {
                    yield break;
                }
                Allbroadcast(Plugin.Instance.Config.AutoServerMessageText, Plugin.Instance.Config.AutoServerMessageTimer, Broadcast.BroadcastFlags.Normal);
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.AutoServerMessageTime * 60f);
            }
        }
        public static IEnumerator<float> RandomItem(Player Player)
        {
            while (true)
            {
                if (!Player.IsInventoryFull)
                {
                    ItemType itemType = GetRadomItem();
                    if (itemType.IsWeapon())
                    {
                        var firearm = Player.AddItem(itemType);
                        ((Firearm)(firearm)).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());
                    }
                    else
                        Player.AddItem(itemType);
                    Player.GetHintProvider().ShowHint("获得一件物品", 5);
                }
                if (!Player.IsAlive || Round.IsRoundEnded)
                {
                    yield break;
                }
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.SCP703ItemTIme * 60f);
            }
        }

        public static IEnumerator<float> SCP191Handle(Player Player)
        {
            int D = 5000;
            while (true)
            {
                Player.GetHintProvider().ShowHint($"<align=right><size=60><b>你目前剩余的电量:<color=yellow>{D}安</color></size></b></align>",11);
                if (Player.Room.Name is MapGeneration.RoomName.Hcz079)
                    if (D <= 4000)
                        D += 1000;
                    else if (D <= 5000)
                        D = 5100;
                D -= 100;
                if (D == 0)
                    Player.Kill("电量耗尽");
                if (!Player.IsAlive || Round.IsRoundEnded)
                {
                    yield break;
                }
                yield return Timing.WaitForSeconds(10f);
            }
        }
        public static IEnumerator<float> SCP347Handle(Player Player)
        {
            while (true)
            {
                Player.EffectsManager.EnableEffect<Invisible>();
                if (!Player.IsAlive || Round.IsRoundEnded)
                {
                    yield break;
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public static bool IsSpecialPlayer(this Player Player)
        {
            if (Player.RoleName == "SCP-029" || Player.RoleName == "SCP-703" || Player.RoleName == "混沌领导者"
                || Player.RoleName == "SCP-191" || Player.RoleName == "SCP-073" || Player.RoleName == "SCP-2936-1")
            {
                return true;
            }
            return false;
        }

        public static bool BreakDoor(DoorVariant Base, DoorDamageType type = DoorDamageType.ServerCommand)
        {
            if (!(Base is IDamageableDoor damageableDoor) || damageableDoor.IsDestroyed)
                return false;
            damageableDoor.ServerDamage((float)ushort.MaxValue, type);
            return true;
        }

        public static void ReloadWeapen(this Player player)
        {
            if (player.CurrentItem == null)
            {
                //手上没有物品
                return;
            }
            else
            {
                if (player.CurrentItem is Firearm firearm)
                {
                    firearm.AmmoManagerModule.ServerTryReload();
                    player.Connection.Send<RequestMessage>(new RequestMessage(firearm.ItemSerial, RequestType.Reload));
                }
                else
                {
                    //持有的物品不是一个武器
                    return;
                }
            }
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
            if (type != ItemType.SCP018 && type != ItemType.SCP500 && type != ItemType.SCP268 && type != ItemType.SCP207 && type != ItemType.SCP244a && type != ItemType.SCP244b && type != ItemType.SCP2176)
            {
                return type == ItemType.SCP1853;
            }

            return true;
        }
        public static bool IsThrowable(this ItemType type)
        {
            if (type != ItemType.SCP018 && type != ItemType.GrenadeHE && type != ItemType.GrenadeFlash)
            {
                return type == ItemType.SCP2176;
            }

            return true;
        }
        public static bool IsMedical(this ItemType type)
        {
            if (type != ItemType.Painkillers && type != ItemType.Medkit && type != ItemType.SCP500)
            {
                return type == ItemType.Adrenaline;
            }

            return true;
        }
        public static bool IsUtility(this ItemType type)
        {
            if (type != ItemType.Flashlight)
            {
                return type == ItemType.Radio;
            }

            return true;
        }
        public static bool IsArmor(this ItemType type)
        {
            if (type != ItemType.ArmorCombat && type != ItemType.ArmorHeavy)
            {
                return type == ItemType.ArmorLight;
            }

            return true;
        }
        public static bool IsKeycard(this ItemType type)
        {
            if (type != 0 && type != ItemType.KeycardScientist && type != ItemType.KeycardResearchCoordinator && type != ItemType.KeycardZoneManager && type != ItemType.KeycardGuard && type != ItemType.KeycardMTFPrivate && type != ItemType.KeycardContainmentEngineer && type != ItemType.KeycardMTFOperative && type != ItemType.KeycardMTFCaptain && type != ItemType.KeycardFacilityManager && type != ItemType.KeycardChaosInsurgency)
            {
                return type == ItemType.KeycardO5;
            }

            return true;
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

        public static void Mybroadcast(this Player player, string text, ushort time, Broadcast.BroadcastFlags broadcastFlags)
        {
            Broadcast.Singleton.GetComponent<Broadcast>().TargetAddElement(player.ReferenceHub.characterClassManager.connectionToClient, text, time, broadcastFlags);
        }

        public static void Allbroadcast(string text, ushort time, Broadcast.BroadcastFlags broadcastFlags)
        {
            Broadcast.Singleton.GetComponent<Broadcast>().RpcAddElement(text, time, broadcastFlags);
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
            foreach (var Player in PlayerList.Where(x => x.PlayerId != player.PlayerId && x.IsReady))
            {
                Player.Connection.Send(new RoleSyncInfo(player.ReferenceHub , type , Player.ReferenceHub));
            }
        }

        public static bool TryGetRoleBase(this RoleTypeId roleType, out PlayerRoleBase roleBase)
        {
            return PlayerRoleLoader.TryGetRoleTemplate<PlayerRoleBase>(roleType, out roleBase);
        }

        public static IEnumerator<float> PositionCheckerCoroutine(Player player)
        {
            Vector3 position = player.Position;
            float health = player.Health;
            int TimeChecker = 0;

            while (true)
            {
                if (position != player.Position || health != player.Health)
                {
                    TimeChecker = 0;
                    position = player.Position;
                    health = player.Health;
                }
                else
                {
                    TimeChecker++;

                    if (TimeChecker >= 2)
                    {
                        TimeChecker = 0;
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
