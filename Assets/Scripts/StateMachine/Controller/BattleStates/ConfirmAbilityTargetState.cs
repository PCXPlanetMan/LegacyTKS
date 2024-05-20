using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 此状态处于技能目标选择确定之后和技能伤害释放之前.一般作用是用来演示技能范围内(有些技能是范围伤害)所有目标受到技能的影响,例如技能对于目标的成功率/可能产生的伤害
    /// 在TKS中目前没有这个中间状态,技能选择确定后即可以直接释放伤害
    /// </summary>
    public class ConfirmAbilityTargetState : BattleState
    {
        List<EncapsuleTile> tiles;
        AbilityArea aa;
        int index = 0;

        public override void Enter()
        {
            base.Enter();
            aa = BattleTurn.Ability.GetComponent<AbilityArea>();
            tiles = aa.GetTilesInArea(BoardOfTilesMap, CurrentTilePos);
            HighlightTiles(tiles);
            FindTargets();
            //ShowStatPanel(BattleTurn.Actor.Tile.VecPos);
            if (BattleTurn.Targets.Count > 0)
            {
                //if (driver.Current == EnumDrivers.Human)
                //    hitSuccessIndicator.Show();
                SetTarget(0);
            }
            if (driver.Current == EnumDrivers.Computer)
                StartCoroutine(ComputerDisplayAbilitySelection());
        }

        public override void Exit()
        {
            base.Exit();
            UnHighlightTiles(tiles);
            HideStatPanel();
            //hitSuccessIndicator.Hide();
        }

        protected override void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {
            if (e.info.y > 0 || e.info.x > 0)
                SetTarget(index + 1);
            else
                SetTarget(index - 1);
        }

        protected override void OnFire(object sender, InfoEventArgs<int> e)
        {
            if (e.info == 0)
            {
                if (BattleTurn.Targets.Count > 0)
                {
                    owner.ChangeState<PerformAbilityState>();
                }
            }
            else
                owner.ChangeState<AbilityTargetState>();
        }

        void FindTargets()
        {
            BattleTurn.Targets = new List<EncapsuleTile>();
            for (int i = 0; i < tiles.Count; ++i)
                if (BattleTurn.Ability.IsTarget(tiles[i]))
                    BattleTurn.Targets.Add(tiles[i]);
        }

        void SetTarget(int target)
        {
            index = target;
            if (index < 0)
                index = BattleTurn.Targets.Count - 1;
            if (index >= BattleTurn.Targets.Count)
                index = 0;

            if (BattleTurn.Targets.Count > 0)
            {
                ShowStatPanel(BattleTurn.Targets[index].VecPos);
                UpdateHitSuccessIndicator();
            }
        }

        void UpdateHitSuccessIndicator()
        {
            int chance = 0;
            int amount = 0;
            EncapsuleTile target = BattleTurn.Targets[index];

            Transform obj = BattleTurn.Ability.transform;
            for (int i = 0; i < obj.childCount; ++i)
            {
                AbilityEffectTarget targeter = obj.GetChild(i).GetComponent<AbilityEffectTarget>();
                if (targeter.IsTarget(target))
                {
                    HitRate hitRate = targeter.GetComponent<HitRate>();
                    chance = hitRate.Calculate(target);

                    BaseAbilityEffect effect = targeter.GetComponent<BaseAbilityEffect>();
                    amount = effect.Predict(target);
                    break;
                }
            }

            //hitSuccessIndicator.SetStats(chance, amount);
        }

        IEnumerator ComputerDisplayAbilitySelection()
        {
            //owner.battleMessageController.Display(BattleTurn.Ability.name);
            yield return new WaitForSeconds(2f);
            owner.ChangeState<PerformAbilityState>();
        }
    }
}