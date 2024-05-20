using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 控制播放战场中的对话(TKS中没有使用此逻辑)
    /// </summary>
    public class ConversationController : MonoBehaviour
    {
        public static event EventHandler completeEvent;

        IEnumerator conversation;

        void Start()
        {

        }

        /// <summary>
        /// 返回整个动画完全播放完毕大概的时间评估
        /// 主要针对当前TKS没有对话时默认自动进入下一阶段的经验时间
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public float Show(List<string> data)
        {
            float fConversationDuration = 0f;
            if (data == null || data.Count == 0)
            {
                conversation = NoSequence();
                fConversationDuration = AutoNextStateDuration;
            }
            else
            {
                conversation = Sequence(data);
            }
            conversation.MoveNext();
            return fConversationDuration;
        }

        public void Next()
        {
            if (conversation == null)
                return;

            conversation.MoveNext();
        }

        IEnumerator Sequence(List<string> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                /*
                SpeakerData sd = data[i];

                ConversationPanel currentPanel = (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.MiddleLeft || sd.anchor == TextAnchor.LowerLeft) ? leftPanel : rightPanel;
                IEnumerator presenter = currentPanel.Display(sd);
                presenter.MoveNext();

                string show, hide;
                if (sd.anchor == TextAnchor.UpperLeft || sd.anchor == TextAnchor.UpperCenter || sd.anchor == TextAnchor.UpperRight)
                {
                    show = ShowTop;
                    hide = HideTop;
                }
                else
                {
                    show = ShowBottom;
                    hide = HideBottom;
                }

                currentPanel.panel.SetPosition(hide, false);
                MovePanel(currentPanel, show);

                yield return null;
                while (presenter.MoveNext())
                    yield return null;

                MovePanel(currentPanel, hide);
                transition.completedEvent += delegate(object sender, EventArgs e) {
                    conversation.MoveNext();
                };
                */

                yield return null;
            }


            if (completeEvent != null)
                completeEvent(this, EventArgs.Empty);
        }

        private readonly float AutoNextStateDuration = 0.5f;

        private IEnumerator NoSequence()
        {
            yield return new WaitForSeconds(AutoNextStateDuration);

            if (completeEvent != null)
                completeEvent(this, EventArgs.Empty);
        }
    }
}