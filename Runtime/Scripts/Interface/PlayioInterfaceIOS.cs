#if UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PlayioSDK
{
    internal class NativeInterface
    {
        [DllImport("__Internal")]
        private static extern void PlayioInit(string clientId, int logLevel);

        [DllImport("__Internal")]
        private static extern void PlayioSetUserId(string userId);

        [DllImport("__Internal")]
        private static extern void PlayioSetUserAttributes(string attributesJson);

        [DllImport("__Internal")]
        private static extern void PlayioSendEvent(string eventName, string parametersJson);

        [DllImport("__Internal")]
        private static extern void PlayioStart();

        [DllImport("__Internal")]
        private static extern void PlayioStop();

        [DllImport("__Internal")]
        private static extern bool PlayioIsStopped();

        [DllImport("__Internal")]
        private static extern string PlayioGetSdkVersion();

        [DllImport("__Internal")]
        private static extern void PlayioDisableCollectAdvertisingIdentifier(bool disable);

        internal static void Init(PlayioConfig config)
        {
            if (!Application.isPlaying) { return; }
            PlayioInit(config.clientId, (int)config.logLevel);
        }

        internal static void SetUserId(string userId)
        {
            if (!Application.isPlaying) { return; }
            PlayioSetUserId(userId);
        }

        internal static void SetUserAttributes(Dictionary<string, string> attributes)
        {
            if (!Application.isPlaying) { return; }
            string json = DictionaryToJson(attributes);
            PlayioSetUserAttributes(json);
        }

        internal static void SendEvent(string eventName, Dictionary<string, string> parameters)
        {
            if (!Application.isPlaying) { return; }
            string json = DictionaryToJson(parameters);
            PlayioSendEvent(eventName, json);
        }

        internal static void Start()
        {
            if (!Application.isPlaying) { return; }
            PlayioStart();
        }

        internal static void Stop()
        {
            if (!Application.isPlaying) { return; }
            PlayioStop();
        }

        internal static bool IsStopped()
        {
            if (!Application.isPlaying) { return true; }
            return PlayioIsStopped();
        }

        internal static string GetSdkVersion()
        {
            if (!Application.isPlaying) { return ""; }
            return PlayioGetSdkVersion();
        }

        internal static void DisableCollectAdvertisingIdentifier(bool disable)
        {
            if (!Application.isPlaying) { return; }
            PlayioDisableCollectAdvertisingIdentifier(disable);
        }

        private static string DictionaryToJson(Dictionary<string, string> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return "{}";
            }

            var pairs = new System.Collections.Generic.List<string>();
            foreach (var kvp in dictionary)
            {
                string key = EscapeJsonString(kvp.Key);
                string value = EscapeJsonString(kvp.Value);
                pairs.Add($"\"{key}\":\"{value}\"");
            }

            return "{" + string.Join(",", pairs.ToArray()) + "}";
        }

        private static string EscapeJsonString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }

            return str.Replace("\\", "\\\\")
                      .Replace("\"", "\\\"")
                      .Replace("\n", "\\n")
                      .Replace("\r", "\\r")
                      .Replace("\t", "\\t");
        }
    }
}
#endif
