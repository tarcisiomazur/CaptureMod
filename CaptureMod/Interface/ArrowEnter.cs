using System;
using CaptureMod.Utils;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CaptureMod.Interface
{
    public static class ArrowEnter
    {
        public static GameObject ArrowEnterBase;
        public static void Build(Transform arrow)
        {
            var enter = arrow.GetChild("GameIdText/arrowEnter").gameObject;
            ArrowEnterBase = Object.Instantiate(enter);
            ArrowEnterBase.active = false; 
            Object.DontDestroyOnLoad(ArrowEnterBase);
        }

        public static GameObject GetBackGroundBase()
        {
            return MyTab.GetBackGroundBase();
        }
        
        public static GameObject MakeEnter(Transform parent, Action onEnter,GameObject goBase = null)
        {
            if (goBase == null)
            {
                goBase = GetBackGroundBase();
            }
            var ArrowEnterConnect = Object.Instantiate(goBase, parent);
            var pb = ArrowEnterConnect.AddComponent<PassiveButton>();
            var brh = ArrowEnterConnect.AddComponent<ButtonRolloverHandler>();
            var bc = ArrowEnterConnect.AddComponent<BoxCollider2D>();
            
            ArrowEnterConnect.active = true;
            var sprite = ArrowEnterBase.GetComponent<SpriteRenderer>().sprite;
            var sr = ArrowEnterConnect.GetComponent<SpriteRenderer>();
            
            sr.sprite = sprite;
            sr.size = new Vector2(0.3f, 0.3f);
            ArrowEnterConnect.transform.SetPos(2.3f,-1.12f,0f);

            pb.Colliders = new Il2CppReferenceArray<Collider2D>(new Collider2D[] {bc});
            pb.OnMouseOver = new Button.ButtonClickedEvent();
            pb.OnMouseOut = new Button.ButtonClickedEvent();
            pb.OnClick = new Button.ButtonClickedEvent();
            pb.OnMouseOver.AddListener(new Action(brh.DoMouseOver));
            pb.OnMouseOut.AddListener(new Action(brh.DoMouseOut));
            pb.OnClick.AddListener(new Action(onEnter));
            
            brh.HoverSound = ArrowEnterBase.GetComponent<ButtonRolloverHandler>().HoverSound;
            brh.OverColor = Color.green;
            brh.OutColor = Color.white;
            brh.Target = sr;

            return ArrowEnterConnect;
        }

    }
}