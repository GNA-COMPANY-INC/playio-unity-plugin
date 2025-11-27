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
            jvalue[] val;
            if (dictionary != null)
            {
                foreach (var entry in dictionary)
                {
                    object javaValue = ConvertToJavaObject(entry.Value);
                    val = AndroidJNIHelper.CreateJNIArgArray(new object[] { entry.Key, javaValue });
                    AndroidJNI.CallObjectMethod(map.GetRawObject(), putMethod, val);
                    AndroidJNI.DeleteLocalRef(val[0].l);
                    AndroidJNI.DeleteLocalRef(val[1].l);
                }
            }

            return map;
        }

        private static object ConvertToJavaObject(object value)
        {
            if (value == null)
            {
                return null;
            }

            // Handle primitive types
            if (value is string || value is bool || value is int || value is long || 
                value is float || value is double)
            {
                return value;
            }

            // Convert other numeric types to appropriate Java types
            if (value is byte || value is sbyte || value is short || value is ushort)
            {
                return Convert.ToInt32(value);
            }

            if (value is uint || value is ulong)
            {
                return Convert.ToInt64(value);
            }

            // For unsupported types, convert to string
            PlayioLogger.LogWarning($"Unsupported type {value.GetType()} in event parameters, converting to string");
            return value.ToString();
        }
    }
}