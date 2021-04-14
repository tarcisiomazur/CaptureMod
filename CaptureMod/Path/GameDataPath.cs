using CaptureMod.Utils;
using HarmonyLib;
using Hazel;
using CaptureMod.Bot;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public class GameDataPath
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(GABADEGMIHF))]
        public static void HandleDisconnect(PlayerControl ABEKPAKNMJH, GABADEGMIHF AMGCOJNIHLM)
        {
            Process.Disconnect(ABEKPAKNMJH, AMGCOJNIHLM.ToString());
            MOD.log.LogMessage("HandleDisconnect " + ABEKPAKNMJH.nameText.text + " reason: " + AMGCOJNIHLM);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), nameof(GameData.AddPlayer))]
        public static void PlayerJoin(PlayerControl MEGIPHPKMMB)
        {
            "PlayerJoin".Log();
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.HandleRpc))]
        public static void HandleRpc(byte ONIABIILFGF, MessageReader JIGFBHFFNFI)
        {
            MOD.log.LogMessage("ShipStatus HandleRpc: " + string.Join(",",MOD.Read(JIGFBHFFNFI)));
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcRepairSystem))]
        public static void RpcRepairSystem(BCPJLGGNHBC JJANOFHFJFO, int BHGHLAILNMC)
        {
            MOD.log.LogMessage("RpcRepairSystem: " + JJANOFHFJFO + " " + BHGHLAILNMC);
        }
        
    }
}