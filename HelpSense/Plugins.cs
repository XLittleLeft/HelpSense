using HarmonyLib;
using HelpSense.API.Events;
using HelpSense.ConfigSystem;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using System;

namespace HelpSense
{
    public class Plugins : Plugin
    {
        public CustomEventHandler Events { get; } = new();

        private Harmony _harmony = new("cn.xlittleleft.plugin");

        public override void LoadConfigs()
        {
            base.LoadConfigs();

            CustomEventHandler.Config = this.LoadConfig<Config>("config.yml");
            CustomEventHandler.TranslateConfig = this.LoadConfig<TranslateConfig>("TranslateConfig.yml");
            CustomEventHandler.SSSSTranslateConfig = this.LoadConfig<SSSSTranslateConfig>("SSSSTranslateConfig.yml");
            CustomEventHandler.CommandTranslateConfig = this.LoadConfig<CommandTranslateConfig>("CommandTranslateConfig.yml");
        }

        public static DateTime LastUpdateTime => new(2025, 5, 25, 11, 03, 27);
        public static System.Version RequiredGameVersion => new(14, 1, 0);

        public static Plugins Instance { get; private set; }

        public override string Name => "HelpSense";

        public override string Description => "HelpSense综合服务器插件";

        public override string Author => "X小左";

        public override System.Version Version => new(1, 4, 2);

        public override System.Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);

        public override void Enable()
        {
            Instance = this;

            CustomHandlersManager.RegisterEventsHandler(Events);

            _harmony.PatchAll();
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);

            _harmony.UnpatchAll(_harmony.Id);

            Instance = null;
            _harmony = null;
        }
    }
}
