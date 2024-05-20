using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class StatModifierFeature : Feature
    {
        public EnumStatTypes type;
        public int amount;

        Stats stats
        {
            get
            {
                return _target.GetComponentInParent<Stats>();
            }
        }

        protected override void OnApply()
        {
            stats[type] += amount;
        }

        protected override void OnRemove()
        {
            stats[type] -= amount;
        }
    }
}