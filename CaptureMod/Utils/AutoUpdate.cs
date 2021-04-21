using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace CaptureMod.Utils
{
    public struct Info
    {
        public string version{ get; set; }
        public string commit{ get; set; }
        public int totalGuilds{ get; set; }
        public int activeConnections{ get; set; }
        public int activeGames{ get; set; }
        public string captureVersion { get; set; }
    }
    
    public static class AutoUpdate
    {
        public static bool CheckUpdate()
        {
            PortKnocking.AuthServer();
            try
            {
                var client = new WebClient();
                var data = client.DownloadData(Config.VersionInfoAddress);
                var des = MOD.Deserialize<Info>(Encoding.Default.GetString(data));
                if (des.captureVersion == MOD.Version)
                {
                    $"Version {MOD.Version} Checked".Log();
                    return true;
                }

                if (!File.Exists("CaptureAutoUpdate.exe"))
                {
                    "Downloading CaptureAutoUpdate!".Log();
                    client = new WebClient();
                    client.Headers.Add("myIP", GetMyIP());
                    "IpGetted".Log();
                    data = client.DownloadData(Config.AutoUpdateAddress);
                    if (data.Length == 0)
                    {
                        @"Download Error".Log();
                    }

                    File.WriteAllBytes("CaptureAutoUpdate.exe", data);
                }
            }
            catch
            {
                "Download Update Capture Error!".Log();
                return false;
            }
            Process.Start("CaptureAutoUpdate.exe");
            Environment.Exit(0);
            return false;
        }

        private static string GetMyIP()
        {
            try
            {
                "DownloadString".Log();
                return new WebClient().DownloadString("https://ipv4.icanhazip.com/");
            }
            catch
            {
                return "";
            }
        }
    }
}