using System.IO;
using CaptureMod.Path.LobbyOptionsAPI;
using CaptureMod.Utils;

namespace CaptureMod.Interface
{
    public class MyOptions : LobbyOptions
    {
        public static MyOptions CustomGameOptions;
        private const byte SettingsVersion = 1;
        public CustomToggleOption VoIP { get; set; }
        public CustomToggleOption Bot{ get; set; }
        public CustomNumberOption Distance { get; set; }
        
        public MyOptions() : base("mySettings", 60)
        {
            Bot = AddOption(false, "Bot", false);
            VoIP = AddOption(false, "VoIP");
            Distance = AddOption(1.5f, "Distance", 0, 3);
        }

        public override void SetRecommendations()
        {
            Bot.Value = false;
            VoIP.Value = false;
            Distance.Value = 2;
        }


        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(SettingsVersion);
            writer.Write(Bot.Value);
            writer.Write(VoIP.Value);
            writer.Write((byte)Distance.Value);
        }
        
        public override void SerializeRPC(BinaryWriter writer)
        {
            writer.Write(SettingsVersion);
            writer.Write(VoIP.Value);
            writer.Write((byte)Distance.Value);
        }

        public override void DeserializeRPC(BinaryReader reader)
        {
            try
            {
                reader.ReadByte();
                VoIP.Value = reader.ReadBoolean();
                Distance.Value = reader.ReadByte();
            }
            catch
            {
                // ignored
            }
        }

        public override void Deserialize(BinaryReader reader)
        {
            try
            {
                reader.ReadByte();
                Bot.Value = reader.ReadBoolean();
                VoIP.Value = reader.ReadBoolean();
                Distance.Value = reader.ReadByte();
            }
            catch
            {
                // ignored
            }
        }

        public override string ToHudString()
        {
            settings.Length = 0;

            try
            {
                settings.AppendLine();
                settings.AppendLine($"Bot: {Bot.Value}");
                settings.AppendLine($"VoIP: {VoIP.Value}");
                settings.Append("VoipDistance: ");
                settings.Append(Distance.Value);
                settings.AppendLine();
            }
            catch
            {
                // ignored
            }

            return settings.ToString();
        }
    }
}