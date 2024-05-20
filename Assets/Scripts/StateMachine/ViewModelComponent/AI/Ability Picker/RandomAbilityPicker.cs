using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class RandomAbilityPicker : BaseAbilityPicker
    {
        public List<BaseAbilityPicker> pickers;

        public override void Pick(PlanOfAttack plan)
        {
            int index = Random.Range(0, pickers.Count);
            BaseAbilityPicker p = pickers[index];
            p.Pick(plan);
        }
    }
}