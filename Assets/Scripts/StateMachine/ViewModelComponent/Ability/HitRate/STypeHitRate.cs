using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class STypeHitRate : HitRate
    {
        public override int Calculate(EncapsuleTile target)
        {
            Unit defender = target.content.GetComponent<Unit>();
            if (AutomaticMiss(defender))
                return Final(100);

            if (AutomaticHit(defender))
                return Final(0);

            int res = GetResistance(defender);
            res = AdjustForStatusEffects(defender, res);
            res = AdjustForRelativeFacing(defender, res);
            res = Mathf.Clamp(res, 0, 100);
            return Final(res);
        }

        int GetResistance(Unit target)
        {
            Stats s = target.GetComponentInParent<Stats>();
            return s[EnumStatTypes.RES];
        }

        int AdjustForRelativeFacing(Unit target, int rate)
        {
            switch (attacker.GetFacing(target))
            {
                case EnumFacings.Front:
                    return rate;
                case EnumFacings.Side:
                    return rate - 10;
                default:
                    return rate - 20;
            }
        }
    }
}