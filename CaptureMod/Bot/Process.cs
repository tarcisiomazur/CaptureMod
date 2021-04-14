using System;
using System.Collections.Generic;
using System.Linq;
using CaptureMod.Connection;
using CaptureMod.Models;
using CaptureMod.Path;

namespace CaptureMod.Bot
{

    public static class Process
    {
        private static GameState _gameState = GameState.UNKNOWN;
        private static AmongUsData _amongUsData;
        private static EventArgs NeedSend;
        private static LobbyEventArgs _lobbyArgs;
        public static EventHandler Send;
        
        public static GameState GameState
        {
            get => _gameState;
            set
            {
                if (value == _gameState) return;
                _gameState = value;
                Send?.Invoke(null, new GameStateEventArgs{NewState = _gameState});
            }
        }

        static Process()
        {
            GameState = GameState.MENU;
            ClientSocket.Instance.OnLogin += (_, __) => SendRefresh();
            ClientSocket.Instance.OnRefresh += (_, __) => SendRefresh();
        }
        
        private static void SendRefresh()
        {
            Send?.Invoke(null, new AmongUsData()
            {
                phase = GameState,
                region = _lobbyArgs.region,
                room = _lobbyArgs.room,
                playerData = PlayerInfo.All.Select(pi => new PlayerData(pi)).ToList()
            });
        }

        private static void SendChangePlayer(PlayerAction action, PlayerInfo pi, bool exiled = false)
        {
            if (pi.Name == "")
                return;
            Send?.Invoke(null, new PlayerChangedEventArgs
            {
                Action = action,
                Disconnected = pi.IsDisconnected,
                Color = (PlayerColor) pi.Color,
                Name = pi.Name,
                IsDead = pi.IsDead || pi.IsDisconnected || exiled,
            });
        }

        private static bool TryProcessEnd()
        {
            var impostors = 0;
            var crewmates=0;
            foreach (var pi in PlayerInfo.All)
            {
                if (pi.IsDead) continue;
                if (pi.IsImpostor)
                    impostors++;
                else
                    crewmates++;
            }

            if (impostors > 0 && impostors < crewmates) return false;
            
            GameState = GameState.ENDGAME;
            return true;
        }

        public static void SetDeath(PlayerInfo player)
        {
            if (TryProcessEnd()) return;
            SendChangePlayer(PlayerAction.Died, player);
        }
        
        public static void Eject(PlayerInfo player)
        {
            if (TryProcessEnd()) return;
            SendChangePlayer(PlayerAction.Exiled, player);
        }

        public static void EndMeeting()
        {
            if (GameState != GameState.ENDGAME) 
                GameState = GameState.TASKS;
        }

        public static void OnMeeting()
        {
            GameState = GameState.DISCUSSION;
        }

        public static void ChangeColor(PlayerInfo pi, int color)
        {
            if (GameState == GameState.LOBBY)
                SendChangePlayer(PlayerAction.ChangedColor, pi);
        }

        public static void Lobby(IEnumerable<PlayerInfo> playerInfos)
        {
            var str = new int[12];
            var pis = playerInfos.ToList();
            if (pis.Any(pi => pi.Color < 0 || ++str[pi.Color] > 1))
            {
                MOD.RunLater(()=>Lobby(pis), 2000);
                return;
            }
            _gameState = GameState.LOBBY;
            _lobbyArgs.playerData = pis.Select(pi => new PlayerData(pi)).ToList();
            Send?.Invoke(null, _lobbyArgs);
        }
        
        public static void Lobby(string code, PlayRegion region)
        {
            _lobbyArgs = new LobbyEventArgs
            {
                room = code,
                region = region,
            };
        }
        
        public static void Menu()
        {
            GameState = GameState.MENU;
        }

        public static void PlayerJoin(PlayerInfo pi)
        {
            SendChangePlayer(PlayerAction.Joined, pi);
        }

        public static void Disconnect(PlayerInfo pi, string reason)
        {
            SendChangePlayer(PlayerAction.Disconnected, pi);
        }
    }
}