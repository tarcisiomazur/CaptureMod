using CaptureMod.Connection;
using CaptureMod.Path;
using CaptureMod.Utils;
using CaptureMod.Path.LobbyOptionsAPI;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CaptureMod.Interface
{
    public static class UIInGameStatus
    {
        private static TextMeshPro TextMeshPro { get; set; }
        private static Transform _textTransform;
        private static Transform _textTransformBase;
        private static string _text;
        private static Color _color;
        public static string Text
        {
            get => _text;
            set
            {
                if (_text == value) return;
                _text = value;
                if (TextMeshPro != null) TextMeshPro.SetText(_text);
            }
        }
        
        public static Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                _color = value;
                if (TextMeshPro != null) TextMeshPro.color = _color;
            }
        }
        
        static UIInGameStatus()
        {
            ClientSocket.Instance.OnLogin += (sender, s) =>
            {
                Text = "Connected and Loggued";
                Color = Color.blue;
            };
            
            ClientSocket.Instance.OnConnected += (sender, s) =>
            {
                Text = "Connected";
                Color = Color.green;
            };
            
            ClientSocket.Instance.OnDisconnected += (sender, s) =>
            {
                Text = "Disconnected";
                Color = Color.red;
            };
            
            ClientSocket.Instance.OnLogout += (sender, s) =>
            {
                Text = "Connected";
                Color = Color.green;
            };
            
            MyOptions.CustomGameOptions.Bot.OnChange += (sender, behaviour) =>
            {
                SetActive(((CustomToggleOption) sender).Value);
            };
        }

        public static void Build()
        {
            var hud = Singleton.HudManager.gameObject.transform;
            var igs = Object.Instantiate(hud.FindChild("PingTrackerTMP"));
            igs.gameObject.active = false;
            Object.DontDestroyOnLoad(igs);
            igs.name = "BotStatus";
            _textTransformBase = igs;
            Object.Destroy(igs.gameObject.GetComponent<PingTracker>());
            Object.Destroy(igs.gameObject.GetComponent<AspectPosition>());
        }

        public static void SetActive(bool active)
        {
            TextMeshPro.gameObject.active = active;
        }
        
        public static void Create()
        {
            if (_textTransformBase == null)
                Build();
            
            var hud = Singleton.HudManager.gameObject.transform;
            _textTransform = Object.Instantiate(_textTransformBase, hud.transform);
            _textTransform.SetPos(2.5f,2.5f);
            TextMeshPro = _textTransform.GetComponent<TextMeshPro>();
            TextMeshPro.SetOnStart(renderer =>
            {
                renderer.color = Color;
                renderer.SetText(Text);
            });
            SetActive(MyOptions.CustomGameOptions.Bot.Value);
            
        }

    }
}