using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public class MoveSequenceState : BattleState
    {
        public override void Enter()
        {
            base.Enter();
            StartCoroutine("Sequence");
        }

        IEnumerator Sequence()
        {
            Movement m = BattleTurn.Actor.GetComponent<Movement>();
            yield return StartCoroutine(m.Traverse(owner.CurrentTile));
            BattleTurn.HasUnitMoved = true;
            owner.ChangeState<CommandSelectionState>();
        }
    }
}