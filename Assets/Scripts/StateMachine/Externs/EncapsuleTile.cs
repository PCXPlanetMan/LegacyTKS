using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 理论上只是Tile数据载体,用于计算寻路等,但由于需要和TileMap坐标进行换算,所以需要记录相关变量
    /// </summary>
    public class EncapsuleTile
    {
        public Vector2Int VecPos { get; private set; }
        private TileBase tileBase;
        public GameObject content;
        public EncapsuleTile PrevTile;
        public int Distance;
        public int Height = 0; // Not used
        private Grid gridMap;

        public EncapsuleTile(Vector2Int vecCellPos, TileBase tile, Grid grid)
        {
            VecPos = vecCellPos;
            tileBase = tile;
            PrevTile = null;
            Distance = 0;
            gridMap = grid;
        }

        public Vector3 ConvertTilePosToWorld()
        {
            Vector3 vecWorld = gridMap.GetCellCenterWorld(new Vector3Int(VecPos.x, VecPos.y, 0));
            return vecWorld;
        }
    }
}