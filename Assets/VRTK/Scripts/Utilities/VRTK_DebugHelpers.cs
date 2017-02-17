namespace VRTK
{
    using UnityEngine;
    using System;

    public static class VRTK_DebugHelpers
    {
        public static string[] ToStringArray(object[] arr)
        {
            return Array.ConvertAll<object, string>(
                arr,
                (object obj) => obj.ToString());
        }

        public static string ObjectDebugString(string name, string[] properties)
        {
            return name + ": (" + string.Join(", ", properties) + ")";
        }

        public static string PropertyDebugString(string name, object value)
        {
            return name + ":" + (value == null ? "null" : value);
        }

        public static string ArrayPropertyDebugString(string name, object[] values)
        {
            if (values == null)
            {
                return PropertyDebugString(name, "null");
            }
            else
            {
                string arrayContents = string.Join(", ", ToStringArray(values));
                return PropertyDebugString(name, "[" + arrayContents + "]");
            }
        }
    }
}