using MEC;
using PluginAPI.Core;
using System;
using System.Reflection;

namespace HelpSense.Hint
{
    internal class HintServiceMeowProvider : IHintProvider
    {
        private readonly object _hint;
        private readonly PropertyInfo _hintTextProperty;
        private DateTime _timeToRemove;

        public Player Player { get; }

        internal HintServiceMeowProvider(Player player)
        {
            Player = player;

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            //Initialize hint instance
            foreach (var assembly in loadedAssemblies)
            {
                Type hintClass = assembly.GetType("HintServiceMeow.Core.Models.Hints.Hint");
                if (hintClass != null)
                {
                    _hint = Activator.CreateInstance(hintClass);
                    _hintTextProperty = hintClass.GetProperty("Text");

                    break;
                }
            }

            //Add hint into player display
            foreach (var assembly in loadedAssemblies)
            {
                Type playerDisplayClass = assembly.GetType("HintServiceMeow.Core.Utilities.PlayerDisplay");
                if (playerDisplayClass != null)
                {
                    MethodInfo getMethod = playerDisplayClass.GetMethod("Get", new[] { typeof(ReferenceHub) });
                    object playerDisplay = getMethod?.Invoke(null, new object[] { player.ReferenceHub });

                    MethodInfo addHintMethod = playerDisplayClass.GetMethod("AddHint", new[] { _hint?.GetType() });
                    addHintMethod?.Invoke(playerDisplay, new[] { _hint });

                    break;
                }
            }
        }

        public void ShowHint(string message, float duration)
        {
            _hintTextProperty.SetValue(_hint, message);
            _timeToRemove = DateTime.Now.AddSeconds(duration - 0.1f);

            Timing.CallDelayed(duration, () =>
            {
                if (DateTime.Now > _timeToRemove)
                    _hintTextProperty.SetValue(_hint, string.Empty);
            });
        }
    }
}
