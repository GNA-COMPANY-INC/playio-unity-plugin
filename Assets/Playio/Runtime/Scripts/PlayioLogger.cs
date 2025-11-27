using UnityEngine;

namespace PlayioSDK
{
    internal static class PlayioLogger
    {
        private static LogLevel currentLogLevel = LogLevel.INFO;

        public static void SetLogLevel(LogLevel level)
        {
            currentLogLevel = level;
        }

        public static void Log(string message)
        {
            if (currentLogLevel <= LogLevel.DEBUG)
            {
                Debug.Log($"[Playio] {message}");
            }
        }

        public static void LogInfo(string message)
        {
            if (currentLogLevel <= LogLevel.INFO)
            {
                Debug.Log($"[Playio] {message}");
            }
        }

        public static void LogWarning(string message)
        {
            if (currentLogLevel <= LogLevel.WARN)
            {
                Debug.LogWarning($"[Playio] {message}");
            }
        }

        public static void LogError(string message)
        {
            if (currentLogLevel <= LogLevel.ERROR)
            {
                Debug.LogError($"[Playio] {message}");
            }
        }
    }
}
