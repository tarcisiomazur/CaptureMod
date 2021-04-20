using System;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using CaptureMod.Utils;
using CaptureMod.Bot;
using CaptureMod.Interface;
using CaptureMod.Path.LobbyOptionsAPI;
using SocketIOClient;

namespace CaptureMod.Connection
{ 
    public class ClientSocket
    {
        private const int DiscordUserIdLength = 18;
        public static ClientSocket Instance { get; }
        private SocketIO socket;
        private PortKnocking portKnocking;

        public event EventHandler<string> OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler<string> OnLogout;
        public event EventHandler<string> OnLogin;
        public event EventHandler OnRefresh;
        public event EventHandler<int> OnMatchID;
        
        public string ConnectCode { get; set; }
        
        static ClientSocket()
        {
            Instance = new ClientSocket();
            Instance.Init();
            Process.Send += SendAsync;
            Stats.Send += SendAsync;
            MyOptions.CustomGameOptions.Bot.OnChange += ChangeConnection;
        }

        private static void ChangeConnection(object sender, OptionBehaviour e)
        {
            var option = sender as CustomToggleOption;
            if (option?.Value ?? false)
                Instance.ConnectAsync();
            else
                Instance.Close();
        }

        private void Init()
        {
            BuildAuth();
            socket = new SocketIO();
            socket.OnError += (sender, s) => { MOD.log.LogMessage($"Error {s}"); };
            socket.OnReconnecting += (sender, i) => { MOD.log.LogMessage($"Reconnecting {i}"); };
            socket.OnReconnectFailed += (sender, exception) => exception.Log(); 
            socket.OnConnected += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(ConnectCode))
                {
                    Login(ConnectCode);
                }
                OnConnected?.Invoke(this, socket.ServerUri.ToString());
            };

            // Handle socket disconnection events.
            socket.OnDisconnected += (sender, e) =>
            {
                MOD.log.LogMessage("Disconnected" + e);
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            };
            
            socket.OnReceivedEvent += (sender, args) =>
            {
                $"Event {args.Event} received".Log();
            };

            socket.On("refresh", response => OnRefresh?.Invoke(null, null));

            socket.On("error", response =>
            {
                if (response.Count == 0) return;
                MOD.log.LogMessage($@"Error Received: {response}");
            });

            socket.On("register", response =>
            {
                MOD.log.LogMessage($@"Register Received: {response}");
                if (response.Count != 1)
                {
                    return;
                }
                var id = response.GetValue().ToString();
                if (id.Length != 18) return;
                OnLogin?.Invoke(this, id);
            });

            socket.On("matchID", response =>
            {
                MOD.log.LogMessage("MatchID Received: " + response);
                var matchID = response.GetValue().ToString();
                int.TryParse(matchID, out var id);
                OnMatchID?.Invoke(null, id);
            });

            socket.On("unregister", response =>
            {
                MOD.log.LogMessage("Unregister Received: " + response);
                var motive = "";
                if (response.Count == 1)
                {
                    motive = "Motive: " + response.GetValue();
                }

                OnLogout?.Invoke(this, motive);
            });

        }

        private void BuildAuth()
        {
            portKnocking = new PortKnocking();
            var file = File.OpenText($"{Directory.GetCurrentDirectory()}\\BepInEx\\plugins\\CaptureMod\\.knock");
            while (!file.EndOfStream)
            {
                var args = file.ReadLine()?.Split(" ".ToCharArray(),3, StringSplitOptions.RemoveEmptyEntries);
                if(args != null && args.Length == 3 && ushort.TryParse(args[0], out var port) && int.TryParse(args[1], out var delay)){
                    portKnocking.AddPort(port, delay, args?[2]);
                }
            }
        }

        private void Close()
        {
            socket.DisconnectAsync().Wait();
        }

        private void LoginWithDiscord(string discordID)
        {
            socket.EmitAsync("discordID", discordID).ContinueWith(task =>
            {
                if (!task.IsCompleted)
                    OnLogout?.Invoke(this, "LoginFail");
                else
                    "OK".Log();
            });
        }

        private void LoginWithConnectCode(string connectCode)
        {
            socket.EmitAsync("connectCode", connectCode).ContinueWith(task =>
            {
                if(!task.IsCompleted)
                    OnLogout?.Invoke(this, "LoginFail");
                else
                    "OK".Log();
            });
        }

        public Task ConnectAsync()
        {
            try
            {
                var newUri = new Uri(UIBotSettings.URL);
                if (newUri.AbsoluteUri != socket.ServerUri?.AbsoluteUri)
                {
                    if (socket.Connected)
                        socket.DisconnectAsync().Wait();
                    socket.ServerUri = newUri;
                }else if (socket.Connected)
                {
                    "isconnected".Log();
                    return null;
                }
                var knocking = portKnocking.KnockAll(newUri);
                knocking?.ContinueWith(task1 => socket.ConnectAsync());
                return knocking;
            }
            catch (Exception ex)
            {
                ex.Log();
            }

            return null;
        }

        private static void SendAsync(object sender, EventArgs e)
        {
            MOD.log.LogMessage(MOD.Serialize(e));
            if (Instance == null || !Instance.socket.Connected) return;
            Instance.socket.EmitAsync(e.getName(), MOD.Serialize(e));
        } 

        public void Connect(string code)
        {
            ConnectCode = code; 
            ConnectAsync();
        }
        
        private void Login(string code)
        {
            switch (code?.Length)
            {
                case DiscordUserIdLength:
                    LoginWithDiscord(code);
                    break;
                case 8:
                    LoginWithConnectCode(code);
                    break;
            }
        }

        public static Uri BuildUri(string value)
        {
            try
            {
                if (value.Contains("localhost:"))
                    value = value.Replace("localhost", "127.0.0.1");
                if (value.Contains("https://"))
                    return new Uri(value.Replace("https://", "http://"));
                if (!value.Contains("http://"))
                    return new Uri("http://" + value);
                return new Uri(value);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }

}