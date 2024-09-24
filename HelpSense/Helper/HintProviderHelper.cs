using HelpSense.Hint;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpSense.Helper
{
    // Type name should be the name of assembly of the provider's assembly
    public enum ProviderType
    {
        None,
        Default,
        HintServiceMeow
    }

    public static class HintProviderHelper
    {
        public static ProviderType Type { get; private set; } = ProviderType.None;

        public static IHintProvider CreateHintProvider(Player player)
        {
            //Initialize when not initialized
            if(Type == ProviderType.None)
            {
                Type = GetProviderType();
            }

            switch (Type)
            {
                case ProviderType.HintServiceMeow:
                    return new HintServiceMeowProvider(player);
            }

            return new DefaultHintProvider(player);
        }

        private static ProviderType GetProviderType()
        {
            var allCompatibleAssemblies = Enum
                .GetNames(typeof(ProviderType))
                .ToList();

            allCompatibleAssemblies.Remove("None");
            allCompatibleAssemblies.Remove("Default");

            //Check if any of the assembly exist
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = assembly.GetTypes();
                var compatibleAssembly = types.FirstOrDefault(t => allCompatibleAssemblies.Contains(t.Namespace));
                if (compatibleAssembly != null)
                {
                    return (ProviderType)Enum.Parse(typeof(ProviderType), compatibleAssembly.Namespace);
                }
            }

            return ProviderType.Default;
        }
    }
}
