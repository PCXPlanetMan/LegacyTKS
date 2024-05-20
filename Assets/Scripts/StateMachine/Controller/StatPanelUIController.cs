using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tksr.statemachine
{
    public class StatPanelUIController : MonoBehaviour
    {
        private const string ShowKey = "Show";
        private const string HideKey = "Hide";

        [SerializeField]
        private StatPanel primaryPanel;
        [SerializeField]
        private StatPanel secondaryPanel;
        [SerializeField]
        private SimplePanel commandPanelLT;
        [SerializeField]
        private SimplePanel commandPanelLB;

        private Tweener primaryTransition;
        private Tweener secondaryTransition;
        private Tweener simpleLTTransition;
        private Tweener simpleLBTransition;


        void Start()
        {
            if (primaryPanel.panel.CurrentPosition == null)
                primaryPanel.panel.SetPosition(HideKey, false);
            if (secondaryPanel.panel.CurrentPosition == null)
                secondaryPanel.panel.SetPosition(HideKey, false);
            if (commandPanelLB.panel.CurrentPosition == null)
                commandPanelLB.panel.SetPosition(HideKey, false);
            if (commandPanelLT.panel.CurrentPosition == null)
                commandPanelLT.panel.SetPosition(HideKey, false);
        }

        public void ShowPrimary(GameObject obj)
        {
            primaryPanel.Display(obj);
            MovePanel(primaryPanel.panel, ShowKey, ref primaryTransition);
        }

        public void HidePrimary()
        {
            MovePanel(primaryPanel.panel, HideKey, ref primaryTransition);
        }

        public void ShowSecondary(GameObject obj)
        {
            secondaryPanel.Display(obj);
            MovePanel(secondaryPanel.panel, ShowKey, ref secondaryTransition);
        }

        public void HideSecondary()
        {
            MovePanel(secondaryPanel.panel, HideKey, ref secondaryTransition);
        }

        public void ShowSimpleLT(GameObject obj)
        {
            commandPanelLT.Display(obj, "Test");
            MovePanel(commandPanelLT.panel, ShowKey, ref simpleLTTransition);
        }

        public void HideSimpleLT()
        {
            MovePanel(commandPanelLT.panel, HideKey, ref simpleLTTransition);
        }

        public void ShowSimpleLB(GameObject obj)
        {
            commandPanelLB.Display(obj, "Test");
            MovePanel(commandPanelLB.panel, ShowKey, ref simpleLBTransition);
        }

        public void HideSimpleLB()
        {
            MovePanel(commandPanelLB.panel, HideKey, ref simpleLBTransition);
        }

        private void MovePanel(Panel panel, string pos, ref Tweener t, bool animated = true)
        {
            Panel.GUITilePosition target = panel[pos];
            if (panel.CurrentPosition != target)
            {
                if (t != null)
                    t.Stop();
                t = panel.SetPosition(pos, animated);
                if (t != null)
                {
                    t.duration = 0.5f;
                    t.equation = EasingEquations.EaseOutQuad;
                }
            }
        }
    }
}
