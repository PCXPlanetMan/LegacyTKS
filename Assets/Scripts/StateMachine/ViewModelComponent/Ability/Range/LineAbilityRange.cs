using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class LineAbilityRange : AbilityRange
    {
        public override bool directionOriented { get { return true; } }

        public override List<EncapsuleTile> GetTilesInRange(Board board)
        {
            Vector2Int startPos = unit.Tile.VecPos;
            Vector2Int endPos;
            List<EncapsuleTile> retValue = new List<EncapsuleTile>();

            switch (unit.Dir)
            {
                case EnumStateDirections.North:
                    endPos = new Vector2Int(startPos.x, board.MaxPos.y);
                    break;
                case EnumStateDirections.East:
                    endPos = new Vector2Int(board.MaxPos.x, startPos.y);
                    break;
                case EnumStateDirections.South:
                    endPos = new Vector2Int(startPos.x, board.MinPos.y);
                    break;
                default: // West
                    endPos = new Vector2Int(board.MinPos.x, startPos.y);
                    break;
            }

            int dist = 0;
            while (startPos != endPos)
            {
                if (startPos.x < endPos.x) startPos.x++;
                else if (startPos.x > endPos.x) startPos.x--;

                if (startPos.y < endPos.y) startPos.y++;
                else if (startPos.y > endPos.y) startPos.y--;

                EncapsuleTile t = board.GetTile(startPos);
                if (t != null && Mathf.Abs(t.Height - unit.Tile.Height) <= vertical)
                    retValue.Add(t);

                dist++;
                if (dist >= horizontal)
                    break;
            }

            return retValue;
        }
    }
}