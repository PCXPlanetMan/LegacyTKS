using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public class ExploreState : BattleState
    {
        public override void Enter()
        {
            base.Enter();
            ShowStatPanel(CurrentTilePos);
        }

        public override void Exit()
        {
            base.Exit();
            HideStatPanel();
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 0)
                owner.ChangeState<CommandSelectionState>();
        }

        protected override void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {
            Vector3 vecMousePos = e.info;
            Vector2Int tilePos = owner.MapManager.MousePosScreenToTile(vecMousePos);

            SelectTile(tilePos);
            Unit target = GetUnit(CurrentTilePos);
            if (target == null)
            {
                HideStatPanel();
            }
            else
            {
                ShowStatPanel(CurrentTilePos);
            }
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            // TODO:通过键盘方向键移动来浏览整个地图,需要解决当前Camera正在Follow的角色冲突的问题
            Vector2Int offsetInput = e.info;
            owner.MapManager.ManualMoveCamera(offsetInput);
        }
    }
}  