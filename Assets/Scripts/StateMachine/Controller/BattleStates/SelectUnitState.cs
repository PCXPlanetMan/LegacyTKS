using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class SelectUnitState : BattleState
    {
        public override void Enter()
        {
            base.Enter();
            StartCoroutine(ChangeCurrentUnit());
        }

        public override void Exit()
        {
            base.Exit();
            HideStatPanel();
        }

        private IEnumerator ChangeCurrentUnit()
        {
            // 进入某个角色的下个回合(将该角色设为当前回合操作的对象)
            owner.Round.MoveNext();
            SelectTile(BattleTurn.Actor.Tile);
            ShowStatPanel(BattleTurn.Actor.Tile.VecPos);
            yield return null;
            owner.ChangeState<CommandSelectionState>();
        }
    }
}