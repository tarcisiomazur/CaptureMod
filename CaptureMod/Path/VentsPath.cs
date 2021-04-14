using HarmonyLib;
using CaptureMod.Bot;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public class Vents
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Vent), nameof(Vent.FCHLKLJCBEP))]
        public static void VentEnter(PlayerControl MEGIPHPKMMB)
        {
            var pos = MOD.GetSystem(MEGIPHPKMMB.GetTruePosition());
            Stats.AddVentIn(MEGIPHPKMMB, pos);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Vent), nameof(Vent.AGOIHGCNABM))]
        public static void VentExit(PlayerControl MEGIPHPKMMB)
        {
            var pos = MOD.GetSystem(MEGIPHPKMMB.GetTruePosition());
            Stats.AddVentOut(MEGIPHPKMMB, pos);
        }

    }
}