using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 与表现层的UI交互有关的状态机(耦合性比较强)
    /// 在选中了某个角色后,如果是可控己方人员则显示可执行命令列表和相关属性状态UI
    /// </summary>
    public class CommandSelectionState : BaseAbilityMenuState
    {
        public override void Enter()
        {
            base.Enter();
            ShowStatPanel(BattleTurn.Actor.Tile.VecPos);
            if (driver.Current == EnumDrivers.Computer)
                StartCoroutine(ComputerTurn());
        }

        public override void Exit()
        {
            base.Exit();
            HideStatPanel();
        }

        protected override void LoadMenu()
        {
            if (menuOptions == null)
            {
                menuTitle = DefinesOfStateMachine.SM_GUI_COMMAND_SELECTION_MENU_TITLE;
            }

            // TODO:Load Skill Slot from SaveDocument and Configuration
            Unit target = GetUnit(BattleTurn.Actor.Tile.VecPos);
            if (target != null)
            {
                UIAbilityMenuPanelController.Reset();
                UIAbilityMenuPanelController.Show(target.gameObject);

                UIAbilityMenuPanelController.SetLocked(0, BattleTurn.HasUnitMoved);
                //UIAbilityMenuPanelController.SetLocked(1, BattleTurn.HasUnitActed);
            }
        }

        protected override void Confirm()
        {
            // DO NOTHING
            if (actionCommandHandled == false)
            {
                DoActionCommand();
                actionCommandHandled = true;
                actionCommand = EnumActionCommand.Invalid;
            }
        }

        /// <summary>
        /// 在命令选择State如果回退的话,将会进入全地图探索State(隐藏当前的UI)
        /// </summary>
        protected override void Cancel()
        {
            if (BattleTurn.HasUnitMoved && !BattleTurn.LockMove)
            {
                BattleTurn.UndoMove();
                UIAbilityMenuPanelController.Reset();
                SelectTile(BattleTurn.Actor.Tile);
            }
            else
            {
                HideStatPanel();
                owner.ChangeState<ExploreState>();
            }
        }

        IEnumerator ComputerTurn()
        {
            if (BattleTurn.Plan == null)
            {
                BattleTurn.Plan = owner.CPU.Evaluate();
                BattleTurn.Ability = BattleTurn.Plan.ability;
            }

            yield return new WaitForSeconds(1f);

            if (BattleTurn.HasUnitMoved == false && BattleTurn.Plan.moveLocation != BattleTurn.Actor.Tile.VecPos)
                owner.ChangeState<MoveTargetState>();
            else if (BattleTurn.HasUnitActed == false && BattleTurn.Plan.ability != null)
                owner.ChangeState<AbilityTargetState>();
            else
                owner.ChangeState<EndFacingState>();
        }

        protected override void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {
            // TODO:等待选择指令的时候不应该可以使用Focus浏览地图
        }

        private bool actionCommandHandled = true;
        private EnumActionCommand actionCommand = EnumActionCommand.Invalid;
        protected override void OnActionCommand(object sender, InfoEventArgs<EnumActionCommand> e)
        {
            EnumActionCommand action = e.info;
            //Debug.Log("OnActionCommand = " + action);
            actionCommand = action;
            actionCommandHandled = false;
        }

        private void DoActionCommand()
        {
            switch (actionCommand)
            {
                case EnumActionCommand.Move:
                    {
                        owner.ChangeState<MoveTargetState>();
                    }
                    break;
                case EnumActionCommand.Attack:
                    {
                        DoAttack();
                    }
                    break;
                case EnumActionCommand.Slot1:
                case EnumActionCommand.Slot2:
                    {
                        owner.ChangeState<CategorySelectionState>();
                    }
                    break;
                case EnumActionCommand.Defense:
                    {

                    }
                    break;
                case EnumActionCommand.Wait:
                    {
                        owner.ChangeState<EndFacingState>();
                    }
                    break;
                default:
                    {
                        Debug.LogErrorFormat("Error Action Command");
                    }
                    break;
            }
        }

        /// <summary>
        /// 进行普通攻击(普通攻击也是技能的一种)
        /// </summary>
        private void DoAttack()
        {
            var abilities = BattleTurn.Actor.GetComponentsInChildren<Ability>();
            Ability ability = null;
            for (int i = 0; i < abilities.Length; i ++)
            {
                ability = abilities[i];
                if (ability.name.CompareTo(DefinesOfStateMachine.SM_GO_ABILITY_NORMAL_ATTACK) == 0)
                {
                    BattleTurn.Ability = ability;
                    break;
                }
                ability = null;
            }
            if (ability == null)
            {
                Debug.LogError("No Normal Attack");
                return;
            }
            BattleTurn.Ability = ability;
            owner.ChangeState<AbilityTargetState>();
        }
    }
}