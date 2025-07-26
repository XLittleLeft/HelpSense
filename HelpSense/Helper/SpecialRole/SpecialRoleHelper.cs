using CustomPlayerEffects;
using HelpSense.ConfigSystem;
using HintServiceMeow.UI.Extension;
using InventorySystem.Items;
using MEC;
using PlayerRoles;
using LabApi.Features.Wrappers;
using System.Collections.Generic;
using HelpSense.API.Events;

namespace HelpSense.Helper.SpecialRole
{
    public static class SpecialRoleHelper
    {
        public static int SCP703ItemTime = 0;
        public static int battry = 5000;

        public static IEnumerator<float> SpecialRoleInfoHandle()
        {
            TranslateConfig config = CustomEventHandler.TranslateConfig;
            string SCP029SpecialIntroduction = config.SCP029SpecialIntroduction;
            string SCP703SpecialIntroduction = config.SCP703SpecialIntroduction;
            string SCP347SpecialIntroduction = config.SCP347SpecialIntroduction;
            string SCP1093SpecialIntroduction = config.SCP1093SpecialIntroduction;
            string SCP191SpecialIntroduction = config.SCP191SpecialIntroduction;
            string SCP2936SpecialIntroduction = config.SCP2936SpecialIntroduction;
            string SCP023SpecialIntroduction = config.SCP023SpecialIntroduction;
            string SCP073AbelSpecialIntroduction = config.SCP073AbelSpecialIntroduction;
            string SCP073CainSpecialIntroduction = config.SCP073CainSpecialIntroduction;
            string SkynetSpecialIntroduction = config.SkynetSpecialIntroduction;
            string SeeNoEvilSpecialIntroduction = config.SeeNoEvilSpecialIntroduction;

            List<string> SCP703StringList = config.SCP703SkillIntroduction;

            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }

                if (CustomEventHandler.SCP029 != null && CustomEventHandler.SCP029.Player != null)
                {
                    CustomEventHandler.SCP029.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP029SpecialIntroduction, config.SCP029SkillIntroduction, 1.25f);
                    if (!CustomEventHandler.SCP029.Player.HasEffect<MovementBoost>())
                        CustomEventHandler.SCP029.Player.EnableEffect<MovementBoost>(20);
                    if (!CustomEventHandler.SCP029.Player.HasEffect<Scp1853>())
                        CustomEventHandler.SCP029.Player.EnableEffect<Scp1853>(2);
                    if (!CustomEventHandler.SCP029.Player.HasEffect<DamageReduction>())
                        CustomEventHandler.SCP029.Player.EnableEffect<DamageReduction>(15);
                }

                if (CustomEventHandler.SCP703 != null && CustomEventHandler.SCP703.Player != null)
                {
                    if (SCP703ItemTime == 0)
                    {
                        if (!CustomEventHandler.SCP703.Player.IsInventoryFull)
                        {
                            ItemType itemType = XHelper.GetRandomItem();
                            if (itemType.IsWeapon())
                            {
                                var firearm = CustomEventHandler.SCP703.Player.AddItem(itemType, ItemAddReason.AdminCommand);
                            }
                            else
                            {
                                CustomEventHandler.SCP703.Player.AddItem(itemType);
                            }

                            CustomEventHandler.SCP703.Player.GetPlayerUi().CommonHint.ShowOtherHint(config.SCP703ReceivedItemHint, 5);
                            SCP703ItemTime = CustomEventHandler.Config.SCP703ItemTime * 60;
                        }
                    }
                    else
                    {
                        SCP703ItemTime--;
                    }

                    SCP703StringList.ForEach(i =>
                    {
                        int index = SCP703StringList.IndexOf(i);
                        if (index >= 0)
                        {
                            SCP703StringList[index] = i.Replace("%Time%", SCP703ItemTime.ToString());
                        }
                    });

                    CustomEventHandler.SCP703.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP703SpecialIntroduction, [.. SCP703StringList], 1.25f);
                }

                if (CustomEventHandler.SCP347 != null && CustomEventHandler.SCP347.Player != null)
                {
                    CustomEventHandler.SCP347.Player.EnableEffect<Invisible>(1);

                    CustomEventHandler.SCP347.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP347SpecialIntroduction, config.SCP347SkillIntroduction, 1.25f);
                }

                if (CustomEventHandler.SCP1093 != null && CustomEventHandler.SCP1093.Player != null)
                {
                    CustomEventHandler.SCP1093.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP1093SpecialIntroduction, [.. config.SCP1093SkillIntroduction], 1.25f);
                }

                if (CustomEventHandler.SCP2936 != null && CustomEventHandler.SCP2936.Player != null)
                {
                    CustomEventHandler.SCP2936.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP2936SpecialIntroduction, config.SCP29361SkillIntroduction, 1.25f);
                }

                if (CustomEventHandler.SCP073 != null && CustomEventHandler.SCP073.Player != null)
                {
                    switch (CustomEventHandler.SCP073.Player.Team)
                    {
                        case Team.FoundationForces:
                            CustomEventHandler.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP073AbelSpecialIntroduction, config.SCP073AbelSkillIntroduction, 1.25f);
                            break;
                        case Team.ChaosInsurgency:
                            CustomEventHandler.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP073CainSpecialIntroduction, config.SCP073CainSkillIntroduction, 1.25f);
                            break;
                    }
                }

                if (CustomEventHandler.SCP191 != null && CustomEventHandler.SCP191.Player != null)
                {
                    CustomEventHandler.SCP191.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP191SpecialIntroduction, [.. config.SCP191SkillIntroduction], 1.25f);

                    CustomEventHandler.SCP191.Player.SendHint(CustomEventHandler.TranslateConfig.SCP191BatteryHintShow.Replace("%Battery%", battry.ToString()), 1.25f);//Use compatibility adapter

                    if (CustomEventHandler.SCP191.Player.Room.Name is MapGeneration.RoomName.Hcz079)
                    {
                        if (battry <= 4000)
                            battry += 1000;
                        else if (battry <= 5000)
                            battry = 5100;
                    }

                    battry -= 10;

                    if (battry <= 0)
                        CustomEventHandler.SCP191.Player.Kill(CustomEventHandler.TranslateConfig.SCP191BatteryDepletionDeathReason);
                }

                if (CustomEventHandler.SCP023 != null && CustomEventHandler.SCP023.Player != null)
                {
                    CustomEventHandler.SCP023.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP023SpecialIntroduction, [.. config.SCP023SkillIntroduction], 1.25f);
                }

                if (!CustomEventHandler.SkynetPlayers.IsEmpty())
                {
                    foreach (Player Player in CustomEventHandler.SkynetPlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(SkynetSpecialIntroduction, [.. config.SkynetSkillIntroduction], 1.25f);
                    }
                }
                if (!CustomEventHandler.SeePlayers.IsEmpty())
                {
                    foreach (Player Player in CustomEventHandler.SeePlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(SeeNoEvilSpecialIntroduction, [.. config.SeeNoEvilSkillIntroduction], 1.25f);
                    }
                }
            }
        }

        public static void Reset()
        {
            SCP703ItemTime = 0;
            battry = 5000;
        }
    }
}
