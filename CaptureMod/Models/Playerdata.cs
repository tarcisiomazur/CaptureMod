using System;
using CaptureMod.Path;
using Newtonsoft.Json;

namespace CaptureMod.Models
{
    public class PlayerData
    {

        [JsonProperty("color")] public int Color { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("isAlive")] public bool IsAlive { get; set; }

        
        public PlayerData(PlayerInfo pi)
        {
            Name = pi.Name;
            Color = pi.Color;
            IsAlive = !pi.IsDead && !pi.IsDisconnected;
        }

    }

    public class PlayerInfoBase
    {
        public byte PlayerId;

        public uint PlayerName;

        public byte ColorId;

        public uint HatId;

        public uint PetId;

        public uint SkinId;

        public byte Disconnected;

        public UInt32 Tasks;

        public byte IsImpostor;

        public byte IsDead;

        public IntPtr _object;

        public bool GetIsDead()
        {
            return this.IsDead > 0;
        }

        public IntPtr GetTasks()
        {
            return (IntPtr) Tasks;
        }

        public bool GetIsImpostor()
        {
            return this.IsImpostor > 0;
        }

        public string GetPlayerName()
        {
            return "";
        }

        public PlayerColor GetPlayerColor()
        {
            return (PlayerColor) this.ColorId;
        }

        public bool GetIsDisconnected()
        {
            return this.Disconnected > 0;
        }
    }

}