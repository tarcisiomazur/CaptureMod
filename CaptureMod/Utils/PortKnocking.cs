using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CaptureMod.Utils
{
    struct Knock
    {
        public ushort port;
        public int delay;
        public byte[] payload;
    }
    
    public class PortKnocking
    {
        private Socket socket;
        private readonly List<Knock> knocks = new List<Knock>();

        public Task KnockAll(Uri ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Task task = null;
            if (IPAddress.TryParse(ip.Host, out var address))
            {
                task = new Task(() => Run(address));
            }
            else
            {
                var res = Dns.GetHostAddresses(ip.Host);
                res.Aggregate("", (s, ipAddress) => $"{s};{ipAddress}");
                if(res.Length>0)
                    task = new Task(() => Run(res[0]));
            }
            task?.Start();
            return task;
        }

        private void Run(Object address)
        {
            foreach (var knock in knocks)
            {
                try
                {
                    $"Send {knock.payload} To {address}:{knock.port}".Log();
                    switch (address)
                    {
                        case IPAddress ipAddress:
                            socket.SendTo(knock.payload,new IPEndPoint(ipAddress, knock.port));
                            break;
                        case string host:
                            socket.SendTo(knock.payload ,new DnsEndPoint(host, knock.port));
                            break;
                    }
                    Thread.Sleep(knock.delay);
                }
                catch (Exception ex)
                {
                    ex.Log();
                }
            }
        }

        public void AddPort(ushort port, int delay, string payload = "PortKnocking")
        {
            knocks.Add(new Knock{port = port, delay = delay, payload = System.Text.Encoding.UTF8.GetBytes(payload)});
        }

    }
}