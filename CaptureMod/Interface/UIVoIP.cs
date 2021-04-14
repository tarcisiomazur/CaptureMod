using System.Linq;
using CaptureMod.Utils;
using CaptureMod.Path.LobbyOptionsAPI;
using PowerTools;
using UnityEngine;

namespace CaptureMod.Interface
{
    public static class UIVoIP
    {
        public static GameObject AreaVoIP;
        public static void CreateVoipMark()
        {
            if (AreaVoIP != null)
                return;
            var skin = PlayerControl.LocalPlayer.transform.FindChild("Skin");
            AreaVoIP = Object.Instantiate(skin.gameObject, skin.parent);
            AreaVoIP.active = false;
            AreaVoIP.name = "AreaVoIP";
            Object.Destroy(AreaVoIP.GetComponent<SkinLayer>());
            Object.Destroy(AreaVoIP.GetComponent<SpriteAnim>());
            Object.Destroy(AreaVoIP.GetComponent<Animator>());
            var dot = Singleton.HudManager.transform.GetChild("Menu/GeneralTab/SoundGroup/SFXSlider/Bar/Dot").gameObject;
            var sr = AreaVoIP.GetComponent<SpriteRenderer>();
            sr.color = new Color(0.2f, 0.2f, 0.5f, 0.11f);
            sr.sprite = dot.GetComponent<SpriteRenderer>().sprite;
            SetRadius(MyOptions.CustomGameOptions.Distance.Value);
            MyOptions.CustomGameOptions.Distance.OnChange += OnChangeDistance;
        }

        private static void OnChangeDistance(object sender, OptionBehaviour optionBehaviour)
        {
            if(sender is CustomNumberOption option)
                SetRadius(option.Value);
        }
        
        public static void SetActive(bool active)
        {
            AreaVoIP.active = active;
        }

        private static void SetRadius(float radius)
        {
            //1rad==0,55scl
            AreaVoIP.transform.localScale = new Vector3(radius/0.55f, radius/0.55f, 1);
        }

        public static void SetListeners(byte[] listeners)
        {
            if (listeners == null) return;
            foreach (var pc in PlayerControl.AllPlayerControls)
            {
                var color = pc.nameText.color;
                color.r = listeners.Contains(pc.PlayerId)? 0.5f : 1f;
                pc.nameText.color = color;
            }
        }
    }
}