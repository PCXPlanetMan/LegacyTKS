using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public abstract class BaseAbilityEffect : MonoBehaviour
    {
        protected const int minDamage = -999;
        protected const int maxDamage = 999;

        public const string GetAttackNotification = "BaseAbilityEffect.GetAttackNotification";
        public const string GetDefenseNotification = "BaseAbilityEffect.GetDefenseNotification";
        public const string GetPowerNotification = "BaseAbilityEffect.GetPowerNotification";
        public const string TweakDamageNotification = "BaseAbilityEffect.TweakDamageNotification";

        public const string MissedNotification = "BaseAbilityEffect.MissedNotification";
        public const string HitNotification = "BaseAbilityEffect.HitNotification";

        public abstract int Predict(EncapsuleTile target);

        public void Apply(EncapsuleTile target)
        {
            // 是否是当前技能可以影响的目标
            if (GetComponent<AbilityEffectTarget>().IsTarget(target) == false)
                return;

            // 根据对象的闪避值(有可能加上己方的命中率)确定是否击中目标
            if (GetComponent<HitRate>().RollForHit(target))
                this.PostNotification(HitNotification, OnApply(target));
            else
                this.PostNotification(MissedNotification);
        }

        protected abstract int OnApply(EncapsuleTile target);

        protected virtual int GetStat(Unit attacker, Unit target, string notification, int startValue)
        {
            var mods = new List<ValueModifier>();
            var info = new Info<Unit, Unit, List<ValueModifier>>(attacker, target, mods);
            this.PostNotification(notification, info);
            mods.Sort(Compare);

            float value = startValue;
            for (int i = 0; i < mods.Count; ++i)
                value = mods[i].Modify(startValue, value);

            int retValue = Mathf.FloorToInt(value);
            retValue = Mathf.Clamp(retValue, minDamage, maxDamage);
            return retValue;
        }

        int Compare(ValueModifier x, ValueModifier y)
        {
            return x.sortOrder.CompareTo(y.sortOrder);
        }
    }
}