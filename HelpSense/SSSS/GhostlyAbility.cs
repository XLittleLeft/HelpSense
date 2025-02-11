using CustomPlayerEffects;
using HelpSense.ConfigSystem;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UserSettings.ServerSpecific;
using UserSettings.ServerSpecific.Examples;
using HelpSense.API.Events;

namespace HelpSense.SSSS
{
    public class GhostlyAbility : SSExampleImplementationBase
    {
        public override string Name => SSSSTranslateConfig.DoorPiercingAbility;

        public static SSSSTranslateConfig SSSSTranslateConfig = CustomEventHandler.SSSSTranslateConfig;

        public static List<ReferenceHub> _activeGhostly = [];

        public override void Activate()
        {

        }

        public static void Load(Player Player)
        {
            ServerSpecificSettingsSync.DefinedSettings =
                [
                new SSGroupHeader(SSSSTranslateConfig.DoorPiercingAbility),
                new SSKeybindSetting((int)SettingType.GhostlyKey , SSSSTranslateConfig.DoorPiercingAbilityKey , KeyCode.F , hint: SSSSTranslateConfig.DoorPiercingAbilityKeyDescription),
                new SSTwoButtonsSetting((int)SettingType.GhostlyToggle , SSSSTranslateConfig.PiercingSkillActivationMode , SSSSTranslateConfig.Hold , SSSSTranslateConfig.Toggle),
                ];

            ServerSpecificSettingsSync.SendToPlayer(Player.ReferenceHub);
            ServerSpecificSettingsSync.ServerOnSettingValueReceived += ProcessUserInput;
        }

        public static void Unload(Player Player)
        {
            ServerSpecificSettingsSync.DefinedSettings =
                [
                
                ];

            ServerSpecificSettingsSync.SendToPlayer(Player.ReferenceHub);
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ProcessUserInput;
        }

        public override void Deactivate()
        {
            ServerSpecificSettingsSync.ServerOnSettingValueReceived -= ProcessUserInput;
        }

        public static void ProcessUserInput(ReferenceHub hub , ServerSpecificSettingBase setting)
        {
            switch ((SettingType)setting.SettingId)
            {
                case SettingType.GhostlyKey
                when setting is SSKeybindSetting keybind :
                    {
                        bool Mode = ServerSpecificSettingsSync.GetSettingOfUser<SSTwoButtonsSetting>(hub, (int)SettingType.GhostlyToggle).SyncIsA;

                        if (Mode)
                        {
                            AddGhostly(hub, keybind.SyncIsPressed);
                        }
                        else
                        {
                            if (!keybind.SyncIsPressed) break;
                            AddGhostly(hub, !_activeGhostly.Contains(hub));
                        }

                        break;
                    }
            }
        }

        public static void AddGhostly(ReferenceHub hub , bool flag)
        {
            Ghostly ghostly = hub.playerEffectsController.GetEffect<Ghostly>();

            if (flag && hub.IsHuman())
            {
                ghostly.ServerSetState(1);
                _activeGhostly.Add(hub);
            }
            else
            {
                ghostly.ServerDisable();
                _activeGhostly.Remove(hub);
            }
        }

        public enum SettingType
        {
            GhostlyKey,
            GhostlyToggle,
        }
    }
}
