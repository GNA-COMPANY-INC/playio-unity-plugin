#if !UNITY_ANDROID && !UNITY_IOS
using System.Collections.Generic;

namespace PlayioSDK
{
    internal class NativeInterface
    {
        internal static void Init(PlayioConfig config) { }

        internal static void SetUserId(string userId) { }

        internal static void SetUserAttributes(Dictionary<string, object> attributes) { }

        internal static void SendEvent(string eventName, Dictionary<string, object> parameters) { }

        internal static void Start() { }

        internal static void Stop() {}

        internal static bool IsStopped() { return false; }

        internal static string GetSdkVersion() { return ""; }

        internal static void DisableCollectAdvertisingIdentifier(bool disable) { }
    }
}
#endif