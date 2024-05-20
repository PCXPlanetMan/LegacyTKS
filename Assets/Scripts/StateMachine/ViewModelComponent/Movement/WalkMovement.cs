using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class WalkMovement : Movement
    {
        // TODO:战斗地图上的移动速度由移动到下一个Tile所需时间决定
        private readonly float EXP_MOVE_TILE_DURTION = .5f;

        protected override bool ExpandSearch(EncapsuleTile from, EncapsuleTile to)
        {
            // Skip if the distance in height between the two tiles is more than the unit can jump
            if ((Mathf.Abs(from.Height - to.Height) > jumpHeight))
                return false;

            // Skip if the tile is occupied by an enemy
            if (to.content != null)
                return false;

            return base.ExpandSearch(from, to);
        }

        public override IEnumerator Traverse(EncapsuleTile tile)
        {
            unit.Place(tile);

            // Build a list of way points from the unit's 
            // starting tile to the destination tile
            List<EncapsuleTile> targets = new List<EncapsuleTile>();
            while (tile != null)
            {
                targets.Insert(0, tile);
                tile = tile.PrevTile;
            }

            // Move to each way point in succession
            for (int i = 1; i < targets.Count; ++i)
            {
                EncapsuleTile from = targets[i - 1];
                EncapsuleTile to = targets[i];

                EnumStateDirections dir = from.GetDirection(to);
                if (unit.Dir != dir)
                    yield return StartCoroutine(Turn(dir));

                if (from.Height == to.Height)
                    yield return StartCoroutine(Walk(to));
                else
                    yield return StartCoroutine(Jump(to));
            }
            // 移动结束后驱动Render播放Static动画(停下来)
            unit.MoveFormTo(Vector3.zero, Vector3.zero);
            yield return null;
        }

        IEnumerator Walk(EncapsuleTile target)
        {
            float fDuration = EXP_MOVE_TILE_DURTION;
            Vector3 targetPos = target.ConvertTilePosToWorld();
            Unit unit = transform.GetComponent<Unit>();
            Transform parent = unit.transform.parent;
            Tweener tweener = parent.MoveTo(targetPos, fDuration, EasingEquations.Linear);
            // 控制Render在地图上移动
            unit.MoveFormTo(parent.transform.position, targetPos);
            while (tweener != null)
            {
                yield return null;
            }
        }

        IEnumerator Jump(EncapsuleTile to)
        {
            Debug.LogError("Not implement Jump");
            yield return null;
        }
    }
}