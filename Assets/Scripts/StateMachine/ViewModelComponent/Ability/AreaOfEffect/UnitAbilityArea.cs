using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class UnitAbilityArea : AbilityArea
    {
        public override List<EncapsuleTile> GetTilesInArea(Board board, Vector2Int pos)
        {
            List<EncapsuleTile> retValue = new List<EncapsuleTile>();
            EncapsuleTile tile = board.GetTile(pos);
            if (tile != null)
                retValue.Add(tile);
            return retValue;
        }
    }
}