using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 用于调整人物的朝向(TKS里面没有没有专门去处理这种逻辑)
    /// 可以在"休息"和"防御"后立马结束当前人物回合
    /// 实际上本State只用于结束当前人物的回合
    /// </summary>
    public class EndFacingState : BattleState
    {
        EnumStateDirections startDir;

        public override void Enter()
        {
            base.Enter();

            // 如果TKS需要手动调整人物朝向,则对于AI人物则自动调整
            //startDir = BattleTurn.Actor.Dir;
            //SelectTile(BattleTurn.Actor.Tile.VecPos);
            //owner.FacingIndicator.gameObject.SetActive(true);
            //owner.facingIndicator.SetDirection(BattleTurn.Actor.Dir);
            //if (driver.Current == EnumDrivers.Computer)
            //    StartCoroutine(ComputerControl());

            StartCoroutine(ComputerControl());
        }

        public override void Exit()
        {
            //owner.FacingIndicator.gameObject.SetActive(false);
            base.Exit();
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            // 根据鼠标位置调整人物朝向标识符
            //BattleTurn.Actor.Dir = e.info.GetDirection();
            //BattleTurn.Actor.Match();
            //owner.FacingIndicator.SetDirection(BattleTurn.Actor.Dir);
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            // 人物朝向通过鼠标Move指定后再通过点击鼠标进行确认本State结束
            //switch (e.info)
            //{
            //    case 0:
            //        owner.ChangeState<SelectUnitState>();
            //        break;
            //    case 1:
            //        BattleTurn.Actor.Dir = startDir;
            //        BattleTurn.Actor.Match();
            //        owner.ChangeState<CommandSelectionState>();
            //        break;
            //}
        }

        IEnumerator ComputerControl()
        {
            // 如果AI自动调整人物朝向,则需要同步显示标识符
            //yield return new WaitForSeconds(0.5f);
            //BattleTurn.Actor.Dir = owner.CPU.DetermineEndFacingDirection();
            //BattleTurn.Actor.Match();
            //owner.FacingIndicator.SetDirection(BattleTurn.Actor.Dir);

            yield return new WaitForSeconds(0.5f);
            owner.ChangeState<SelectUnitState>();
        }
    }
}