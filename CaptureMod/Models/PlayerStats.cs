using System;
using System.Collections.Generic;

namespace CaptureMod.Models
{
    public class EndPlayerStatsEventArgs : EventArgs
    {
        public long MatchID { get; set; }
        public List<PlayerStats> EndPlayerStats { get; set; }
        public List<Murder> Murders { get; set; }
        public List<Meeting> Meetings { get; set; }
    }
    
    public class Murder
    {
        public static List<Murder> Murders = new List<Murder>();
        
        public Murder(string killer, string victim, int local)
        {
            Killer = killer;
            Victim = victim;
            Local = local;
        }

        public string Killer { get; set; }
        public string Victim { get; set; }
        public int Local { get; set; }

        public override string ToString()
        {
            return $"Killer: {Killer},\nVictim: {Victim},\nLocal: {MOD.GetSystemName(Local)}";
        }
    }

    public class Vote
    {
        public Vote(string player, string suspect)
        {
            Player = player;
            Suspect = suspect;
            if (Meeting.Meetings.Count > 0)
            {
                Meeting.Meetings[Meeting.Meetings.Count-1].Votes.Add(this);
            }
        }

        public string Player { get; set; }
        public string Suspect { get; set; }
        
        
        public override string ToString()
        {
            return $"Player: {Player},\nSuspect: {Suspect}";
        }
    }

    public class Meeting
    {
        public static List<Meeting> Meetings = new List<Meeting>();
        
        public Meeting(string reporter, string body, int local)
        {
            Reporter = reporter;
            Body = body;
            Votes = new List<Vote>();
            Local = local;
            Ejected = "";
            if (Meetings.Count > 0 && Meetings[Meetings.Count-1].Votes.Count == 0)
            {
                Meetings.RemoveAt(Meetings.Count-1);
            }
            Meetings.Add(this);
        }

        public string Reporter { get; set; }
        public string Body { get; set; }
        public List<Vote> Votes { get; set; }
        public int Local { get; set; }
        public string Ejected { get; set; }

        public override string ToString()
        {
            return $"Reporter: {Reporter},\nBody: {Body},\nVotes: [{string.Join(",", Votes)}],\nLocal: {MOD.GetSystemName(Local)},\nEjected: {Ejected}";
        }

        public static void SetEjected(string ejected)
        {
            if (Meetings.Count == 0) return;
            Meetings[Meetings.Count - 1].Ejected = ejected;
        }
    }
    
    public class PlayerStats
    {
        public static Dictionary<string, PlayerStats> Stats = new Dictionary<string, PlayerStats>();
        public PlayerStats(string name, bool isWinner, bool isImpostor, bool isDeath, int color, int tasks, int vent)
        {
            Name = name;
            IsWinner = isWinner;
            IsImpostor = isImpostor;
            IsDeath = isDeath;
            Color = color;
            Tasks = tasks;
            Vent = vent;
        }

        public string Name { get; set; }
        public bool IsWinner { get; set; }
        public bool IsImpostor { get; set; }
        public bool IsDeath { get; set; }
        public int Color { get; set; }
        public int Tasks { get; set; }
        public int Vent { get; set; }

        public override string ToString()
        {
            return
                $"Name: {Name},\nIsWinner: {IsWinner},\nIsImpostor: {IsImpostor},\nIsDeath: {IsDeath},\n" +
                $"Color: {Color},\n Tasks: {Tasks},\nVent: {Vent}";
        }
    }
}