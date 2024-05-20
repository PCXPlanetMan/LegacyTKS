using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 战斗状态机所需要的地图Tiles信息的封装
    /// </summary>
    public class Board
    {
        public Dictionary<Vector2Int, EncapsuleTile> DictTiles = new Dictionary<Vector2Int, EncapsuleTile>();

        public Vector2Int MinPos { get { return minPos; } }
        public Vector2Int MaxPos { get { return maxPos; } }

        private Vector2Int minPos;
        private Vector2Int maxPos;

        private readonly Vector2Int[] dirs = new Vector2Int[4]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        public EncapsuleTile GetTile(Vector2Int p)
        {
            return DictTiles.ContainsKey(p) ? DictTiles[p] : null;
        }

        public List<EncapsuleTile> Search(EncapsuleTile start, Func<EncapsuleTile, EncapsuleTile, bool> addTile)
        {
            List<EncapsuleTile> retValue = new List<EncapsuleTile>();
            retValue.Add(start);

            ClearSearch();
            Queue<EncapsuleTile> checkNext = new Queue<EncapsuleTile>();
            Queue<EncapsuleTile> checkNow = new Queue<EncapsuleTile>();

            start.Distance = 0;
            checkNow.Enqueue(start);

            while (checkNow.Count > 0)
            {
                EncapsuleTile t = checkNow.Dequeue();
                for (int i = 0; i < 4; ++i)
                {
                    EncapsuleTile next = GetTile(t.VecPos + dirs[i]);
                    if (next == null || next.Distance <= t.Distance + 1)
                        continue;

                    if (addTile(t, next))
                    {
                        next.Distance = t.Distance + 1;
                        next.PrevTile = t;
                        checkNext.Enqueue(next);
                        retValue.Add(next);
                    }
                }

                if (checkNow.Count == 0)
                    SwapReference(ref checkNow, ref checkNext);
            }

            return retValue;
        }

        private void ClearSearch()
        {
            foreach (EncapsuleTile t in DictTiles.Values)
            {
                t.PrevTile = null;
                t.Distance = int.MaxValue;
            }
        }

        private void SwapReference(ref Queue<EncapsuleTile> a, ref Queue<EncapsuleTile> b)
        {
            Queue<EncapsuleTile> temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// 从外部读取地图地块数据
        /// </summary>
        /// <param name="DictTiles"></param>
        public void Load(List<EncapsuleTile> listTiles)
        {
            DictTiles.Clear();
            minPos = new Vector2Int(int.MaxValue, int.MaxValue);
            maxPos = new Vector2Int(int.MinValue, int.MinValue);

            for (int i = 0; i < listTiles.Count; ++i)
            {
                var tile = listTiles[i];
                DictTiles.Add(tile.VecPos, tile);

                minPos.x = Mathf.Min(minPos.x, tile.VecPos.x);
                minPos.y = Mathf.Min(minPos.y, tile.VecPos.y);
                maxPos.x = Mathf.Max(maxPos.x, tile.VecPos.x);
                maxPos.y = Mathf.Max(maxPos.y, tile.VecPos.y);
            }
        }
    }
}