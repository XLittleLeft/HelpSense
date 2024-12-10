using CustomPlayerEffects;
using HelpSense.ConfigSystem;
using HintServiceMeow.UI.Extension;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using System.Collections.Generic;

namespace HelpSense.Helper.SpecialRole
{
    public static class SpecialRoleHelper
    {
        public static IEnumerator<float> SpecialRoleInfoHandle()
        {
            TranslateConfig config = Plugin.Instance.TranslateConfig;
            int SCP703ItemTime = 0;
            string SCP029SpecialIntroduction = config.SCP029SpecialIntroduction;
            string SCP703SpecialIntroduction = config.SCP703SpecialIntroduction;
            string SCP347SpecialIntroduction = config.SCP347SpecialIntroduction;
            string SCP1093SpecialIntroduction = config.SCP1093SpecialIntroduction;
            string SCP191SpecialIntroduction = config.SCP191SpecialIntroduction;
            string SCP2936SpecialIntroduction = config.SCP2936SpecialIntroduction;
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

                if (Plugin.Instance.SCP029 != null && Plugin.Instance.SCP029.Player != null)
                {
                    Plugin.Instance.SCP029.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP029SpecialIntroduction, config.SCP029SkillIntroduction, 1.25f);
                }

                if (Plugin.Instance.SCP703 != null && Plugin.Instance.SCP703.Player != null)
                {
                    if (SCP703ItemTime == 0)
                    {
                        if (!Plugin.Instance.SCP703.Player.IsInventoryFull)
                        {
                            ItemType itemType = XHelper.GetRandomItem();
                            if (itemType.IsWeapon())
                            {
                                var firearm = Plugin.Instance.SCP703.Player.AddItem(itemType);
                                //TODO:子弹
                                //((Firearm)firearm).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());
                            }
                            else
                            {
                                Plugin.Instance.SCP703.Player.AddItem(itemType);
                            }

                            Plugin.Instance.SCP703.Player.GetPlayerUi().CommonHint.ShowOtherHint(config.SCP703ReceivedItemHint, 5);
                            SCP703ItemTime = Plugin.Instance.Config.SCP703ItemTime * 60;
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

                    Plugin.Instance.SCP703.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP703SpecialIntroduction, [.. SCP703StringList], 1.25f);
                }

                if (Plugin.Instance.SCP347 != null && Plugin.Instance.SCP347.Player != null)
                {
                    Plugin.Instance.SCP347.Player.EffectsManager.EnableEffect<Invisible>();

                    Plugin.Instance.SCP347.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP347SpecialIntroduction, config.SCP347SkillIntroduction, 1.25f);
                }

                if (Plugin.Instance.SCP1093 != null && Plugin.Instance.SCP1093.Player != null)
                {
                    Plugin.Instance.SCP1093.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP1093SpecialIntroduction, [.. config.SCP1093SkillIntroduction], 1.25f);
                }

                if (Plugin.Instance.SCP2936 != null && Plugin.Instance.SCP2936.Player != null)
                {
                    Plugin.Instance.SCP2936.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP2936SpecialIntroduction, config.SCP29361SkillIntroduction, 1.25f);
                }

                if (Plugin.Instance.SCP073 != null && Plugin.Instance.SCP073.Player != null)
                {
                    switch (Plugin.Instance.SCP073.Player.Team)
                    {
                        case Team.FoundationForces:
                            Plugin.Instance.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP073AbelSpecialIntroduction, config.SCP073AbelSkillIntroduction, 1.25f);
                            break;
                        case Team.ChaosInsurgency:
                            Plugin.Instance.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP073CainSpecialIntroduction, config.SCP073CainSkillIntroduction, 1.25f);
                            break;
                    }
                }

                if (Plugin.Instance.SCP191 != null && Plugin.Instance.SCP191.Player != null)
                {
                    Plugin.Instance.SCP191.Player.GetPlayerUi().CommonHint.ShowRoleHint(SCP191SpecialIntroduction, [.. config.SCP191SkillIntroduction], 1.25f);
                }

                if (!Plugin.Instance.SkynetPlayers.IsEmpty())
                {
                    foreach (var Player in Plugin.Instance.SkynetPlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(SkynetSpecialIntroduction, [.. config.SkynetSkillIntroduction], 1.25f);
                    }
                }
                if (!Plugin.Instance.SeePlayers.IsEmpty())
                {
                    foreach (var Player in Plugin.Instance.SeePlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(SeeNoEvilSpecialIntroduction, [.. config.SeeNoEvilSkillIntroduction], 1.25f);
                    }
                }
            }
        }
    }
}
