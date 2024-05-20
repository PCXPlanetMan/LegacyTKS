using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 在技能释放之前预先确定技能方向和技能目标选择(实施技能释放时通过点击完成的,点击后即确定了对目标释放技能)
    /// </summary>
    public class AbilityTargetState : BattleState
    {
        List<EncapsuleTile> tiles;
        AbilityRange ar;

        public override void Enter()
        {
            base.Enter();
            ar = BattleTurn.Ability.GetComponent<AbilityRange>();
            SelectTiles();
            // TODO:在地图上显示人物技能范围
            // ...
            if (ar.directionOriented)
                RefreshSecondaryStatPanel(CurrentTilePos);
            if (driver.Current == EnumDrivers.Computer)
                StartCoroutine(ComputerHighlightTarget());
        }

        public override void Exit()
        {
            base.Exit();
            UnHighlightTiles(tiles);
            HideStatPanel();
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 0)
            {
                if (ar.directionOriented || tiles.Contains(BoardOfTilesMap.GetTile(CurrentTilePos)))
                {
                    // TKS中无需显示技能的预估伤害和成功率,直接进入技能结算
                    //owner.ChangeState<ConfirmAbilityTargetState>();
                    owner.ChangeState<PerformAbilityState>();
                }
            }
            else
            {
                // TKS中技能命令是和其他命令在同一层级,因此没有必要再区分CategorySelectionState,在此处如果要返回,则应该返回到命令选择State
                // TKS中是直接在UI中选择了技能,而无需进入下一个State
                //owner.ChangeState<CategorySelectionState>();
                owner.ChangeState<CommandSelectionState>();
            }
        }

        /// <summary>
        /// 根据PC鼠标相对当前人物方向而确定技能预判方向
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {
            Vector3 vecMousePos = e.info;
            Vector2Int tilePos = owner.MapManager.MousePosScreenToTile(vecMousePos);
            tilePos = tilePos - CurrentTilePos;

            if (ar.directionOriented)
            {
                ChangeDirection(tilePos);
            }
            else
            {
                SelectTile(tilePos + CurrentTilePos);
                RefreshSecondaryStatPanel(CurrentTilePos);
            }
        }

        void ChangeDirection(Vector2Int p)
        {
            EnumStateDirections dir = p.GetDirection();
            //Debug.LogFormat("dir={0},BattleTurn.Actor.Dir={1}", dir.ToString(), BattleTurn.Actor.Dir.ToString());
            if (BattleTurn.Actor.Dir != dir)
            {
                UnHighlightTiles(tiles);
                BattleTurn.Actor.Dir = dir;
                BattleTurn.Actor.Match();
                SelectTiles();
            }
        }

        /// <summary>
        /// 技能有目标的时候则高亮选择目标
        /// </summary>
        void SelectTiles()
        {
            tiles = ar.GetTilesInRange(BoardOfTilesMap);
            bool hasTargetInTiles = false;
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].content != null)
                {
                    hasTargetInTiles = true;
                }
            }
            if (hasTargetInTiles)
                HighlightTiles(tiles);
        }

        IEnumerator ComputerHighlightTarget()
        {
            if (ar.directionOriented)
            {
                ChangeDirection(BattleTurn.Plan.attackDirection.GetNormal());
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                Vector2Int cursorPos = CurrentTilePos;
                while (cursorPos != BattleTurn.Plan.fireLocation)
                {
                    if (cursorPos.x < BattleTurn.Plan.fireLocation.x) cursorPos.x++;
                    if (cursorPos.x > BattleTurn.Plan.fireLocation.x) cursorPos.x--;
                    if (cursorPos.y < BattleTurn.Plan.fireLocation.y) cursorPos.y++;
                    if (cursorPos.y > BattleTurn.Plan.fireLocation.y) cursorPos.y--;
                    SelectTile(cursorPos);
                    yield return new WaitForSeconds(0.25f);
                }
            }
            yield return new WaitForSeconds(0.5f);
            // TKS中无需显示技能的预估伤害和成功率,直接进入技能结算
            //owner.ChangeState<ConfirmAbilityTargetState>();
            owner.ChangeState<PerformAbilityState>();
        }
    }
}