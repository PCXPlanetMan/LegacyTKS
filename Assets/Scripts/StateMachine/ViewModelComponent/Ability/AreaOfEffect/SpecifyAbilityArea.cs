using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public class SpecifyAbilityArea : AbilityArea
    {
        public int horizontal;
        public int vertical;
        EncapsuleTile tile;

        public override List<EncapsuleTile> GetTilesInArea(Board board, Vector2Int pos)
        {
            tile = board.GetTile(pos);
            return board.Search(tile, ExpandSearch);
        }

        bool ExpandSearch(EncapsuleTile from, EncapsuleTile to)
        {
            return (from.Distance + 1) <= horizontal && Mathf.Abs(to.Height - tile.Height) <= vertical;
        }
    }
}