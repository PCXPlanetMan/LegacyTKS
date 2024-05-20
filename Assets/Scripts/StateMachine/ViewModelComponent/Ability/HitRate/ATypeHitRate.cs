using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class ATypeHitRate : HitRate
    {
        private readonly int MIN_EVADE_THRESHOLD = 5;
        private readonly int MAX_EVADE_THRESHOLD = 95;

        public override int Calculate(EncapsuleTile target)
        {
            Unit defender = target.content.GetComponent<Unit>();
            if (AutomaticHit(defender))
                return Final(0);

            if (AutomaticMiss(defender))
                return Final(100);

            int evade = GetEvade(defender);
            evade = AdjustForRelativeFacing(defender, evade);
            evade = AdjustForStatusEffects(defender, evade);
            evade = Mathf.Clamp(evade, MIN_EVADE_THRESHOLD, MAX_EVADE_THRESHOLD);
            return Final(evade);
        }

        int GetEvade(Unit target)
        {
            Stats s = target.GetComponentInParent<Stats>();
            return Mathf.Clamp(s[EnumStatTypes.EVD], 0, 100);
        }

        int AdjustForRelativeFacing(Unit target, int rate)
        {
            // TKS暂时不考虑攻击朝向而影响命中率的问题
            return rate;

            //switch (attacker.GetFacing(target))
            //{
            //    case EnumFacings.Front:
            //        return rate;
            //    case EnumFacings.Side:
            //        return rate / 2;
            //    default:
            //        return rate / 4;
            //}
        }
    }
}