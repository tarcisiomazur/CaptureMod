using System.Collections.Generic;
using System.IO;
using System.Text;
using CaptureMod.Utils;
using HarmonyLib;
using Hazel;
using UnityEngine;

namespace CaptureMod.Path.LobbyOptionsAPI
{
    public abstract class LobbyOptions : ScrollerOptionsManager
    {
        private static List<LobbyOptions> lobbyOptions = new List<LobbyOptions>();

        protected StringBuilder settings = new StringBuilder(2048);
        private string fileName;
        private byte rpcId;
        private int priority;

        public LobbyOptions(string fileName, byte rpcId, int priority = Priority.Normal)
        {
            this.fileName = fileName;
            this.rpcId = rpcId;
            this.priority = priority;
            lobbyOptions.Add(this);
        }

        public abstract void SetRecommendations();
        public abstract void Serialize(BinaryWriter writer);
        public abstract void SerializeRPC(BinaryWriter writer);

        public abstract void Deserialize(BinaryReader reader);
        public abstract void DeserializeRPC(BinaryReader reader);

        public override string ToString()
        {
            return ToHudString();
        }

        public abstract string ToHudString();

        public byte[] ToBytes()
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
                {
                    this.SerializeRPC(binaryWriter);
                    binaryWriter.Flush();
                    memoryStream.Position = 0L;
                    result = memoryStream.ToArray();
                }
            }

            return result;
        }

        public void FromBytes(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    this.DeserializeRPC(binaryReader);
                }
            }
        }

        public void LoadGameOptions(string filename)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
            if (File.Exists(path))
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        Deserialize(binaryReader);
                    }
                }
            }
        }

        private void SaveGameOptions(string filename)
        {
            using (FileStream fileStream = new FileStream(System.IO.Path.Combine(Application.persistentDataPath, filename),
                FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    Serialize(binaryWriter);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
        public static class PlayerControl_RpcSyncSettings
        {
            public static void Postfix(CEIOGGEDKAN DJGAEEMDIDF)
            {
                if (PlayerControl.AllPlayerControls.Count > 1)
                {
                    foreach (var opt in lobbyOptions)
                    {
                        MessageWriter messageWriter =
                            AmongUsClient.Instance.StartRpc(PlayerControl.LocalPlayer.NetId, opt.rpcId,
                                SendOption.Reliable);
                        messageWriter.WriteBytesAndSize(opt.ToBytes());
                        messageWriter.EndMessage();
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class PlayerControl_HandleRpc
        {
            public static void Postfix(byte ONIABIILFGF, MessageReader JIGFBHFFNFI)
            {
                foreach (var opt in lobbyOptions)
                {
                    if (ONIABIILFGF == opt.rpcId)
                    {
                        opt.FromBytes(JIGFBHFFNFI.ReadBytesAndSize());
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CEIOGGEDKAN), nameof(CEIOGGEDKAN.LAOPONOFCIP))] //SetRecommendations
        public static class GameOptionsData_SetRecommendations
        {
            public static void Postfix()
            {
                foreach (var opt in lobbyOptions)
                {
                    opt.SetRecommendations();
                }
            }
        }

        [HarmonyPatch(typeof(BLCGIFOPMIA), nameof(BLCGIFOPMIA.DHHFAHODPGO))] //LoadGameOptions
        public static class SaveManager_LoadGameOptions
        {
            public static void Postfix(string JLDDOLHMOHC)
            {
                if (JLDDOLHMOHC == "gameHostOptions")
                {
                    "Load".Log();
                    foreach (var opt in lobbyOptions)
                    {
                        opt.LoadGameOptions(opt.fileName);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(BLCGIFOPMIA), nameof(BLCGIFOPMIA.PELDOPKNGFA))] //SaveGameOptions
        public static class SaveManager_SaveGameOptions
        {
            public static void Postfix(string JLDDOLHMOHC)
            {
                if (JLDDOLHMOHC == "gameHostOptions")
                {
                    "Save".Log();
                    foreach (var opt in lobbyOptions)
                    {
                        opt.SaveGameOptions(opt.fileName);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CEIOGGEDKAN), nameof(CEIOGGEDKAN.ONCLFHFDADB))]
        static class GameOptionsData_ToHudString
        {
            [HarmonyPriority(Priority.Normal - 1)]
            static void Postfix(ref string __result)
            {
                var builder = new StringBuilder(__result);
                foreach (var lby in lobbyOptions)
                {
                    builder.Append(lby.ToHudString());
                }

                __result = builder.ToString();

                Singleton.HudManager.GameSettings.m_fontScale = 0.5f;
            }
        }
        
    }
}