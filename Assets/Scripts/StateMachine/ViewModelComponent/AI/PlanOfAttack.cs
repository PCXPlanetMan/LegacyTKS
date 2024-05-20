using UnityEngine;
using System.Collections;
using com.tksr.property;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class PlanOfAttack
    {
        public Ability ability;
        public EnumTargets target;
        public Vector2Int moveLocation;
        public Vector2Int fireLocation;
        public EnumStateDirections attackDirection;
    }
}