using System;
using System.Collections.Generic;
using CaptureMod.Path;
using CaptureMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CaptureMod.Interface
{

    public static class MyTab
    {
        private static Transform GeneralButton;
        private static Transform GraphicsButton;
        private static Transform GeneralTab;
        private static Transform GraphicTab;
        private static Transform _myButton;
        private static Transform _myTab;

        public static GameObject GetTitleBase()
        {
            return GeneralTab.transform.GetChild("ControlGroup/ControlText_TMP").gameObject;
        }

        public static GameObject GetTextBase()
        {
            return GeneralTab.transform.GetChild("ControlGroup/StickSizeSlider/Text_TMP").gameObject;
        }

        public static GameObject GetBackGroundBase()
        {
            return GeneralTab.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
        }


        public static void CreateBotSettings(string name)
        {
            var Menu_GO = Singleton.HudManager.GameMenu.gameObject.transform;

            GeneralButton = Menu_GO.GetChild(1);
            GraphicsButton = Menu_GO.GetChild(3);
            _myButton = Object.Instantiate(GeneralButton, GeneralButton.transform.parent);
            _myButton.name = name + "Button";
            _myButton.GetChild(1).GetComponent<TextMeshPro>().SetOnStart(renderer => renderer.text = name);

            // Reposiciona

            GeneralButton.transform.Move(-0.5f);
            _myButton.transform.SetPos(1.8f, 2.4f);
            GraphicsButton.transform.SetPos(0);


            // Configura botões
            var pb = _myButton.GetComponent<PassiveButton>();
            GraphicsButton.GetComponent<PassiveButton>().OnClick.AddListener(CloseMyConfigs);
            GeneralButton.GetComponent<PassiveButton>().OnClick.AddListener(CloseMyConfigs);
            pb.OnClick = new Button.ButtonClickedEvent();
            pb.OnClick.AddListener(OpenMyConfigs);

            //Instancia uma nova tab
            GeneralTab = Singleton.HudManager.GameMenu.transform.GetChild(2);
            GraphicTab = Singleton.HudManager.GameMenu.transform.GetChild(4);
            _myTab = Object.Instantiate(GeneralTab, GeneralTab.transform.parent);
            _myTab.gameObject.active = false;
            _myTab.name = name + "Tab";

            //Destrói o lixo da tab
            Object.Destroy(_myTab.transform.GetChild(5).gameObject);
            Object.Destroy(_myTab.transform.GetChild(4).gameObject);
            Object.Destroy(_myTab.transform.GetChild(3).gameObject);
            Object.Destroy(_myTab.transform.GetChild(2).gameObject);
            Object.Destroy(_myTab.transform.GetChild(1).gameObject);
            Object.Destroy(_myTab.transform.GetChild(0).gameObject);

            //Altera os nomes
            UIBotSettings.CreateSettings(_myTab);
            UIVoIPSettings.CreateSettings(_myTab);

            //Altera a abertura das configurações
            Singleton.HudManager.transform.GetChild(2).GetComponent<ButtonBehavior>()
                .OnClick.AddListener(new Action(() =>
                    {
                        var go = Singleton.HudManager.transform.GetChild(4).gameObject;
                        if (go.active)
                            _myTab.gameObject.active = false;
                    })
                );

        }

        private static readonly Action CloseMyConfigs = () =>
        {
            _myTab.gameObject.active = false;
            _myButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
        };

        private static readonly Action OpenMyConfigs = () =>
        {
            GeneralButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            GraphicsButton.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            GeneralTab.gameObject.active = false;
            GraphicTab.gameObject.active = false;
            _myTab.gameObject.active = true;
        };

    }

}