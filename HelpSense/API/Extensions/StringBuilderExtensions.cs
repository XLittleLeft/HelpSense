namespace HelpSense.API.Extensions
{
    public static class StringBuilderExtensions
    {
        //TODO:刷新
        /*public static StringBuilder SetAllProperties(this StringBuilder builder, int? spectatorCount = null)
        {
            return builder.SetRoundTime().SetMinutesAndSeconds().SetSpawnableTeam().SetSpectatorCountAndTickets(spectatorCount).SetGeneratorCount().SetTpsAndTickrate().SetHint().SetWarhead();
        }

        private static StringBuilder SetRoundTime(this StringBuilder builder)
        {
            int minutes = RoundStart.RoundLength.Minutes;
            builder.Replace("{round_minutes}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && minutes < 10) ? "0" : string.Empty, minutes));
            int seconds = RoundStart.RoundLength.Seconds;
            builder.Replace("{round_seconds}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && seconds < 10) ? "0" : string.Empty, seconds));
            return builder;
        }

        private static StringBuilder SetMinutesAndSeconds(this StringBuilder builder)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds((double)RespawnManager.Singleton._timeForNextSequence - RespawnManager.Singleton._stopwatch.Elapsed.TotalSeconds);
            RespawnManager.RespawnSequencePhase curSequence = RespawnManager.Singleton._curSequence;
            bool flag = curSequence == RespawnSequencePhase.SelectingTeam || curSequence == RespawnSequencePhase.PlayingEntryAnimations || !TimerView.Current.Properties.TimerOffset;
            if (flag)
            {
                int num = (int)timeSpan.TotalSeconds / 60;
                builder.Replace("{minutes}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && num < 10) ? "0" : string.Empty, num));
                int num2 = (int)Math.Round(timeSpan.TotalSeconds % 60.0);
                builder.Replace("{seconds}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && num2 < 10) ? "0" : string.Empty, num2));
            }
            else
            {
                int num3 = (RespawnTokensManager.Counters[1].Amount >= 50f) ? 18 : 14;
                int num4 = (int)(timeSpan.TotalSeconds + (double)num3) / 60;
                builder.Replace("{minutes}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && num4 < 10) ? "0" : string.Empty, num4));
                int num5 = (int)Math.Round((timeSpan.TotalSeconds + (double)num3) % 60.0);
                builder.Replace("{seconds}", string.Format("{0}{1}", (Current.Properties.LeadingZeros && num5 < 10) ? "0" : string.Empty, num5));
            }
            return builder;
        }

        private static StringBuilder SetSpawnableTeam(this StringBuilder builder)
        {
            switch (Respawn.NextKnownTeam)
            {
                case SpawnableTeamType.None:
                    return builder;
                case SpawnableTeamType.ChaosInsurgency:
                    builder.Replace("{team}", Current.Properties.Ci);
                    break;
                case SpawnableTeamType.NineTailedFox:
                    builder.Replace("{team}", Current.Properties.Ntf);
                    break;
            }
            return builder;
        }

        private static StringBuilder SetSpectatorCountAndTickets(this StringBuilder builder, int? spectatorCount = null)
        {
            string oldValue = "{spectators_num}";
            string newValue;
            if ((newValue = ((spectatorCount != null) ? spectatorCount.GetValueOrDefault().ToString() : null)) == null)
            {
                newValue = XHelper.PlayerList.Count((Player x) => x.Role == RoleTypeId.Spectator && !x.IsOverwatchEnabled).ToString();
            }
            builder.Replace(oldValue, newValue);
            builder.Replace("{ntf_tickets_num}", Mathf.Round(RespawnTokensManager.Counters[1].Amount).ToString());
            builder.Replace("{ci_tickets_num}", Mathf.Round(RespawnTokensManager.Counters[0].Amount).ToString());
            return builder;
        }

        private static StringBuilder SetGeneratorCount(this StringBuilder builder)
        {
            builder.Replace("{generator_engaged}", Scp079Recontainer.AllGenerators.Count((Scp079Generator x) => x.Engaged).ToString());
            builder.Replace("{generator_count}", "3");
            return builder;
        }

        private static StringBuilder SetTpsAndTickrate(this StringBuilder builder)
        {
            builder.Replace("{tps}", Math.Round(1.0 / (double)Time.smoothDeltaTime).ToString(CultureInfo.InvariantCulture));
            builder.Replace("{tickrate}", Application.targetFrameRate.ToString());
            return builder;
        }
        private static StringBuilder SetHint(this StringBuilder builder)
        {
            bool flag = !Current.Hints.Any<string>();
            StringBuilder result;
            if (flag)
            {
                result = builder;
            }
            else
            {
                builder.Replace("{hint}", Current.Hints[Current.HintIndex]);
                result = builder;
            }
            return result;
        }
        private static StringBuilder SetWarhead(this StringBuilder builder)
        {
            builder.Replace("{warhead_status}", Warhead.IsDetonated ? "已经炸力" : Warhead.IsDetonationInProgress ? "正在启动,快run" : "核弹在睡觉");
            return builder;
        }*/
    }
}
