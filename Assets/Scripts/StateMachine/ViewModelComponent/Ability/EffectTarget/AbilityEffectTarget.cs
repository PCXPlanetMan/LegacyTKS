using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public abstract class AbilityEffectTarget : MonoBehaviour
    {
        public abstract bool IsTarget(EncapsuleTile tile);
    }
}