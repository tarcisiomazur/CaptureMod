using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace CaptureMod.Utils
{
    public static class Config
    {
        public static bool DebugTerminal { get; set; }
        public static string AutoUpdatePath { get; set; }
        public static string ServerHostAddress { get; set; }
        public static ushort ServerHostPort { get; set; }

        public static StreamReader KnockFile =>
            TryOpenRead($"{Directory.GetCurrentDirectory()}\\BepInEx\\plugins\\CaptureMod\\.knock");

        public static string AutoUpdateAddress =>
            $"{ServerHostAddress}:{ServerHostPort}/{AutoUpdatePath}";
        public static string VersionInfoAddress =>
            $"{ServerHostAddress}:{ServerHostPort}";

        

        static Config()
        {
            try
            {
                LoadConfigs();
            }
            catch
            {
                //ignore
            }
        }

        static void LoadConfigs()
        {
            foreach (var rawLine in File.ReadAllLines($"{Directory.GetCurrentDirectory()}\\BepInEx\\config\\CaptureMod.cfg"))
            {
                var line = rawLine.Trim();

                if (line.StartsWith("#"))
                    continue;

                var split = line.Split(new[] { '=' }, 2);
                if (split.Length != 2)
                    continue;

                var currentKey = split[0].Trim();
                var currentValue = split[1].Trim();
                typeof(Config).GetProperties().DoIf(info => info.Name.Equals(currentKey), info =>
                {
                    $"{currentKey} = {currentValue}".Log();
                    var converter = TypeDescriptor.GetConverter(info.PropertyType);
                    info.SetValue(null, converter.ConvertFromString(currentValue));
                });
            }
        }


        private static StreamReader TryOpenRead(string fileName)
        {
            try
            {
                return File.OpenText(fileName);
            }
            catch
            {
                return new StreamReader(string.Empty);
            }
        }
    }
}