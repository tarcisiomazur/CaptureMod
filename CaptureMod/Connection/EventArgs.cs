using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using CaptureMod.Models;

namespace CaptureMod.Connection
{
    public static class EventName
    {
        public static string getName(this EventArgs ev)
        {
            return ev switch
            {
                GameStateEventArgs _ => "state",
                PlayerChangedEventArgs _ => "player",
                LobbyEventArgs _ => "lobby",
                AmongUsData _ => "refresh",
                EndPlayerStatsEventArgs _ => "endplayerstats",
                _ => "null"
            };
        }
    }

    public enum GameState
    {
        LOBBY,
        TASKS,
        DISCUSSION,
        MENU,
        ENDGAME,
        UNKNOWN
    }

    public class Response : EventArgs
    {
        public Response(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
    
    public class GameStateEventArgs : EventArgs
    {
        public GameState NewState { get; set; }
    }

    public enum PlayRegion
    {
        NorthAmerica = 0,
        Asia = 1,
        Europe = 2
    }
    public enum PlayerAction
    {
        Joined,
        Left,
        Died,
        ChangedColor,
        ForceUpdated,
        Disconnected,
        Exiled
    }
    public class PlayerChangedEventArgs : EventArgs
    {
        public PlayerAction? Action { get; set; }
        public string Name { get; set; }
        public bool IsDead { get; set; }
        public bool Disconnected { get; set; }
        public PlayerColor Color { get; set; }
    }

    public class LobbyEventArgs : EventArgs
    {
        public string room { get; set; }
        public PlayRegion region { get; set; }
        public List<PlayerData> playerData { get; set; }
    }

    public class AmongUsData : EventArgs
    {
        public GameState phase { get; set; }
        public string room { get; set; }
        public PlayRegion region { get; set; }
        public List<PlayerData> playerData { get; set; }
    }
    
    public class ConnectEventArgs: EventArgs
    {
        public string GameCode { get; set; }
        public string GameName { get; set; }
        public int ID { get; set; }
    }
}