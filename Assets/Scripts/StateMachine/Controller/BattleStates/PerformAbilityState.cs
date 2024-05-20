using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 技能伤害结算.
    /// 由于TKS中目前没有ConfirmAbilityTargetState这个中间状态(此状态具有确定技能范围内所有目标的功能)
    /// 所以对于TKS来说在进行伤害结算时必须提前提取出所有受此技能影响的目标
    /// </summary>
    public class PerformAbilityState : BattleState
    {
        List<EncapsuleTile> tiles;
        AbilityArea aa;

        public override void Enter()
        {
            base.Enter();

            // TKS自行处理范围技能目标选择的问题
            aa = BattleTurn.Ability.GetComponent<AbilityArea>();
            tiles = aa.GetTilesInArea(BoardOfTilesMap, CurrentTilePos);
            FindTargets();

            BattleTurn.HasUnitActed = true;
            if (BattleTurn.HasUnitMoved)
                BattleTurn.LockMove = true;
            StartCoroutine(Animate());
        }

        void FindTargets()
        {
            BattleTurn.Targets = new List<EncapsuleTile>();
            for (int i = 0; i < tiles.Count; ++i)
                if (BattleTurn.Ability.IsTarget(tiles[i]))
                    BattleTurn.Targets.Add(tiles[i]);
        }

        IEnumerator Animate()
        {
            // TODO play animations, etc
            
            ApplyAbility();

            yield return PlayAbilityAttackAnimation();

            if (IsBattleOver())
                owner.ChangeState<CutSceneState>();
            else if (!UnitHasControl())
                owner.ChangeState<SelectUnitState>();
            else if (BattleTurn.HasUnitMoved)
                owner.ChangeState<EndFacingState>();
            else
                owner.ChangeState<CommandSelectionState>();
        }

        void ApplyAbility()
        {
            BattleTurn.Ability.Perform(BattleTurn.Targets);
        }

        bool UnitHasControl()
        {
            return BattleTurn.Actor.GetComponentInChildren<KnockOutStatusEffect>() == null;
        }

        private IEnumerator PlayAbilityAttackAnimation()
        {
            
            // 技能释放方播放动画
            if (BattleTurn.Ability.name.CompareTo(DefinesOfStateMachine.SM_GO_ABILITY_NORMAL_ATTACK) == 0)
            {
                // 普通攻击动画(攻击动画播放完毕后需要切换为Idle)
                float fDuration = BattleTurn.Actor.PlayNormalAttackAnim();
                yield return new WaitForSeconds(fDuration);

                // TODO:普通攻击可能同时击中多个目标,同时播放所有受击者的动画
                for (int i = 0; i < BattleTurn.Targets.Count; i++)
                {
                    var target = BattleTurn.Targets[i];
                    Unit defender = target.content.GetComponent<Unit>();
                    Stats s = defender.GetComponent<Stats>();
                    if (s[EnumStatTypes.HP] == 0)
                    {
                        yield return defender.PlayDeadAnimAsync();
                        UnSpawnBattleCharacter(defender);
                    }
                    else
                    {
                        yield return defender.PlayUnderAttackAnimAsync();
                    }
                }

                // 攻击动画播放完毕后需要切换为Idle(攻击动画结束后停留在最后一帧后等待敌人受击动画结束再切换为Idle)
                BattleTurn.Actor.PlayIdleAnim();
            }
            else
            {
                // 技能动画
                // 在技能释放后,某些技能会产生Debuff或者技能是范围技能,则目标的受击特效需要延后播放
            }


            yield return null;
        }

        private void UnSpawnBattleCharacter(Unit unit)
        {
            if (unit != null)
            {
                unit.UnPlace();
                Units.Remove(unit);

                var cmc = unit.GetComponentInParent<CharMainController>();
                if (cmc != null)
                {
                    cmc.transform.parent = null;
                    cmc.gameObject.SetActive(false);
                    GameObject.Destroy(cmc.gameObject);
                }
            }
        }
    }
}