using PluginAPI.Core;

namespace HelpSense.Hint
{
    public interface IHintProvider
    {
        Player Player { get; }

        void ShowHint(string hint, float duration = 3);
    }
}
