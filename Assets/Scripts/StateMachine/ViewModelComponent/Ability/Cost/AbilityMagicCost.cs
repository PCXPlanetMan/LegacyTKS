using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class AbilityMagicCost : MonoBehaviour
    {
        public int amount;
        Ability owner;

        void Awake()
        {
            owner = GetComponent<Ability>();
        }

        void OnEnable()
        {
            this.AddObserver(OnCanPerformCheck, Ability.CanPerformCheck, owner);
            this.AddObserver(OnDidPerformNotification, Ability.DidPerformNotification, owner);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnCanPerformCheck, Ability.CanPerformCheck, owner);
            this.RemoveObserver(OnDidPerformNotification, Ability.DidPerformNotification, owner);
        }

        void OnCanPerformCheck(object sender, object args)
        {
            Stats s = GetComponentInParent<Stats>();
            if (s[EnumStatTypes.MP] < amount)
            {
                BaseException exc = (BaseException)args;
                exc.FlipToggle();
            }
        }

        void OnDidPerformNotification(object sender, object args)
        {
            Stats s = GetComponentInParent<Stats>();
            s[EnumStatTypes.MP] -= amount;
        }
    }
}