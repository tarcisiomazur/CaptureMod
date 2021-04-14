using System.Linq;
using CaptureMod.Models;
using CaptureMod.Utils;
using HarmonyLib;
using Hazel;
using InnerNet;
using CaptureMod.Bot;
using CaptureMod.Connection;
using CaptureMod.Interface;
using UnhollowerBaseLib;

namespace CaptureMod.Path
{
    using InnerPlayerControl = PlayerControl;
    using InnerPlayerInfo = GameData.LGBOMGHJELL;

    [HarmonyPatch]
    public static class PlayerControlPath
    {
        public static PlayerControl GetPlayerControl(int id)
        {
            try
            {
                return PlayerControl.AllPlayerControls.ToArray().First(pc => pc.PlayerId == id);
            }
            catch
            {
                return null;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InnerPlayerControl),
            nameof(InnerPlayerControl.CoStartMeeting))] // CmdReportDeadBody
        public static void StartMeeting(InnerPlayerControl __instance, InnerPlayerInfo DGDGDKCCKHJ)
        {
            var pos = MOD.GetSystem(__instance.GetTruePosition());
            Stats.Report(__instance, DGDGDKCCKHJ, pos);
        }

        [HarmonyPatch(typeof(InnerPlayerControl), nameof(InnerPlayerControl.MurderPlayer))]
        public static void Postfix(InnerPlayerControl __instance, InnerPlayerControl DGDGDKCCKHJ)
        {
            var pos = MOD.GetSystem(DGDGDKCCKHJ.GetTruePosition());

            Stats.Murder(__instance, DGDGDKCCKHJ, pos);
            Process.SetDeath(DGDGDKCCKHJ);
        }

        [HarmonyPatch(typeof(InnerPlayerControl), nameof(InnerPlayerControl.HandleRpc))]
        public static void Prefix(byte ONIABIILFGF, MessageReader JIGFBHFFNFI)
        {
            var rpc = (RPC) ONIABIILFGF;
            switch (rpc)
            {
                case RPC.SetInfected:
                    new SetInfected(MOD.Read(JIGFBHFFNFI)).infecteds.ForEach(control =>
                        MOD.log.LogMessage(control.nameText.text + " is Impostor"));
                    break;
                default:
                    //MOD.log.LogMessage(rpc.ToString());
                    break;
            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InnerPlayerControl), nameof(InnerPlayerControl.RpcSetInfected))]
        public static void RpcSetInfected(Il2CppReferenceArray<InnerPlayerInfo> BHNEINNHPIJ)
        {
            MOD.log.LogMessage(BHNEINNHPIJ.Aggregate("RpcSetInfected: ", (s, pi) => $"{s} {pi.Get().Name}"));
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InnerPlayerControl), nameof(InnerPlayerControl.SetColor))]
        public static void SetColor(InnerPlayerControl __instance, int CKNNKEMNHAK)
        {
            Process.ChangeColor(__instance, CKNNKEMNHAK);
            MOD.log.LogMessage(__instance.PPMOEEPBHJO.Get().Name + " change color to: " + (PlayerColor) CKNNKEMNHAK);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.SendRpc))]
        public static void SendRpc(uint BDMKPEPHLAI, byte ONIABIILFGF, SendOption BGOHFPGAOPF)
        {
            MOD.log.LogMessage("SendRPC: " + BDMKPEPHLAI + " " + ONIABIILFGF + " " + BGOHFPGAOPF);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.CJOLFPHJDGL))]
        public static void Prefix(MessageReader FMNNFELNLID)
        {
            var read = MOD.Read(FMNNFELNLID);
            if (read.Length != 14 || read[0] != 11)
            {
                //MOD.log.LogMessage("RPC InnerNetClient: " + string.Join(",", read));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InnerPlayerControl), nameof(InnerPlayerControl.SetName))]
        public static void SetName(InnerPlayerControl __instance, string LFBNFBOLLNN, bool PKGCJHDPPAF)
        {
            if (PlayerControl.LocalPlayer == null || ((PlayerInfo) __instance).Name == "") return;
            if (PlayerControl.LocalPlayer == __instance)
                MOD.RunLater(() => Process.Lobby(PlayerInfo.All), 2000);
            else
                Process.PlayerJoin(__instance);
            UIVoIP.CreateVoipMark();
            VoIPConnection.SetArgs(new ConnectEventArgs
            {
                GameCode = Singleton.GameStartManager.GameRoomName.text.Split('\n')[1],
                GameName = PlayerControl.LocalPlayer.nameText.name,
                ID = PlayerControl.LocalPlayer.PlayerId
            });
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.StartGame))]
        public static void Prefix()
        {
            MOD.log.LogMessage("GameStarted Joined: ");
        }

    }
    
}