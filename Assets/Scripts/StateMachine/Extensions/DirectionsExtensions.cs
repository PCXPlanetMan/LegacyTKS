using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public static class DirectionsExtensions
    {
        public static EnumStateDirections GetDirection(this EncapsuleTile t1, EncapsuleTile t2)
        {
            if (t1.VecPos.y < t2.VecPos.y)
                return EnumStateDirections.North;
            if (t1.VecPos.x < t2.VecPos.x)
                return EnumStateDirections.East;
            if (t1.VecPos.y > t2.VecPos.y)
                return EnumStateDirections.South;
            return EnumStateDirections.West;
        }

        public static Vector3 ToEuler(this EnumStateDirections d)
        {
            return new Vector3(0, (int)d * 90, 0);
        }

        public static EnumStateDirections GetDirection(this Vector2Int p)
        {
            if (p.y > 0)
                return EnumStateDirections.North;
            if (p.x > 0)
                return EnumStateDirections.East;
            if (p.y < 0)
                return EnumStateDirections.South;
            return EnumStateDirections.West;
        }

        public static Vector2Int GetNormal(this EnumStateDirections dir)
        {
            switch (dir)
            {
                case EnumStateDirections.North:
                    return new Vector2Int(0, 1);
                case EnumStateDirections.East:
                    return new Vector2Int(1, 0);
                case EnumStateDirections.South:
                    return new Vector2Int(0, -1);
                default: // EnumStateDirections.West:
                    return new Vector2Int(-1, 0);
            }
        }
    }
}