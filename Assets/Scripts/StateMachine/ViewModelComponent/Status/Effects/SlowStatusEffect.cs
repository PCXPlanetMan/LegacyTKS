using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class SlowStatusEffect : StatusEffect
    {
        Stats myStats;

        void OnEnable()
        {
            myStats = GetComponentInParent<Stats>();
            if (myStats)
                this.AddObserver(OnCounterWillChange, Stats.WillChangeNotification(EnumStatTypes.CTR), myStats);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnCounterWillChange, Stats.WillChangeNotification(EnumStatTypes.CTR), myStats);
        }

        void OnCounterWillChange(object sender, object args)
        {
            ValueChangeException exc = args as ValueChangeException;
            MultDeltaModifier m = new MultDeltaModifier(0, 0.5f);
            exc.AddModifier(m);
        }
    }
}