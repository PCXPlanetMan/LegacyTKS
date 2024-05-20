using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class MagicalAbilityPower : BaseAbilityPower
    {
        public int level;

        protected override int GetBaseAttack()
        {
            return GetComponentInParent<Stats>()[EnumStatTypes.MAT];
        }

        protected override int GetBaseDefense(Unit target)
        {
            return target.GetComponent<Stats>()[EnumStatTypes.MDF];
        }

        protected override int GetPower()
        {
            return level;
        }
    }
}