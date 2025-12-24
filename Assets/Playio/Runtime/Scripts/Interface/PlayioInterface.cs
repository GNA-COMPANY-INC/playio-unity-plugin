#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)
using System.Collections.Generic;

namespace PlayioSDK
{
    internal class NativeInterface
    {
        internal static void Init(PlayioConfig config)
        {
            PlayioLogger.Log($"Init - clientId: {config.clientId}, logLevel: {config.logLevel}");
        }

        internal static void SetUserId(string userId)
        {
            PlayioLogger.Log($"SetUserId - userId: {userId}");
        }

        internal static void SetUserAttributes(Dictionary<string, object> attributes)
        {
            PlayioLogger.Log($"SetUserAttributes - count: {attributes?.Count ?? 0}");
        }

        internal static void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            PlayioLogger.Log($"SendEvent - eventName: {eventName}, params count: {parameters?.Count ?? 0}");
        }

        internal static void Start()
        {
            PlayioLogger.Log("Start");
        }

        internal static void Stop()
        {
            PlayioLogger.Log("Stop");
        }

        internal static bool IsStopped()
        {
            PlayioLogger.Log("IsStopped");
            return false;
        }

        internal static string GetSdkVersion()
        {
            PlayioLogger.Log("GetSdkVersion");
            return "editor";
        }

        internal static void DisableCollectAdvertisingIdentifier(bool disable)
        {
            PlayioLogger.Log($"DisableCollectAdvertisingIdentifier - disable: {disable}");
        }

        internal static void OnTimeEvent(long startMillis, long endMillis)
        {
            PlayioLogger.Log($"OnTimeEvent - startMillis: {startMillis}, endMillis: {endMillis}");
        }
    }
}
#endif