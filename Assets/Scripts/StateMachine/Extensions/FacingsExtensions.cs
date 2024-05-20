using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public static class FacingsExtensions
    {
        public static EnumFacings GetFacing(this Unit attacker, Unit target)
        {
            Vector2 targetDirection = target.Dir.GetNormal();
            Vector2 approachDirection = ((Vector2)(target.Tile.VecPos - attacker.Tile.VecPos)).normalized;
            float dot = Vector2.Dot(approachDirection, targetDirection);
            if (dot >= 0.45f)
                return EnumFacings.Back;
            if (dot <= -0.45f)
                return EnumFacings.Front;
            return EnumFacings.Side;
        }
    }
}