using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CaptureMod.Utils
{
    public static class Extended
    {
        public static T[] SubArray<T>(this T[] array, int startIndex, int length)
        {
            int length1;
            if (array == null || (length1 = array.Length) == 0)
                return new T[0];
            if (startIndex < 0 || length <= 0 || startIndex + length > length1)
                return new T[0];
            if (startIndex == 0 && length == length1)
                return array;
            var objArray = new T[length];
            Array.Copy((Array) array, startIndex, (Array) objArray, 0, length);
            return objArray;
        }
        public static void SetPos(this Transform c, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            var pos = c.localPosition;
            if (!float.IsNaN(x)) pos.x = x;
            if (!float.IsNaN(y)) pos.y = y;
            if (!float.IsNaN(z)) pos.z = z;
            c.localPosition = pos;
        }
        
        public static void Move(this Transform c, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            var pos = c.localPosition;
            if (!float.IsNaN(x)) pos.x += x;
            if (!float.IsNaN(y)) pos.y += y;
            if (!float.IsNaN(z)) pos.z += z;
            c.localPosition = pos;
        }
        
        public static void SetScale(this Transform c, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            var pos = c.localScale;
            if (!float.IsNaN(x)) pos.x = x;
            if (!float.IsNaN(y)) pos.y = y;
            if (!float.IsNaN(z)) pos.z = z;
            c.localScale = pos;
        }
        public static void AddScale(this Transform c, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            var pos = c.localScale;
            if (!float.IsNaN(x)) pos.x += x;
            if (!float.IsNaN(y)) pos.y += y;
            if (!float.IsNaN(z)) pos.z += z;
            c.localScale = pos;
        }
        
        private static float Pitagoras(float a, float b)
        {
            return (float) Math.Sqrt(a * a + b * b);
        }
        
        public static float Distance(this Vector2 c, Vector2 obj)
        {
            return Pitagoras(c.x-obj.x, c.y-obj.y);
        }
        public static Vector2 Add(this Vector2 c, float x = 0, float y = 0)
        {
            return new Vector2(c.x + x, c.y + y);
        }

        public static Vector2 Limit(this Vector2 c, float x, float y)
        {
            return new Vector2(c.x > x ? x : c.x, c.y > y ? y : c.y);
        }
        
        public static void Log(this object obj,int level = 0,[CallerLineNumber] int line=0,
            [CallerMemberName] string caller="",
            [CallerFilePath] string path = "")
        {
            MOD.Debug(obj, level, line,caller,path);
        }

        public static Transform GetChild(this Transform go, string path) =>
            path.Split('/').Aggregate(go, (current, s) => current.FindChild(s));
        
    }
}