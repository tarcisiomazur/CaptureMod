using System;
using CaptureMod.Utils;
using HarmonyLib;
using InnerNet;
using CaptureMod.Bot;
using CaptureMod.Connection;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public class AuClient
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ONICLMLJABJ))]
        public static void JoinGame (string IBNLGMAMNKK, ClientData MLPPNIKICPC)
        {
            Enum.TryParse(Singleton.ServerManager.HMIJGFFKBNN.Name.Replace(" ",""), out PlayRegion region);
            Singleton.ServerManager.HMIJGFFKBNN.Name.Log();
            Process.Lobby(IBNLGMAMNKK, region);
        }
        
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.DDIEDPFFHOG))]
        public static void Prefix(AMGMAKBHCMN NEPMFBMGGLF, bool FBEKDLNKNLL)
        {
            MOD.log.LogMessage("GameEnd by MDDLANONDOD reason: " + NEPMFBMGGLF);
            MOD.log.LogMessage("Stack: " + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ABBMMHOBFJA))]
        public static void Start()
        {
            Stats.Clear();
            MFEGHOFFKKA.BPDANAHEJDD.Clear();
            Process.GameState = GameState.TASKS;
            MOD.log.LogMessage("Clear All Data");
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.ExitGame))]
        public static void ExitGame(GABADEGMIHF AMGCOJNIHLM)
        {
            Process.GameState = GameState.MENU;
            MOD.log.LogMessage("Exit Game: " + AMGCOJNIHLM);
        }
    }
    
}