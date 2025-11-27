using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayioSDK
{
    public class DictionaryUtil
    {
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