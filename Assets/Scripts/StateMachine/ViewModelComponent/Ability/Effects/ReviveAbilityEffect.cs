using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class ReviveAbilityEffect : BaseAbilityEffect
    {
        public float percent;

        public override int Predict(EncapsuleTile target)
        {
            Stats s = target.content.GetComponent<Stats>();
            return Mathf.FloorToInt(s[EnumStatTypes.MHP] * percent);
        }

        protected override int OnApply(EncapsuleTile target)
        {
            Stats s = target.content.GetComponent<Stats>();
            int value = s[EnumStatTypes.HP] = Predict(target);
            return value;
        }
    }
}