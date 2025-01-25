using CommandSystem;
using HelpSense.API.Features.Pool;
using HelpSense.ConfigSystem;
using System;

namespace HelpSense.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class VersionCommand : ICommand
    {
        public string Command => "Version";

        public string[] Aliases => ["VersionInfo", "VI"];

        public string Description => "查询HelpSense插件版本和信息";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var config = Plugin.Instance.Config;
            CommandTranslateConfig CommandTranslateConfig = Plugin.Instance.CommandTranslateConfig;
            var sb = StringBuilderPool.Pool.Get();

            //Version and update time
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["PluginVersion"], Plugin.PluginVersion).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["LastUpdateTime"], Plugin.LastUpdateTime).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["RequiredGameVersion"], Plugin.RequiredGameVersion).AppendLine();

            //Function list
            sb.AppendLine(CommandTranslateConfig.VersionCommand["Text"]);
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SeeNoEvil"], BoolTranslate(config.EnableSeeNoEvil)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["Skynet"], BoolTranslate(config.EnableSkynet)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP023"], BoolTranslate(config.SCP023)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP029"], BoolTranslate(config.EnableSCP029)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP073"], BoolTranslate(config.SCP073)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP191"], BoolTranslate(config.SCP191)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP347"], BoolTranslate(config.SCP347)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP703"], BoolTranslate(config.EnableSCP703)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP1056"], BoolTranslate(config.SCP1056)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP1068"], BoolTranslate(config.SCP1068)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["ChaosLeader"], BoolTranslate(config.EnableChaosLeader)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP2936"], BoolTranslate(config.SCP2936)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["SCP1093"], BoolTranslate(config.SCP1093)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["InfiniteAmmo"], BoolTranslate(config.InfiniteAmmo)).AppendLine();
            sb.AppendFormat(CommandTranslateConfig.VersionCommand["InfiniteAmmoType"], Plugin.Instance.Config.InfiniteAmmoType).AppendLine();

            //Copyright
            sb.AppendLine("-Made By X小左(XLittleLeft)-");
            sb.AppendLine("Copyright © X小左 2022-2025");

            response = sb.ToString();
            StringBuilderPool.Pool.Return(sb);

            return true;
        }

        public string BoolTranslate(bool value) => value ? "✔" : "✖";
    }
}
