using HarmonyLib;
using UnhollowerBaseLib;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public class Pets
    {
        [HarmonyPatch(typeof(HatManager), "GetUnlockedPets")]
        public static bool Prefix(ref Il2CppReferenceArray<PetBehaviour> __result)
        {
            __result = (Il2CppReferenceArray<PetBehaviour>) HatManager.CHNDKKBEIDG.AllPets.ToArray();
            return false;
        }
        
        [HarmonyPatch(typeof(HatManager), "GetUnlockedSkins")]
        public static bool Prefix(ref Il2CppReferenceArray<SkinData> __result)
        {
            __result = (Il2CppReferenceArray<SkinData>) HatManager.CHNDKKBEIDG.AllSkins.ToArray();
            return false;
        }
        
        [HarmonyPatch(typeof(HatManager), "GetUnlockedHats")]
        public static bool Prefix(ref Il2CppReferenceArray<HatBehaviour> __result)
        {
            __result = (Il2CppReferenceArray<HatBehaviour>) HatManager.CHNDKKBEIDG.AllHats.ToArray();
            return false;
        }
        
    }


}