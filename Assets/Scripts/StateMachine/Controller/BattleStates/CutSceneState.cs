using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using com.tksr.data;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 切换场景的状态,主要是负责战斗对话播放.
    /// 可以处理交互消息,用于触发对话Next进行(例如鼠标点击屏幕则进行下一段对话从而推动剧情)
    /// TKS不需要战场对话,所以可以不用重载OnFire
    /// </summary>
    public class CutSceneState : BattleState
    {
        private ConversationController conversationController;

        protected override void Awake()
        {
            base.Awake();
            conversationController = gameObject.AddComponent<ConversationController>();
        }

        private EnumBattleAnimStatus curAnimStatus = EnumBattleAnimStatus.None;

        public override void Enter()
        {
            base.Enter();
            if (IsBattleOver())
            {
                if (DidPlayerWin())
                {
                    // TODO: 战斗胜利结束对话内容
                    Debug.Log("TODO: Battle Over, Win");
                    curAnimStatus = EnumBattleAnimStatus.EndWin;
                }
                else
                {
                    // TODO: 战斗失败结束对话内容
                    Debug.Log("TODO: Battle Over, Lose");
                    curAnimStatus = EnumBattleAnimStatus.EndLose;
                }
            }
            else
            {
                // TODO: 战斗开场对话内容
                Debug.Log("TODO: Opening Battle Info");
                curAnimStatus = EnumBattleAnimStatus.Open;
            }

            if (curAnimStatus != EnumBattleAnimStatus.Open)
            {
                // 获取对话内容,若为空,则理论上应该直接切换下一个状态 
                // 但由于此时还处于CutScene这个State的Enter状态,所以根本无法切换到下一个State(即使调用了owner.ChangeState<SelectUnitState>())
                // 因此应该在本State的Enter结束后再模拟手动MoveNext
                List<string> conversations = new List<string>(); // TODO:如果TKS需要显示对话,则在此处加载所需要的对话内容
                float fDuration = conversationController.Show(conversations);
                if (conversations.Count == 0)
                {
                    StartCoroutine(AutoNextState(fDuration));
                }
            }
            else
            {
                // 获取对话内容,若为空,则理论上应该直接切换下一个状态 
                // 但由于此时还处于CutScene这个State的Enter状态,所以根本无法切换到下一个State(即使调用了owner.ChangeState<SelectUnitState>())
                // 因此应该在本State的Enter结束后再模拟手动MoveNext
                List<string> conversations = new List<string>(); // TODO:如果TKS需要显示对话,则在此处加载所需要的对话内容
                float fDuration = conversationController.Show(conversations);
                if (conversations.Count == 0)
                {
                    // 显示战斗开场动画
                    StartCoroutine(ShowOpenAnimation());
                }
            }
        }

        private IEnumerator ShowOpenAnimation()
        {
            yield return owner.UIDisplayBattleInfoController.DisplayBattleOpenAnimation();
            conversationController.Next();
        }

        private IEnumerator AutoNextState(float fDuration)
        {
            yield return new WaitForSeconds(fDuration);   
            conversationController.Next();
        }

        public override void Exit()
        {
            base.Exit();
        }

        protected override void AddListeners()
        {
            base.AddListeners();
            ConversationController.completeEvent += OnCompleteConversation;
        }

        protected override void RemoveListeners()
        {
            base.RemoveListeners();
            ConversationController.completeEvent -= OnCompleteConversation;
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            // TKS没有战场对话,不能手动点击下一段对话而推动状态机运行
            //base.OnFire(sender, e);
            //conversationController.Next();
        }

        private void OnCompleteConversation(object sender, System.EventArgs e)
        {
            if (IsBattleOver())
            {
                owner.ChangeState<EndBattleState>();
            }
            else
            {
                owner.ChangeState<SelectUnitState>();
            }
        }
    }
}