using System.Linq;
using HarmonyLib;
using Hazel;
using CaptureMod.Bot;
using CaptureMod.Models;
using UnhollowerBaseLib;

namespace CaptureMod.Path
{
    using State = MeetingHud.MANCENPNMAC;
    
    [HarmonyPatch]
    public class MeetingHudPath
    {
        private static readonly bool[] DidVote = new bool[255];
        private static State _lastState = State.Results;
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.HandleRpc))]
        public static void HandleRpc(byte ONIABIILFGF, MessageReader JIGFBHFFNFI)
        {
            var rpc = (RPC) ONIABIILFGF;
            MOD.log.LogMessage(rpc + ": " + string.Join(",", MOD.Read(JIGFBHFFNFI)));
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static void Update(MeetingHud __instance)
        {
            if(!_lastState.Equals(__instance.FOIGOPKABAA))
            {
                _lastState = __instance.FOIGOPKABAA;
                switch (_lastState)
                {
                    case State.Discussion:
                    case State.NotVoted:
                    case State.Voted:
                        Process.OnMeeting();
                        break;
                    default:
                        Process.EndMeeting();
                        break;
                }

            }

            foreach (var pva in __instance.GBKFCOAKLAH)
            {
                var player = pva.GEIOMAPOPKA;
                if (pva.didVote)
                {
                    if (DidVote[player] || pva.votedFor == 14) continue;
                    DidVote[player] = true;
                    if (pva.votedFor > -1)
                    {
                        Stats.AddVote(player, pva.votedFor);
                    }
                    else if(!_lastState.Equals(State.Results))
                    {
                        Stats.AddSkip(player);
                    }
                }
                else
                {
                    DidVote[player] = false;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BBFDNCCEJHI))]
        public static void VotingComplete(
            Il2CppStructArray<byte> COMOIMMLKHF,
            GameData.LGBOMGHJELL EAFLJGMBLCH,
            bool EMBDDLIPBME)
        {
            if (EAFLJGMBLCH != null)
            {
                Process.Eject(EAFLJGMBLCH);
                Stats.Eject(EAFLJGMBLCH);
            }
            else
            {
                MOD.log.LogMessage("Skipado");
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MFEGHOFFKKA), nameof(MFEGHOFFKKA.OMHNAMNPJCP))]
        public static void EndGame(AMGMAKBHCMN AMGCOJNIHLM)
        {
            if (MFEGHOFFKKA.BPDANAHEJDD.Count <= 0) return;
            GameData.Instance.AllPlayers.ToArray().Do(pi => Stats.ProcessEnd(pi.Get()));
            Stats.Winners(MFEGHOFFKKA.BPDANAHEJDD.ToArray().Select(win => win.NNMPJKHJLMB));
            Stats.SendPlayerStats();
        }

    }
    
    
}