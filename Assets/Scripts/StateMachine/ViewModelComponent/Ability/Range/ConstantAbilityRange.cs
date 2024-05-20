using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class ConstantAbilityRange : AbilityRange
    {
        public override List<EncapsuleTile> GetTilesInRange(Board board)
        {
            return board.Search(unit.Tile, ExpandSearch);
        }

        bool ExpandSearch(EncapsuleTile from, EncapsuleTile to)
        {
            return (from.Distance + 1) <= horizontal && Mathf.Abs(to.Height - unit.Tile.Height) <= vertical;
        }
    }
}