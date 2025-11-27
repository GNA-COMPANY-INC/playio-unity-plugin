#if UNITY_IOS
using System;
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

        internal static void SetUserAttributes(Dictionary<string, object> attributes)
        {
            if (!Application.isPlaying) { return; }
            string json = DictionaryToJson(attributes);
            PlayioSetUserAttributes(json);
        }

        internal static void SendEvent(string eventName, Dictionary<string, object> parameters)
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

        private static string DictionaryToJson(Dictionary<string, object> dictionary)
        {
            if (dictionary == null || dictionary.Count == 0)
            {
                return "{}";
            }

            var pairs = new System.Collections.Generic.List<string>();
            foreach (var kvp in dictionary)
            {
                string key = EscapeJsonString(kvp.Key);
                string value = ConvertValueToJson(kvp.Value);
                pairs.Add($"\"{key}\":{value}");
            }

            return "{" + string.Join(",", pairs.ToArray()) + "}";
        }

        private static string ConvertValueToJson(object value)
        {
            if (value == null)
            {
                return "null";
            }

            // Handle boolean
            if (value is bool boolValue)
            {
                return boolValue ? "true" : "false";
            }

            // Handle numeric types (no quotes)
            if (value is int || value is long || value is byte || value is sbyte || 
                value is short || value is ushort || value is uint || value is ulong)
            {
                // Integer types - use invariant culture
                return Convert.ToInt64(value).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            if (value is float floatValue)
            {
                return floatValue.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
            }

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            // Handle string (with quotes and escaping)
            if (value is string stringValue)
            {
                return "\"" + EscapeJsonString(stringValue) + "\"";
            }

            // For unsupported types, convert to string
            PlayioLogger.LogWarning($"Unsupported type {value.GetType()} in event parameters, converting to string");
            return "\"" + EscapeJsonString(value.ToString()) + "\"";
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
