using System;
using CaptureMod.Path;
using CaptureMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CaptureMod.Interface
{
    public class MyTextBox
    {
        public static GameObject TextBoxBase;
        private GameObject _textBox { get; set; }

        public string Text
        {
            get => TextBox.text;
            set => TextBox.SetText(value);
        }
        
        private int CursorSize { get; set; }
        public TextBoxTMP TextBox { get; private set; }
        
        public EventHandler<string> OnChange { get; set; }
        private Action OnChangeAction => () => OnChange.Invoke(this, Text);
        public EventHandler<string> OnEnter { get; set; }
        private Action OnEnterAction => () => OnEnter.Invoke(this, Text);

        public MyTextBox(string name, int limit, string defaultValue, Transform parent, Vector2 size,
            Vector3 position = new Vector3(), bool active = false)
        {
            _textBox = Object.Instantiate(TextBoxBase, parent);
            _textBox.name = name;
            _textBox.transform.localPosition = position;
            _textBox.active = active;
            TextBox = _textBox.GetComponent<TextBoxTMP>();
            TextBox.characterLimit = limit;
            TextBox.OnEnter = new Button.ButtonClickedEvent();
            TextBox.OnEnter.AddListener(OnEnterAction);
            TextBox.OnChange = new Button.ButtonClickedEvent();
            TextBox.OnChange.AddListener(OnChangeAction);
            TextBox.ForceUppercase = false;
            TextBox.ClearOnFocus = false;
            var tr = _textBox.transform.GetChild("Text_TMP").GetComponent<TextMeshPro>();
            tr.SetOnStart(_ =>
            {
                TextBox.SetText(defaultValue);
                var rect = _textBox.transform.GetChild("Text_TMP").GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(size.x, 0.4f);
            });
            CursorSize = (int) (size.x * 50 * 0.90f);

            var col = _textBox.GetComponent<BoxCollider2D>();
            col.size = size;

            var background = Object.Instantiate(MyTab.GetBackGroundBase(), TextBox.transform);
            background.name = "BackGround";
            background.GetComponent<SpriteRenderer>().size = size;
        }

        public static void CopyTextBox(Transform transform)
        {
            var NameBox = transform.transform.gameObject;
            TextBoxBase = Object.Instantiate(NameBox);

            Object.Destroy(TextBoxBase.transform.GetChild("arrowEnter").gameObject);
            Object.Destroy(TextBoxBase.transform.GetChild("Background").gameObject);
            Object.Destroy(TextBoxBase.GetComponent<NameTextBehaviour>());
            TextBoxBase.active = false;
            TextBoxBase.name = "ModelTextBox";
                
            Object.DontDestroyOnLoad(TextBoxBase);
            // Destrói o antigo background

            var bx = TextBoxBase.GetComponent<TextBoxTMP>();
            var col = TextBoxBase.GetComponent<BoxCollider2D>();
            col.size = col.size.Add(-2, -0.5f);
            bx.characterLimit = 40;
            bx.OnChange = new Button.ButtonClickedEvent();
            bx.OnEnter = new Button.ButtonClickedEvent();
            bx.OnFocusLost = new Button.ButtonClickedEvent();
            bx.SetText("");
            bx.allowAllCharacters = true;
            bx.AllowEmail = true;
            bx.AllowPaste = true;
            bx.AllowSymbols = true;
            var text = bx.transform.GetChild(1);
            text.SetPos(0,0,-1);
            var tr = text.GetComponent<TextMeshPro>();
            tr.enableAutoSizing = true;
            tr.text = bx.text;
            //tr.scaleToFit = true;
            var pipetr = bx.transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();
            //pipetr.scale = 0.9f;
        }


    }
}