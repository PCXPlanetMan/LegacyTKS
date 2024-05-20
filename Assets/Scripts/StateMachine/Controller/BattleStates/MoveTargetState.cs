using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class MoveTargetState : BattleState
    {
        List<EncapsuleTile> tiles;

        public override void Enter()
        {
            base.Enter();
            Movement mover = BattleTurn.Actor.GetComponent<Movement>();
            tiles = mover.GetTilesInRange(BoardOfTilesMap);
            HighlightTiles(tiles);
            HideStatPanel();
            ShowSimplePanel(CurrentTilePos, true);
            if (driver.Current == EnumDrivers.Computer)
                StartCoroutine(ComputerHighlightMoveTarget());
        }

        public override void Exit()
        {
            base.Exit();
            UnHighlightTiles(tiles);
            tiles = null;
            HideSimplePanel();
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            SelectTile(e.info + CurrentTilePos);
            ShowStatPanel(CurrentTilePos);
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 0)
            {
                if (tiles.Contains(owner.CurrentTile))
                    owner.ChangeState<MoveSequenceState>();
            }
            else
            {
                owner.ChangeState<CommandSelectionState>();
            }
        }

        protected override void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {
            Vector3 vecMousePos = e.info;
            Vector2Int tilePos = owner.MapManager.MousePosScreenToTile(vecMousePos);
            SelectTile(tilePos);
        }

        IEnumerator ComputerHighlightMoveTarget()
        {
            Vector2Int cursorPos = CurrentTilePos;
            while (cursorPos != BattleTurn.Plan.moveLocation)
            {
                if (cursorPos.x < BattleTurn.Plan.moveLocation.x)
                    cursorPos.x++;
                if (cursorPos.x > BattleTurn.Plan.moveLocation.x)
                    cursorPos.x--;
                if (cursorPos.y < BattleTurn.Plan.moveLocation.y)
                    cursorPos.y++;
                if (cursorPos.y > BattleTurn.Plan.moveLocation.y)
                    cursorPos.y--;
                SelectTile(cursorPos);
                yield return new WaitForSeconds(0.25f);
            }
            yield return new WaitForSeconds(0.5f);
            owner.ChangeState<MoveSequenceState>();
        }
    }
}