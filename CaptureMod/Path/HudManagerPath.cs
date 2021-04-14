using CaptureMod.Utils;
using CaptureMod.Voice;
using HarmonyLib;
using CaptureMod.Connection;
using CaptureMod.Interface;

namespace CaptureMod.Path
{
    [HarmonyPatch]
    public class HudManagerPath
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static void Start()
        {
            MyTab.CreateBotSettings("BotConfigs");

            if(MyOptions.CustomGameOptions.Bot.Value)
                ClientSocket.Instance.ConnectAsync();
            if (MyOptions.CustomGameOptions.VoIP.Value)
                VoIP.Start();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.OnDestroy))]
        public static void OnDestroy()
        {
            VoIP.Stop();
            VoIPConnection.Disconnect();
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TextBoxTMP), nameof(TextBoxTMP.Update))]
        public static void Update(TextBoxTMP __instance)
        {
            if (ArrowEnter.ArrowEnterBase == null && __instance.transform.parent?.name == "JoinGameButton")
            {
                ArrowEnter.Build(__instance.transform.parent);
            }
            if (MyTextBox.TextBoxBase == null && __instance.transform.name == "GameIdText")
            {
                MyTextBox.CopyTextBox(__instance.transform);
            }
        }
    }

}