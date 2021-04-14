using System;
using System.Collections.Generic;
using System.Linq;
using CaptureMod.Runtime;
using CaptureMod.Utils;
using HarmonyLib;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CaptureMod.Path.LobbyOptionsAPI
{
    public class ScrollerOptionsManager
    {
        private static StringNames titleNum = (StringNames) 1337;
        private static List<ScrollerOptions> options = new List<ScrollerOptions>();

        public CustomNumberOption AddOption(byte defaultValue, string title, byte min, byte max, byte step = 1,
            string extension = "", bool onlyHost = true)
        {
            var obj = new CustomNumberOption(defaultValue, titleNum++, title, min, max, step, onlyHost);
            options.Add(obj);
            obj.format = "0" + extension;
            return obj;
        }

        public CustomNumberOption AddOption(float defaultValue, string title, float min, float max, float step = 0.25f,
            string extension = "", bool onlyHost = true)
        {
            var obj = new CustomNumberOption(defaultValue, titleNum++, title, min, max, step, onlyHost);
            options.Add(obj);
            obj.format = "0.0#" + extension;
            return obj;
        }

        public CustomToggleOption AddOption(bool defaultValue, string title, bool onlyHost = true)
        {
            var obj = new CustomToggleOption(defaultValue, titleNum++, title, onlyHost);
            options.Add(obj);
            return obj;
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
            new Type[] {typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>)})]
        static class TranslationController_GetString
        {
            public static bool Prefix(StringNames AKGLBKHCEMI, ref string __result)
            {
                foreach (var opt in options)
                {
                    if (opt.optionTitleName == AKGLBKHCEMI)
                    {
                        __result = opt.optionTitle;
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        static class GameOptionsMenu_Start
        {
            static float GetLowestConfigY(GameOptionsMenu __instance)
            {
                return __instance.GetComponentsInChildren<OptionBehaviour>()
                    .Min(option => option.transform.localPosition.y);
            }

            public static void OnValueChanged(OptionBehaviour option)
            {
                if (AmongUsClient.Instance == null) return;
                foreach (var opt in options)
                {
                    if (opt.optionTitleName == option.Title)
                    {
                        opt.Update(option);
                        opt.OnChange.Invoke(opt,option);
                        break;
                    }
                }
/*
                if (PlayerControl.GameOptions.BMMDHGDAPEJ) // isDefault
                {
                    PlayerControl.GameOptions.BMMDHGDAPEJ = false;
                    "Refresh".Log();
                    Object.FindObjectOfType<GameOptionsMenu>().ValueChanged(option); //RefreshChildren
                }*/

                var local = PlayerControl.LocalPlayer;
                if (local != null && AmongUsClient.Instance.HHBLOCGKFAB) //amHost
                {
                    local.RpcSyncSettings(PlayerControl.GameOptions);
                }
            }

            static void Postfix(ref GameOptionsMenu __instance)
            {
                var lowestY = GetLowestConfigY(__instance);
                float offset = 0.0f;
                foreach (var opt in options)
                {
                    offset += 0.5f;
                    if (opt.onlyHost && !AmongUsClient.Instance.HHBLOCGKFAB) continue;
                    opt.Start(__instance, lowestY, offset, OnValueChanged);
                }

                __instance.GetComponentInParent<Scroller>().YBounds.max += offset - 0.9f;
            }
        }

        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.OnEnable))]
        static class GameSettingsMenu_OnEnable
        {
            static void Prefix(ref GameSettingMenu __instance)
            {
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.OnEnable))]
        static class NumberOption_OnEnable
        {
            static bool Prefix(ref NumberOption __instance)
            {
                foreach (var opt in options)
                {
                    if (__instance.Title == opt.optionTitleName)
                    {
                        opt.OnEnable(ref __instance, GameOptionsMenu_Start.OnValueChanged);
                        return false;
                    }
                }

                return true;
            }
        }

        [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.OnEnable))]
        static class ToggleOption_OnEnable
        {
            static bool Prefix(ref ToggleOption __instance)
            {
                foreach (var opt in options)
                {
                    if (__instance.Title == opt.optionTitleName)
                    {
                        opt.OnEnable(ref __instance, GameOptionsMenu_Start.OnValueChanged);
                        return false;
                    }
                }

                return true;
            }
        }
    }

    public class CustomNumberOption : ScrollerOptions
    {
        private NumberOption _countOption;
        private float _value = 0;
        public float min;
        public float max;
        public float step;
        public string format = "0.0";
        public float Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnChange?.Invoke(this,null);
                }
            }
        }

        public CustomNumberOption(float defaultValue, StringNames optionTitleName, string optionTitle, float min,
            float max, float step, bool onlyHost) : base(optionTitleName, optionTitle, onlyHost)
        {
            this._value = defaultValue;
            this.min = min;
            this.max = max;
            this.step = step;
        }

        public override void Start(GameOptionsMenu __instance, float lowestY, float offset,
            Action<OptionBehaviour> callback)
        {
            
            var go = RuntimeProvider.GetGameObjectAsset("PlayerOptionsMenu");
            var no = go.transform.GetChild("GameTab/GameGroup/SliderInner/EmergencyMeetings").GetComponent<NumberOption>();

            _countOption = Object.Instantiate(no, __instance.transform);
            _countOption.transform.localPosition = new Vector3(_countOption.transform.localPosition.x,
                lowestY - offset,
                _countOption.transform.localPosition.z);
            _countOption.Title = optionTitleName;
            _countOption.Value = Value;
            _countOption.TitleText.SetText(optionTitle);
            _countOption.OnValueChanged = new Action<OptionBehaviour>(callback);
            _countOption.gameObject.AddComponent<OptionBehaviour>();
            _countOption.ValidRange.max = max;
            _countOption.ValidRange.min = min;
            _countOption.Increment = step;
            _countOption.FormatString = format;
        }

        public override void Update(OptionBehaviour option)
        {
            _value = option.GetFloat();
        }

        public override void OnEnable(ref NumberOption __instance, Action<OptionBehaviour> callback)
        {
            __instance.TitleText.SetText(optionTitle);
            __instance.OnValueChanged = new Action<OptionBehaviour>(callback);
            __instance.Value = Value;
            __instance.enabled = true;
        }
    }

    public class CustomToggleOption : ScrollerOptions
    {
        
        private ToggleOption toggleOption;
        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnChange?.Invoke(this, null);
                }
            }
        }

        public CustomToggleOption(bool defaultValue, StringNames optionTitleName, string optionTitle, bool onlyHost) : base(
            optionTitleName, optionTitle, onlyHost)
        {
            this.Value = defaultValue;
        }

        public override void Start(GameOptionsMenu __instance, float lowestY, float offset,
            Action<OptionBehaviour> callback)
        {
            var go = RuntimeProvider.GetGameObjectAsset("PlayerOptionsMenu");
            var to = go.transform.GetChild("GameTab/GameGroup/SliderInner/ConfirmEjects").GetComponent<ToggleOption>();

            toggleOption = Object.Instantiate(to, __instance.transform);
            toggleOption.transform.localPosition = new Vector3(toggleOption.transform.localPosition.x,
                lowestY - offset, toggleOption.transform.localPosition.z);
            toggleOption.Title = optionTitleName;
            toggleOption.CheckMark.enabled = Value;
            toggleOption.TitleText.SetText(optionTitle);
            toggleOption.OnValueChanged = new Action<OptionBehaviour>(callback);
            toggleOption.gameObject.AddComponent<OptionBehaviour>();
        }

        public override void Update(OptionBehaviour option)
        {
            Value = option.GetBool();
        }

        public override void OnEnable(ref ToggleOption __instance, Action<OptionBehaviour> callback)
        {
            __instance.TitleText.SetText(optionTitle);
            __instance.OnValueChanged = new Action<OptionBehaviour>(callback);
            __instance.enabled = Value;
        }

        public void SetValue(bool value)
        {
            _value = value;
            if(toggleOption.GetBool() != value)
                toggleOption.Toggle();
        }
    }

    public abstract class ScrollerOptions
    {
        public StringNames optionTitleName;
        public string optionTitle;
        public bool onlyHost;
        public EventHandler<OptionBehaviour> OnChange { get; set; }

        public ScrollerOptions(StringNames optionTitleName, string optionTitle, bool onlyHost)
        {
            this.optionTitleName = optionTitleName;
            this.optionTitle = optionTitle;
            this.onlyHost = onlyHost;
        }

        public abstract void Start(GameOptionsMenu __instance, float lowestY, float offset,
            Action<OptionBehaviour> callback);

        public abstract void Update(OptionBehaviour option);

        public virtual void OnEnable(ref NumberOption __instance, Action<OptionBehaviour> callback)
        {
        }

        public virtual void OnEnable(ref ToggleOption __instance, Action<OptionBehaviour> callback)
        {
        }
    }
}