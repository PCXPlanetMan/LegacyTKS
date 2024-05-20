using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 管理可交互的指令(行走/攻击/技能/防御/休息等)
    /// </summary>
    public class AbilityMenuPanelUIController : MonoBehaviour
    {
        public static event EventHandler<InfoEventArgs<EnumActionCommand>> actionCommand;

        private const string ShowKey = "Show";
        private const string HideKey = "Hide";

        [SerializeField]
        GameObject canvas;
        [SerializeField]
        private CommandFixPanel mainPanel;
        [SerializeField]
        private StatPanel subPanel;

        void Start()
        {
            mainPanel.parentController = this;
            mainPanel.panel.SetPosition(HideKey, false);
            canvas.SetActive(false);
        }

        public void Show(GameObject obj)
        {
            canvas.SetActive(true);
            mainPanel.Display(obj);
            TogglePos(mainPanel.panel, ShowKey);
        }

        public void Hide()
        {
            Tweener t = TogglePos(mainPanel.panel, HideKey);
            t.completedEvent += delegate (object sender, System.EventArgs e)
            {
                //if (panel.CurrentPosition == panel[HideKey])
                //{
                //    Clear();
                //    canvas.SetActive(false);
                //}
            };
        }
        
        private Tweener TogglePos(Panel panel, string pos)
        {
            Tweener t = panel.SetPosition(pos, true);
            t.duration = 0.5f;
            t.equation = EasingEquations.EaseOutQuad;
            return t;
        }

        public void Reset()
        {
            mainPanel.ResetButtons();
        }

        public void DoCommand(InfoEventArgs<EnumActionCommand> e)
        {
            if (actionCommand != null)
            {
                actionCommand(this, e);
            }
        }

        public void SetLocked(int index, bool locked)
        {
            mainPanel.LockButtonByIndex(index, locked);
        }
    }
}
