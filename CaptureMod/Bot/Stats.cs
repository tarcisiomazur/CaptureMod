using System;
using System.Collections.Generic;
using System.Linq;
using CaptureMod.Models;
using CaptureMod.Utils;
using CaptureMod.Connection;
using CaptureMod.Path;

namespace CaptureMod.Bot
{
    using SystemTypes = BCPJLGGNHBC;

    public static class Stats
    {
        private static readonly int[] Vents = new int[10];
        public static EventHandler Send;
        private static int _gameID;

        static Stats()
        {
            ClientSocket.Instance.OnMatchID += (_, id) => _gameID = id;
        }

        public static void Eject(PlayerInfo player)
        {
            if (Meeting.Meetings.Count > 0)
            {
                Meeting.SetEjected(player.Name);
            }
        }

        public static void Report(PlayerInfo pi, PlayerInfo pi2, SystemTypes local)
        {
            if(pi2.IsNull)
                new Meeting(pi.Name, null, (int) local - 1).Log();
            else
                new Meeting(pi.Name, pi2.Name, (int) local - 1).Log();
        }

        public static void Murder(PlayerInfo pi, PlayerInfo pi1, SystemTypes local)
        {
            var murder = new Murder(pi.Name, pi1.Name, (int) local - 1);
            Models.Murder.Murders.Add(murder);
            murder.Log();
        }

        public static void AddVentIn(PlayerInfo pi, SystemTypes local)
        {
            Vents[pi.ID]++;
            MOD.log.LogMessage(pi.Name + " vent in " + local);
        }

        public static void AddVentOut(PlayerInfo pi, SystemTypes local)
        {
            MOD.log.LogMessage(pi.Name + " vent out " + local);
        }

        public static void ProcessEnd(PlayerInfo pi)
        {
            if (!PlayerStats.Stats.ContainsKey(pi.Name))
            {
                var tasks = pi.Tasks.Count(task => task.LBBFBHJINJK);
                PlayerStats.Stats.Add(pi.Name, new PlayerStats(pi.Name, false, pi.IsImpostor,
                    pi.IsDead, pi.Color, tasks, Vents[pi.ID]));
            }
        }

        public static void Clear()
        {
            for (var i = 0; i < Vents.Length; i++)
                Vents[i] = 0;
            _gameID = 0;
            "ClearGameID".Log();
            PlayerStats.Stats.Clear();
            Models.Murder.Murders.Clear();
            Meeting.Meetings.Clear();
        }

        public static void Winners(IEnumerable<string> select)
        {
            foreach (var s in select)
            {
                PlayerStats.Stats[s].IsWinner = true;
            }
        }

        private static void Print()
        {
            MOD.log.LogMessage(MOD.Serialize(new EndPlayerStatsEventArgs()
            {
                Meetings = Meeting.Meetings,
                Murders = Models.Murder.Murders,
                EndPlayerStats = PlayerStats.Stats.Values.ToList(),
            }));
        }

        public static void SendPlayerStats()
        {
            Send?.Invoke(null, new EndPlayerStatsEventArgs
            {
                MatchID = _gameID,
                Meetings = Meeting.Meetings,
                Murders = Models.Murder.Murders,
                EndPlayerStats = PlayerStats.Stats.Values.ToList(),
            });
        }

        public static void AddVote(PlayerInfo pi, PlayerInfo pi2)
        {
            MOD.log.LogMessage(pi.Name + " vote in " + pi2.Name);
            new Vote(pi.Name, pi2.Name);
        }

        public static void AddSkip(PlayerInfo pi)
        {
            MOD.log.LogMessage(pi.Name + " skip");
            new Vote(pi.Name, null);
        }
    }
}