using System.Collections.Generic;

namespace PlayioSDK
{
    partial class PlayioAPI
    {
        internal static void Init(PlayioConfig config)
        {
            NativeInterface.Init(config);
        }

        internal static void SetUserId(string userId)
        {
            NativeInterface.SetUserId(userId);
        }

        internal static void SetUserAttributes(Dictionary<string, string> attributes)
        {
            NativeInterface.SetUserAttributes(attributes);
        }

        internal static void SendEvent(string eventName, Dictionary<string, string> parameters)
        {
            NativeInterface.SendEvent(eventName, parameters);
        }

        internal static void Start()
        {
            NativeInterface.Start();
        }

        internal static void Stop()
        {
            NativeInterface.Stop();
        }

        internal static bool IsStopped()
        {
            return NativeInterface.IsStopped();
        }

        internal static string GetSdkVersion()
        {
            return NativeInterface.GetSdkVersion();
        }

        internal static void DisableCollectAdvertisingIdentifier(bool disable)
        {
            NativeInterface.DisableCollectAdvertisingIdentifier(disable);
        }
    }
}