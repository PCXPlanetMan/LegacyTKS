using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class KnockOutStatusEffect : StatusEffect
    {
        Unit owner;
        Stats stats;

        void Awake()
        {
            owner = GetComponentInParent<Unit>();
            stats = owner.GetComponent<Stats>();
        }

        void OnEnable()
        {
            this.AddObserver(OnTurnCheck, TurnOrderController.TurnCheckNotification, owner);
            this.AddObserver(OnStatCounterWillChange, Stats.WillChangeNotification(EnumStatTypes.CTR), stats);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnTurnCheck, TurnOrderController.TurnCheckNotification, owner);
            this.RemoveObserver(OnStatCounterWillChange, Stats.WillChangeNotification(EnumStatTypes.CTR), stats);
        }

        void OnTurnCheck(object sender, object args)
        {
            // Dont allow a KO'd unit to take turns
            BaseException exc = args as BaseException;
            if (exc.defaultToggle == true)
                exc.FlipToggle();
        }

        void OnStatCounterWillChange(object sender, object args)
        {
            // Dont allow a KO'd unit to increment the turn order counter
            ValueChangeException exc = args as ValueChangeException;
            if (exc.toValue > exc.fromValue)
                exc.FlipToggle();
        }
    }
}