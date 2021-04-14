using System;
using System.Collections.Generic;
using HarmonyLib;
using TMPro;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public static class TextMeshProPath
    {
        private static readonly Dictionary<IntPtr, Action<TextMeshPro>> OnStartDict =
            new Dictionary<IntPtr, Action<TextMeshPro>>();
        
        [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.OnDestroy))]
        public static void Cancel(TextMeshPro __instance)
        {
            if(OnStartDict.ContainsKey(__instance.m_CachedPtr))
                OnStartDict.Remove(__instance.m_CachedPtr);
        }
        
        public static void SetOnStart(this TextMeshPro textMeshPro, Action<TextMeshPro> action)
        {
            if (OnStartDict.ContainsKey(textMeshPro.m_CachedPtr))
                OnStartDict[textMeshPro.m_CachedPtr] = action;
            else
                OnStartDict.Add(textMeshPro.m_CachedPtr, action);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TextMeshPro), nameof(TextMeshPro.OnEnable))]
        public static void OnStart(TextMeshPro __instance)
        {
            if (__instance != null && OnStartDict.TryGetValue(__instance.m_CachedPtr, out var action))
            {
                OnStartDict.Remove(__instance.m_CachedPtr);
                MOD.RunLater(() => action?.Invoke(__instance));
            }
        }
    }
}