#if !UNITY_ANDROID && !UNITY_IOS
using System.Collections.Generic;

namespace PlayioSDK
{
    internal class NativeInterface
    {
        internal static void Init(PlayioConfig config) { }

        internal static void SetUserId(string userId) { }

        internal static void SetUserAttributes(Dictionary<string, string> attributes) { }

        internal static void SendEvent(string eventName, Dictionary<string, string> parameters) { }

        internal static void Start() { }

        internal static void Stop() {}

        internal static bool IsStopped() { return false; }

        internal static string GetSdkVersion() { return ""; }

        internal static void DisableCollectAdvertisingIdentifier(bool disable) { }
    }
}
#endif