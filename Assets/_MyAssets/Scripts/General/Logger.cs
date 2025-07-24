using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace General
{
    internal static class Logger
    {
        private const string SYMBOL = "MY_LOGGER_ON";

        [Conditional(SYMBOL)] internal static void Log(this object message) => Debug.Log(message);
        [Conditional(SYMBOL)] internal static void LogWarning(this object message) => Debug.LogWarning(message);
        [Conditional(SYMBOL)] internal static void LogError(this object message) => Debug.LogError(message);
        [Conditional(SYMBOL)] internal static void LogException(this System.Exception exception) => Debug.LogException(exception);
    }
}