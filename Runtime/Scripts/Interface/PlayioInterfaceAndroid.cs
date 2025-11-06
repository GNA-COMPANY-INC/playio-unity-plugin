#if UNITY_ANDROID
using System.Collections.Generic;
using UnityEngine;

namespace PlayioSDK
{
    internal class NativeInterface
    {
        private const string JAVA_PACKAGE_NAME = "com.gna.playio_unity_wrapper";
        static AndroidJavaObject wrapper = new AndroidJavaObject(JAVA_PACKAGE_NAME + ".PlayioUnityWrapper");
        internal static void Init(PlayioConfig config)
        {
            if (!Application.isPlaying) { return; }
            AndroidJavaClass logLevelEnumClass = new AndroidJavaClass(
                JAVA_PACKAGE_NAME + ".PlayioConfig$LogLevel"
            );

            AndroidJavaObject javaLogLevel = logLevelEnumClass.GetStatic<AndroidJavaObject>(config.logLevel.ToString());

            AndroidJavaObject javaConfig = new AndroidJavaObject(
                JAVA_PACKAGE_NAME + ".PlayioConfig",
                config.clientId,
                javaLogLevel
            );
            Invoke("init", javaConfig);
        }

        internal static void SetUserId(string userId)
        {
            if (!Application.isPlaying) { return; }
            Invoke("setUserId", userId);
        }

        internal static void SetUserAttributes(Dictionary<string, string> attributes)
        {
            if (!Application.isPlaying) { return; }
            Invoke("setUserAttributes", attributes);
        }

        internal static void SendEvent(string eventName, Dictionary<string, string> parameters)
        {
            if (!Application.isPlaying) { return; }
            Invoke("sendEvent", eventName, parameters);
        }

        internal static void Start()
        {
            if (!Application.isPlaying) { return; }
            Invoke("start");
        }

        internal static void Stop()
        {
            if (!Application.isPlaying) { return; }
            Invoke("stop");
        }

        internal static bool IsStopped()
        {
            if (!Application.isPlaying) { return false; }
            return wrapper.Call<bool>("isStopped");
        }

        internal static string GetSdkVersion()
        {
            if (!Application.isPlaying) { return ""; }
            return wrapper.Call<string>("getSdkVersion");
        }

        internal static void DisableCollectAdvertisingIdentifier(bool disable)
        {
            if (!Application.isPlaying) { return; }
            Invoke("disableCollectAdvertisingIdentifier", disable);
        }

        private static void Invoke(
            string method,
            params object[] parameters
        )
        {
            if (!Application.isPlaying) { return; }
            // if parameter is Dictionary, convert it to JavaMap
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is System.Collections.Generic.Dictionary<string, string>)
                {
                    parameters[i] = DictionaryUtil.toJavaMap(parameters[i] as System.Collections.Generic.Dictionary<string, string>);
                }
            }
            wrapper.Call(method, parameters);
        }
    }
}
#endif