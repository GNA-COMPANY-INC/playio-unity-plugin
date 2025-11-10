using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayioSDK
{
    public class DictionaryUtil
    {
        // https://github.com/AppsFlyerSDK/appsflyer-unity-plugin/blob/a2bbbf0eee56f656b7acb1c3904c1245342f17f3/Assets/AppsFlyer/AppsFlyerAndroid.cs#L666
        internal static AndroidJavaObject toJavaMap(Dictionary<string, string> dictionary)
        {
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            IntPtr putMethod = AndroidJNIHelper.GetMethodID(map.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
            jvalue[] val;
            if (dictionary != null)
            {
                foreach (var entry in dictionary)
                {
                    val = AndroidJNIHelper.CreateJNIArgArray(new object[] { entry.Key, entry.Value });
                    AndroidJNI.CallObjectMethod(map.GetRawObject(), putMethod, val);
                    AndroidJNI.DeleteLocalRef(val[0].l);
                    AndroidJNI.DeleteLocalRef(val[1].l);
                }
            }

            return map;
        }
    }
}