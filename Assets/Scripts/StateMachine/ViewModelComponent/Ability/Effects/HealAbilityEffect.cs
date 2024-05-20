using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class HealAbilityEffect : BaseAbilityEffect
    {
        public override int Predict(EncapsuleTile target)
        {
            Unit attacker = GetComponentInParent<Unit>();
            Unit defender = target.content.GetComponent<Unit>();
            return GetStat(attacker, defender, GetPowerNotification, 0);
        }

        protected override int OnApply(EncapsuleTile target)
        {
            Unit defender = target.content.GetComponent<Unit>();

            // Start with the predicted value
            int value = Predict(target);

            // Add some random variance
            value = Mathf.FloorToInt(value * UnityEngine.Random.Range(0.9f, 1.1f));

            // Clamp the amount to a range
            value = Mathf.Clamp(value, minDamage, maxDamage);

            // Apply the amount to the target
            Stats s = defender.GetComponent<Stats>();
            s[EnumStatTypes.HP] += value;
            return value;
        }
    }
}