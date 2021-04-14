using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CaptureMod.Utils;
using CaptureMod.Voice;
using CaptureMod.Interface;
using CaptureMod.Path.LobbyOptionsAPI;
using NetCoreServer;
using CaptureMod.Path;

namespace CaptureMod.Connection
{
    public class AmongClient : WsClient
    {
        public ConnectEventArgs ConnectEventArgs { get; set; }
        
        public AmongClient(string address, int port, ConnectEventArgs connectEventArgs) : base(address, port)
        {
            $"VoIP address: {address}".Log();
            this.ConnectEventArgs = connectEventArgs;
        }

        public void DisconnectAndStop()
        {
            _stop = true;
            CloseAsync(1000);
            while (IsConnected)
                Thread.Yield();
        }

        public override void OnWsConnecting(HttpRequest request)
        {
            request.SetBegin("GET", "/");
            request.SetHeader("Host", "localhost");
            request.SetHeader("Origin", "http://localhost");
            request.SetHeader("Upgrade", "websocket");
            request.SetHeader("Connection", "Upgrade");
            request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
            request.SetHeader("Sec-WebSocket-Protocol", "among, voip");
            request.SetHeader("Sec-WebSocket-Version", "13");
            request.SetHeader("Room",MOD.Serialize(ConnectEventArgs));
            
        }

        public override void OnWsConnected(HttpResponse response)
        {
            MOD.log.LogMessage($"Chat WebSocket client connected a new session with Id {Id}");
        }

        public override void OnWsDisconnected()
        {
            MOD.log.LogMessage($"Chat WebSocket client disconnected a session with Id {Id}");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            VoIP.ReceiveDataAvailable(new VoipMessage(buffer,offset));
        }
        
        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            MOD.log.LogMessage($"Chat TCPclient disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        public override void OnWsError(SocketError error)
        {
            MOD.log.LogMessage($"Chat WebSocket client caught an error with code {error}");
        }
        
        private bool _stop;
    }


    public static class VoIPConnection
    {
        public static AmongClient MyClient { get; private set; }

        static VoIPConnection()
        {
            MyOptions.CustomGameOptions.VoIP.OnChange += ChangeConnection;
        }
        
        public static bool Connected => MyClient?.IsConnected ?? false;

        public static void SetArgs(ConnectEventArgs connectEventArgs)
        {
            try
            {
                var address = Dns.GetHostAddresses(UIBotSettings.IP);
                if (address.Length > 0)
                    MyClient = new AmongClient(address[0].ToString(), 8124, connectEventArgs);
            }
            catch
            {
                // ignore
            }
        }

        private static void ChangeConnection(object sender, OptionBehaviour e)
        {
            var option = sender as CustomToggleOption;
            if (option?.Value ?? false)
                Connect();
            else
                Disconnect();
        }

        private static void Connect()
        {
            MOD.log.LogMessage("Client connecting...");
            MyClient.ConnectAsync();
            MOD.log.LogMessage("Done!");
        }

        public static void Disconnect()
        {
            MyClient.DisconnectAndStop();
        }
        
        public static void Send(float[] buffer, int offset, int count)
        {
            if (PlayerControl.LocalPlayer == null)
            {
                return;
            }
            var msg = new VoipMessage(buffer, offset, count);
            msg.Source = PlayerControl.LocalPlayer.PlayerId;
            var obj = PlayerControl.LocalPlayer.GetTruePosition();
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                if (pc.PlayerInfo().IsDead && pc.GetTruePosition().Distance(obj) < 1)
                    msg.AddListener(pc.PlayerId);
            }
            if (MyClient.IsConnected)
            {
                MyClient.SendTextAsync(msg._byteMessage, 0, msg._byteMessage.Length);
            }
            
        }
        
    }
    
    

}