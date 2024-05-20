using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class InfiniteAbilityRange : AbilityRange
    {
        public override bool positionOriented { get { return false; } }

        public override List<EncapsuleTile> GetTilesInRange(Board board)
        {
            return new List<EncapsuleTile>(board.DictTiles.Values);
        }
    }
}