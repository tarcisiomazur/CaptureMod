using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using Hazel;
using CaptureMod.Connection;
using CaptureMod.Interface;
using CaptureMod.Utils;
using Newtonsoft.Json;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace CaptureMod
{
    using SystemTypes = BCPJLGGNHBC;
    
    [BepInPlugin("org.bepinex.plugins.CaptureMod", "Capture MOD", Version)]
    public class MOD: BasePlugin
    {
        public static BepInEx.Logging.ManualLogSource log;
        private readonly Harmony _harmony;
        public const string Version = "3.0.0";
        
        [DllImport("kernel32")]
        static extern bool AllocConsole();
        
        public MOD()
        {
            log = Log;
            _harmony = new Harmony("MOD");
        }
        
        public override void Load()
        {
            log = Log;
            log.LogMessage("Loading...");
            if (!AutoUpdate.CheckUpdate())
            {
                log.LogError("Not Loaded... old version and not have update");
                return;
            }
            ClassInjector.RegisterTypeInIl2Cpp<MyClass>();
            MyOptions.CustomGameOptions = new MyOptions();
            ClientSocket.BuildUri("");
            _harmony.PatchAll();
            log.LogMessage("Loaded...");
        }


        public static byte[] Read(MessageReader msg)
        {
            var pos = msg.Position;
            var read = msg.ReadBytes(msg.BytesRemaining).ToArray();
            msg.Position = pos;
            return read;
        }

        public static SystemTypes GetSystem(Vector2 point)
        {
            var get = ShipStatus.Instance.LEBGJFABFAA.FirstOrDefault(room => room.roomArea?.OverlapPoint(point)??false);
            return get != default ? get.RoomId : SystemTypes.Hallway;
        }

        public static string Serialize(object obj, bool indented = false)
        {
            var ser = new JsonSerializer {Formatting = indented? Formatting.Indented: Formatting.None};
            TextWriter r = new StringWriter();
            ser.Serialize(r, obj);
            return r.ToString();
        }
        
        public static T Deserialize<T>(string obj, bool indented = false)
        {
            var ser = new JsonSerializer {Formatting = indented? Formatting.Indented: Formatting.None};
            var r = new JsonTextReader(new StringReader(obj));
            return ser.Deserialize<T>(r);
        }

        public static void RunLater(Action act, int delay = 10)
        {
            new Thread(() =>
            {
                Thread.Sleep(delay);
                act.Invoke();
            }).Start();
        }

        public static void Debug(object obj, int level, int line, string caller, string path)
        {
            var msg = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {path.Split('\\').Last()} {caller}:{line} {obj}";
            switch (level)
            {
                case 0:
                    log.LogMessage(msg);
                    break;
                case 1:
                    log.LogWarning(msg);
                    break;
                case 2:
                    log.LogError(msg);
                    break;
                default:
                    log.LogFatal(msg);
                    break;
            }
            
        }


        public static string GetSystemName(int local)
        {
            return local == 0 ? "Button" : ((SystemTypes) (local - 1)).ToString();
        }
    }
    

}