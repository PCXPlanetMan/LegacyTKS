using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 特殊的BattleState可以用于处理UI相关逻辑
    /// </summary>
    public abstract class BaseAbilityMenuState : BattleState
    {
        protected string menuTitle;
        protected List<string> menuOptions;

        public override void Enter()
        {
            base.Enter();
            SelectTile(BattleTurn.Actor.Tile);
            if (driver.Current == EnumDrivers.Human)
                LoadMenu();
        }

        public override void Exit()
        {
            base.Exit();
            UIAbilityMenuPanelController.Hide();
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 0)
                Confirm();
            else
                Cancel();
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            //if (e.info.x > 0 || e.info.y < 0)
            //    abilityMenuPanelController.Next();
            //else
            //    abilityMenuPanelController.Previous();
        }

        protected override void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {
            Vector3 vecMousePos = e.info;
            Vector2Int tilePos = owner.MapManager.MousePosScreenToTile(vecMousePos);
            SelectTile(tilePos);
        }

        protected abstract void LoadMenu();
        protected abstract void Confirm();
        protected abstract void Cancel();
    }
}