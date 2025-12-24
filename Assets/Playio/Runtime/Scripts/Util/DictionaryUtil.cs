using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayioSDK
{
    public class DictionaryUtil
    {
        internal static AndroidJavaObject toJavaMap(Dictionary<string, object> dictionary)
        {
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            IntPtr putMethod = AndroidJNIHelper.GetMethodID(map.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");

            if (dictionary == null)
            {
                return map;
            }

            object[] args = new object[2];
            foreach (var kvp in dictionary)
            {
                AndroidJavaObject k = new AndroidJavaObject("java.lang.String", kvp.Key);
                AndroidJavaObject v = ConvertToJavaObject(kvp.Value);

                args[0] = k;
                args[1] = v;

                AndroidJNI.CallObjectMethod(map.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray(args));
            }

            return map;
        }

        private static AndroidJavaObject ConvertToJavaObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int)
            {
                return new AndroidJavaObject("java.lang.Integer", value);
            }
            else if (value is long)
            {
                return new AndroidJavaObject("java.lang.Long", value);
            }
            else if (value is double)
            {
                return new AndroidJavaObject("java.lang.Double", value);
            }
            else if (value is float)
            {
                return new AndroidJavaObject("java.lang.Float", value);
            }
            else if (value is bool)
            {
                return new AndroidJavaObject("java.lang.Boolean", value);
            }
            else if (value is string)
            {
                return new AndroidJavaObject("java.lang.String", value);
            }
            else
            {
                PlayioLogger.LogWarning($"Unsupported type {value.GetType()} in event parameters, converting to string");
                return new AndroidJavaObject("java.lang.String", value.ToString());
            }
        }
    }
}