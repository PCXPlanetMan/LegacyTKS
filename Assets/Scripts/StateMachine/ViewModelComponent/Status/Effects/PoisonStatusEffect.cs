using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class PoisonStatusEffect : StatusEffect
    {
        Unit owner;

        void OnEnable()
        {
            owner = GetComponentInParent<Unit>();
            if (owner)
                this.AddObserver(OnNewTurn, TurnOrderController.TurnBeganNotification, owner);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnNewTurn, TurnOrderController.TurnBeganNotification, owner);
        }

        void OnNewTurn(object sender, object args)
        {
            Stats s = GetComponentInParent<Stats>();
            int currentHP = s[EnumStatTypes.HP];
            int maxHP = s[EnumStatTypes.MHP];
            int reduce = Mathf.Min(currentHP, Mathf.FloorToInt(maxHP * 0.1f));
            s.SetValue(EnumStatTypes.HP, (currentHP - reduce), false);
        }
    }
}