﻿using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class AbsorbDamageAbilityEffectTarget : BaseAbilityEffect
    {
        public int trackedSiblingIndex;
        BaseAbilityEffect effect;
        int amount;

        void Awake()
        {
            effect = GetTrackedEffect();
        }

        void OnEnable()
        {
            this.AddObserver(OnEffectHit, BaseAbilityEffect.HitNotification, effect);
            this.AddObserver(OnEffectMiss, BaseAbilityEffect.MissedNotification, effect);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnEffectHit, BaseAbilityEffect.HitNotification, effect);
            this.RemoveObserver(OnEffectMiss, BaseAbilityEffect.MissedNotification, effect);
        }

        public override int Predict(EncapsuleTile target)
        {
            return 0;
        }

        protected override int OnApply(EncapsuleTile target)
        {
            Stats s = GetComponentInParent<Stats>();
            s[EnumStatTypes.HP] += amount;
            return amount;
        }

        void OnEffectHit(object sender, object args)
        {
            amount = (int)args * -1;
        }

        void OnEffectMiss(object sender, object args)
        {
            amount = 0;
        }

        BaseAbilityEffect GetTrackedEffect()
        {
            Transform owner = GetComponentInParent<Ability>().transform;
            if (trackedSiblingIndex >= 0 && trackedSiblingIndex < owner.childCount)
            {
                Transform sibling = owner.GetChild(trackedSiblingIndex);
                return sibling.GetComponent<BaseAbilityEffect>();
            }
            return null;
        }
    }
}