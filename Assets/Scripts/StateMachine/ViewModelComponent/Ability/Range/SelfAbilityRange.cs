using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class SelfAbilityRange : AbilityRange
    {
        public override bool positionOriented { get { return false; } }

        public override List<EncapsuleTile> GetTilesInRange(Board board)
        {
            List<EncapsuleTile> retValue = new List<EncapsuleTile>(1);
            retValue.Add(unit.Tile);
            return retValue;
        }
    }
}