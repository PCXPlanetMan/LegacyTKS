using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class ConeAbilityRange : AbilityRange
    {
        public override bool directionOriented { get { return true; } }

        public override List<EncapsuleTile> GetTilesInRange(Board board)
        {
            Vector2Int pos = unit.Tile.VecPos;
            List<EncapsuleTile> retValue = new List<EncapsuleTile>();
            int dir = (unit.Dir == EnumStateDirections.North || unit.Dir == EnumStateDirections.East) ? 1 : -1;
            int lateral = 1;

            if (unit.Dir == EnumStateDirections.North || unit.Dir == EnumStateDirections.South)
            {
                for (int y = 1; y <= horizontal; ++y)
                {
                    int min = -(lateral / 2);
                    int max = (lateral / 2);
                    for (int x = min; x <= max; ++x)
                    {
                        Vector2Int next = new Vector2Int(pos.x + x, pos.y + (y * dir));
                        EncapsuleTile tile = board.GetTile(next);
                        if (ValidTile(tile))
                            retValue.Add(tile);
                    }
                    lateral += 2;
                }
            }
            else
            {
                for (int x = 1; x <= horizontal; ++x)
                {
                    int min = -(lateral / 2);
                    int max = (lateral / 2);
                    for (int y = min; y <= max; ++y)
                    {
                        Vector2Int next = new Vector2Int(pos.x + (x * dir), pos.y + y);
                        EncapsuleTile tile = board.GetTile(next);
                        if (ValidTile(tile))
                            retValue.Add(tile);
                    }
                    lateral += 2;
                }
            }

            return retValue;
        }

        bool ValidTile(EncapsuleTile t)
        {
            return t != null && Mathf.Abs(t.Height - unit.Tile.Height) <= vertical;
        }
    }
}