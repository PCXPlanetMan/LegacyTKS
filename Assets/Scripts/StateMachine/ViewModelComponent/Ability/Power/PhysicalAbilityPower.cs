using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class PhysicalAbilityPower : BaseAbilityPower
    {
        public int level;

        protected override int GetBaseAttack()
        {
            return GetComponentInParent<Stats>()[EnumStatTypes.ATK];
        }

        protected override int GetBaseDefense(Unit target)
        {
            return target.GetComponent<Stats>()[EnumStatTypes.DEF];
        }

        protected override int GetPower()
        {
            return level;
        }
    }
}