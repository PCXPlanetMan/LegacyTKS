using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class DamageAbilityEffect : BaseAbilityEffect
    {
        public override int Predict(EncapsuleTile target)
        {
            Unit attacker = GetComponentInParent<Unit>();
            Unit defender = target.content.GetComponent<Unit>();

            // Get the attackers base attack stat considering
            // mission items, support check, status check, and equipment, etc
            int attack = GetStat(attacker, defender, GetAttackNotification, 0);

            // Get the targets base defense stat considering
            // mission items, support check, status check, and equipment, etc
            int defense = GetStat(attacker, defender, GetDefenseNotification, 0);

            // Calculate base damage
            // TKS的基础伤害计算:Attack-Defense
            //int damage = attack - (defense / 2);
            int damage = attack - defense;
            damage = Mathf.Max(damage, 1);

            // Get the abilities power stat considering possible variations
            // 技能最终伤害是在人物基础伤害的基础上按照百分比进行计算
            int power = GetStat(attacker, defender, GetPowerNotification, 0);

            // Apply power bonus
            damage = power * damage / 100;
            damage = Mathf.Max(damage, 1);

            // Tweak the damage based on a variety of other checks like
            // Elemental damage, Critical Hits, Damage multipliers, etc.
            damage = GetStat(attacker, defender, TweakDamageNotification, damage);

            // Clamp the damage to a range
            damage = Mathf.Clamp(damage, minDamage, maxDamage);
            return -damage;
        }

        protected override int OnApply(EncapsuleTile target)
        {
            Unit defender = target.content.GetComponent<Unit>();

            // Start with the predicted damage value
            int value = Predict(target);

            // Add some random variance
            // TODO:暂时不添加伤害浮动
            //value = Mathf.FloorToInt(value * UnityEngine.Random.Range(0.9f, 1.1f));

            // Clamp the damage to a range
            value = Mathf.Clamp(value, minDamage, maxDamage);

            // Apply the damage to the target
            Stats s = defender.GetComponent<Stats>();
            s[EnumStatTypes.HP] += value;
            return value;
        }
    }
}