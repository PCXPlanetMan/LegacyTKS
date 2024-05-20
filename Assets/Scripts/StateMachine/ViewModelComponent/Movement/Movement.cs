using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public abstract class Movement : MonoBehaviour
    {
        public int range { get { return stats[EnumStatTypes.MOV]; } }
        public int jumpHeight { get { return stats[EnumStatTypes.JMP]; } }
        protected Unit unit;
        protected Transform jumper;
        protected Stats stats;

        protected virtual void Awake()
        {
            unit = GetComponent<Unit>();
            jumper = transform.Find("Jumper"); // Not used
        }

        protected virtual void Start()
        {
            stats = GetComponent<Stats>();
        }

        public virtual List<EncapsuleTile> GetTilesInRange(Board board)
        {
            List<EncapsuleTile> retValue = board.Search(unit.Tile, ExpandSearch);
            Filter(retValue);
            return retValue;
        }

        public abstract IEnumerator Traverse(EncapsuleTile tile);

        protected virtual bool ExpandSearch(EncapsuleTile from, EncapsuleTile to)
        {
            return (from.Distance + 1) <= range;
        }

        protected virtual void Filter(List<EncapsuleTile> tiles)
        {
            for (int i = tiles.Count - 1; i >= 0; --i)
                if (tiles[i].content != null)
                    tiles.RemoveAt(i);
        }

        protected virtual IEnumerator Turn(EnumStateDirections dir)
        {
            /*
            TransformLocalEulerTweener t = (TransformLocalEulerTweener)transform.RotateToLocal(dir.ToEuler(), 0.25f, EasingEquations.EaseInOutQuad);

            // When rotating between North and West, we must make an exception so it looks like the unit
            // rotates the most efficient way (since 0 and 360 are treated the same)
            if (Mathf.Approximately(t.startTweenValue.y, 0f) && Mathf.Approximately(t.endTweenValue.y, 270f))
                t.startTweenValue = new Vector3(t.startTweenValue.x, 360f, t.startTweenValue.z);
            else if (Mathf.Approximately(t.startTweenValue.y, 270) && Mathf.Approximately(t.endTweenValue.y, 0))
                t.endTweenValue = new Vector3(t.startTweenValue.x, 360f, t.startTweenValue.z);
            */
            TransformLocalEulerTweener t = (TransformLocalEulerTweener)transform.TileISOToDirection(EasingEquations.Linear);

            unit.Dir = dir;
            unit.RefreshDirection();

            while (t != null)
                yield return null;
        }
    }
}