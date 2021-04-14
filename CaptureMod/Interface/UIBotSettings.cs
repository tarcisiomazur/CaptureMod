using System;
using CaptureMod.Connection;
using CaptureMod.Path;
using CaptureMod.Utils;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CaptureMod.Interface
{
    public class MyClass : MonoBehaviour
    {
        public MyClass(IntPtr ptr) : base(ptr){}

        internal void Update()
        {
            "Update".Log();
        }
    }
    public static class UIBotSettings
    {
        public static string IP { get; set; }
        public static int Port { get; set; }
        public static string Code { get; set; }
        public static string DiscordID { get; set; }

        public static string URL
        {
            get => $"http://{IP}:{Port}";
            private set
            {
                value.Log();
                var uri = ClientSocket.BuildUri(value);
                IP = uri?.Host;
                Port = uri?.Port ?? 0;
            }
        }

        static UIBotSettings()
        {
            ClientSocket.Instance.OnConnected += OnConnected;
            ClientSocket.Instance.OnLogin += OnLogin;
            ClientSocket.Instance.OnLogout += OnLogout;
            ClientSocket.Instance.OnDisconnected += OnDisconnected;
        }

        private static void OnLogin(object sender, string discordID)
        {
            $"PlayerPrefs discordID {discordID} saved".Log();
            PlayerPrefs.SetString("DiscordID", discordID);
        }

        private static void OnConnected(object sender, string url)
        {
            $"PlayerPrefs url {url} saved".Log();
            PlayerPrefs.SetString("BotIP", url);
        }

        private static void OnLogout(object sender, string e)
        {
            
        }

        private static void OnDisconnected(object sender, EventArgs e)
        {
            
        }

        public static void CreateSettings(Transform myTab)
        {
            URL = PlayerPrefs.GetString("BotIP", "10.0.0.15:8123");
            DiscordID = PlayerPrefs.GetString("DiscordID");
            if (!string.IsNullOrEmpty(DiscordID))
                ClientSocket.Instance.ConnectCode = DiscordID;
            
            var bsg = new GameObject("BotSettingsGroup");
            bsg.transform.parent = myTab;
            bsg.transform.SetPos(0, 1.8f, -2f);
            var connection = Object.Instantiate(MyTab.GetTitleBase(), bsg.transform);
            connection.name = "Connection";
            var render = connection.GetComponent<TextMeshPro>();
            render.SetOnStart(text => text.text = "Connection");
            var ipGroup = new GameObject("IpGroup");
            var manualGroup = new GameObject("ManualGroup");
            var discordGroup = new GameObject("DiscordGroup");
            ipGroup.transform.parent = bsg.transform;
            ipGroup.transform.SetPos(0, -0.4f, 0);
            manualGroup.transform.parent = bsg.transform;
            manualGroup.transform.SetPos(0, -0.9f, 0);
            discordGroup.transform.parent = bsg.transform;
            discordGroup.transform.SetPos(0, -1.4f, 0);

            #region IpGroup

            var ipGroupText = Object.Instantiate(MyTab.GetTextBase(), ipGroup.transform);
            ipGroupText.name = "Text";
            ipGroupText.transform.localPosition = new Vector3(-1.9f, 0, 0);
            var ipTMP = ipGroupText.GetComponent<TextMeshPro>();
            ipTMP.alignment = TextAlignmentOptions.Left;
            ipTMP.SetOnStart(renderer => renderer.text = "IP:Port");
            var ipGroupTextBox = new MyTextBox("IpPortTextBox", 50, URL, ipGroup.transform, new Vector2(3, 0.4f),
                new Vector3(1, 0, 0), true);
            ipGroupTextBox.OnChange += (_, s) => URL = s;
            ipGroupTextBox.OnEnter += (_, __) => OnFastConnect();

            #endregion

            #region ManualGroup

            var manualGroupText = Object.Instantiate(MyTab.GetTextBase(), manualGroup.transform);
            manualGroupText.name = "Text";
            manualGroupText.transform.localPosition = new Vector3(-1.9f, 0, 0);
            var rectTrans = manualGroupText.GetComponent<RectTransform>();
            rectTrans.rect.size.Set(1.8f, 0.4f);
            rectTrans.sizeDelta.Set(1.8f, 0.4f);
            var manualTMP = manualGroupText.GetComponent<TextMeshPro>();
            manualTMP.alignment = TextAlignmentOptions.Left;
            manualTMP.SetOnStart(renderer => renderer.text = "Manual Connect Code");
            var manualGroupTextBox = new MyTextBox("ConnectCode", 8, "", manualGroup.transform, new Vector2(2, 0.4f),
                new Vector3(0.75f, 0, 0), true);
            manualGroupTextBox.OnChange += (_, s) => Code = s;
            manualGroupTextBox.OnEnter += (_,__) => OnManualConnect();
            manualGroupTextBox.TextBox.ForceUppercase = true;
            manualGroupTextBox.TextBox.ClearOnFocus = true;
            manualGroupTextBox.TextBox.allowAllCharacters = false;
            manualGroupTextBox.TextBox.AllowSymbols = false;
            var manualConnectArrow = ArrowEnter.MakeEnter(manualGroup.transform, OnManualConnect);
            manualConnectArrow.transform.SetPos(2.25f,0,0);

            #endregion


            #region DiscordGroup

            var discordGroupText = Object.Instantiate(MyTab.GetTextBase(), discordGroup.transform);
            discordGroupText.name = "Text";
            discordGroupText.transform.localPosition = new Vector3(-1.9f, 0, 0);
            var discordTMP = discordGroupText.GetComponent<TextMeshPro>();
            discordTMP.SetOnStart(renderer => renderer.text = "DiscordID");
            discordTMP.alignment = TextAlignmentOptions.Left;
            var discordValueText = Object.Instantiate(MyTab.GetTextBase(), discordGroup.transform);
            var discordTr = discordValueText.GetComponent<TextMeshPro>();
            discordTr.SetOnStart(renderer => renderer.text = DiscordID);
            discordValueText.name = "Value";
            discordValueText.transform.localPosition = new Vector3(0.5f, 0, 0);
            var fastConnectArrow = ArrowEnter.MakeEnter(discordGroup.transform, OnFastConnect);
            fastConnectArrow.transform.SetPos(2.25f,0,0);
            ClientSocket.Instance.OnLogin += (_, discordID) => discordTr.text = DiscordID = discordID;

            #endregion
            
            UIInGameStatus.Create();
        }

        private static readonly Action OnManualConnect = () =>
        {
            "OnmanualConnect".Log();
            MyOptions.CustomGameOptions.Bot.SetValue(true);
            ClientSocket.Instance.Connect(Code);
        };
        
        private static readonly Action OnFastConnect = () =>
        {
            "OnFastConnect".Log();
            MyOptions.CustomGameOptions.Bot.SetValue(true);
            ClientSocket.Instance.Connect(DiscordID);
        };

}
}