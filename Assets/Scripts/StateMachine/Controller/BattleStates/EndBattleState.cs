using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public class EndBattleState : BattleState
    {
        private bool isAnimFinished = false;

        public override void Enter()
        {
            base.Enter();
            isAnimFinished = false;
            Debug.Log("End Battle");
            //Application.LoadLevel(0);

            if (DidPlayerWin())
            {
                Debug.Log("TODO: Battle Over, Win");
                StartCoroutine(ShowEndAnimation(true));
            }
            else
            {
                Debug.Log("TODO: Battle Over, Lose");
                StartCoroutine(ShowEndAnimation(false));
            }
        }

        private IEnumerator ShowEndAnimation(bool bWin)
        {
            yield return owner.UIDisplayBattleInfoController.DisplayBattleEndAnimation(bWin);
            isAnimFinished = true;
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            base.OnFire(sender, e);
            if (isAnimFinished)
            {
                if (DidPlayerWin())
                {
                    Application.LoadLevel(0);
                }
                else
                {
                    Application.LoadLevel(0);
                }
            }
        }
    }
}