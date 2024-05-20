using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public class FullTypeHitRate : HitRate
    {
        public override bool IsAngleBased { get { return false; } }

        public override int Calculate(EncapsuleTile target)
        {
            Unit defender = target.content.GetComponent<Unit>();
            if (AutomaticMiss(defender))
                return Final(100);

            return Final(0);
        }
    }
}