using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class FullAbilityArea : AbilityArea
    {
        public override List<EncapsuleTile> GetTilesInArea(Board board, Vector2Int pos)
        {
            AbilityRange ar = GetComponent<AbilityRange>();
            return ar.GetTilesInRange(board);
        }
    }
}