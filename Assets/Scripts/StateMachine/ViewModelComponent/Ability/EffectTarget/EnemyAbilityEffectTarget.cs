using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;
using com.tksr.property;

namespace com.tksr.statemachine
{
    public class EnemyAbilityEffectTarget : AbilityEffectTarget
    {
        Alliance alliance;

        void Start()
        {
            alliance = GetComponentInParent<Alliance>();
        }

        public override bool IsTarget(EncapsuleTile tile)
        {
            if (tile == null || tile.content == null)
                return false;

            Alliance other = tile.content.GetComponentInChildren<Alliance>();
            return alliance.IsMatch(other, EnumTargets.Foe);
        }
    }
}