using System;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace CaptureMod.Runtime
{
    public class RuntimeProvider
    {
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr il2cpp_resolve_icall([MarshalAs(UnmanagedType.LPStr)] string name);

        public delegate IntPtr d_FindObjectsOfTypeAll(IntPtr type);

        private static d_FindObjectsOfTypeAll _find;

        static RuntimeProvider()
        {
            var ptr = il2cpp_resolve_icall("UnityEngine.ResourcesAPIInternal::FindObjectsOfTypeAll");
            _find = (d_FindObjectsOfTypeAll) Marshal.GetDelegateForFunctionPointer(ptr, typeof(d_FindObjectsOfTypeAll));
        }
        
        public static Il2CppReferenceArray<T> FindObjectsOfTypeAll<T>() where T : Il2CppObjectBase
        {
            return new Il2CppReferenceArray<T>(_find.Invoke(Il2CppType.Of<T>().Pointer));
        }

        public static GameObject GetGameObjectAsset(string name)
        {
            var objs = FindObjectsOfTypeAll<GameObject>();
            foreach (var gameObject in objs)
            {
                if (gameObject.name == name)
                    return gameObject;
            }
            return new GameObject(name);
        }
    }
}