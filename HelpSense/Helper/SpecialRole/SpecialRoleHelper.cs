using CustomPlayerEffects;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.UI.Extension;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Test;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Helper.SpecialRole
{
    public static class SpecialRoleHelper
    {
        public static IEnumerator<float> SpecialRoleInfoHandle()
        {
            int scp703ItemTime = 0;
            string scp029SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP029SpecialIntroduction;
            string scp703SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP703SpecialIntroduction;
            string scp347SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP347SpecialIntroduction;
            string scp1093SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP1093SpecialIntroduction;
            string scp191SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP191SpecialIntroduction;
            string scp2936SpecialIntroduction = Plugin.Instance.TranslateConfig.SCP2936SpecialIntroduction;
            string scp073AbelSpecialIntroduction = Plugin.Instance.TranslateConfig.SCP073AbelSpecialIntroduction;
            string scp073CainSpecialIntroduction = Plugin.Instance.TranslateConfig.SCP073CainSpecialIntroduction;
            string skynetSpecialIntroduction = Plugin.Instance.TranslateConfig.SkynetSpecialIntroduction;
            string seeNoEvilSpecialIntroduction = Plugin.Instance.TranslateConfig.SeeNoEvilSpecialIntroduction;

            List<string> scp703StringList = Plugin.Instance.TranslateConfig.SCP703SkillIntroduction;

            while (true)
            {
                yield return Timing.WaitForSeconds(1f);

                if (Round.IsRoundEnded || !Round.IsRoundStarted)
                {
                    yield break;
                }

                if (Plugin.Instance.SCP029 != null && Plugin.Instance.SCP029.Player != null)
                {
                    Plugin.Instance.SCP029.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp029SpecialIntroduction, Plugin.Instance.TranslateConfig.SCP029SkillIntroduction , 1.25f);
                }

                if (Plugin.Instance.SCP703 != null && Plugin.Instance.SCP703.Player != null)
                {
                    if (scp703ItemTime == 0)
                    {
                        if (!Plugin.Instance.SCP703.Player.IsInventoryFull)
                        {
                            ItemType itemType = XHelper.GetRandomItem();
                            if (itemType.IsWeapon())
                            {
                                var firearm = Plugin.Instance.SCP703.Player.AddItem(itemType);
                                ((Firearm)firearm).Status = new FirearmStatus(((Firearm)(firearm)).AmmoManagerModule.MaxAmmo, ((Firearm)(firearm)).Status.Flags, ((Firearm)(firearm)).GetCurrentAttachmentsCode());
                            }
                            else
                            {
                                Plugin.Instance.SCP703.Player.AddItem(itemType);
                            }

                            Plugin.Instance.SCP703.Player.GetPlayerUi().CommonHint.ShowOtherHint(Plugin.Instance.TranslateConfig.SCP703ReceivedItemHint, 5);
                            scp703ItemTime = Plugin.Instance.Config.SCP703ItemTime * 60;
                        }
                    }
                    else
                    {
                        scp703ItemTime--;
                    }

                    scp703StringList.ForEach(i =>
                    {
                        int index = scp703StringList.IndexOf(i);
                        if (index >= 0)
                        {
                            scp703StringList[index] = i.Replace("%Time%", scp703ItemTime.ToString());
                        }
                    });

                    Plugin.Instance.SCP703.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp703SpecialIntroduction, scp703StringList.ToArray(), 1.25f);
                }

                if (Plugin.Instance.SCP347 != null && Plugin.Instance.SCP347.Player != null)
                {
                    Plugin.Instance.SCP347.Player.EffectsManager.EnableEffect<Invisible>();

                    Plugin.Instance.SCP347.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp347SpecialIntroduction, Plugin.Instance.TranslateConfig.SCP347SkillIntroduction, 1.25f);
                }

                if (Plugin.Instance.SCP1093 != null && Plugin.Instance.SCP1093.Player != null)
                {
                    Plugin.Instance.SCP1093.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp1093SpecialIntroduction, Plugin.Instance.TranslateConfig.SCP1093SkillIntroduction.ToArray(), 1.25f);
                }

                if (Plugin.Instance.SCP2936 != null && Plugin.Instance.SCP2936.Player != null)
                {
                    Plugin.Instance.SCP2936.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp2936SpecialIntroduction, Plugin.Instance.TranslateConfig.SCP29361SkillIntroduction, 1.25f);
                }

                if (Plugin.Instance.SCP073 != null && Plugin.Instance.SCP073.Player != null)
                {
                    switch (Plugin.Instance.SCP073.Player.Team)
                    {
                        case Team.FoundationForces:
                            Plugin.Instance.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp073AbelSpecialIntroduction, Plugin.Instance.TranslateConfig.SCP073AbelSkillIntroduction, 1.25f);
                            break;
                        case Team.ChaosInsurgency:
                            Plugin.Instance.SCP073.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp073CainSpecialIntroduction, Plugin.Instance.TranslateConfig.SCP073CainSkillIntroduction, 1.25f);
                            break;
                    }
                }

                if (Plugin.Instance.SCP191 != null && Plugin.Instance.SCP191.Player != null)
                {
                    Plugin.Instance.SCP191.Player.GetPlayerUi().CommonHint.ShowRoleHint(scp191SpecialIntroduction, Plugin.Instance.TranslateConfig.SCP191SkillIntroduction.ToArray(), 1.25f);
                }

                if (!Plugin.Instance.SkynetPlayers.IsEmpty())
                {
                    foreach (var Player in Plugin.Instance.SkynetPlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(skynetSpecialIntroduction, Plugin.Instance.TranslateConfig.SkynetSkillIntroduction.ToArray(), 1.25f);
                    }
                }
                if (!Plugin.Instance.SeePlayers.IsEmpty())
                {
                    foreach (var Player in Plugin.Instance.SeePlayers)
                    {
                        Player.GetPlayerUi().CommonHint.ShowRoleHint(seeNoEvilSpecialIntroduction, Plugin.Instance.TranslateConfig.SeeNoEvilSkillIntroduction.ToArray(), 1.25f);
                    }
                }
            }
        }
    }
}
