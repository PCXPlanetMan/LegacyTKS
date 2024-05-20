using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    public abstract class AbilityArea : MonoBehaviour
    {
        public abstract List<EncapsuleTile> GetTilesInArea(Board board, Vector2Int pos);
    }
}