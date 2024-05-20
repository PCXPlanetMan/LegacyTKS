using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class UndeadAbilityEffectTarget : AbilityEffectTarget
    {
        /// <summary>
        /// Indicates whether the Undead component must be present (true)
        /// or must not be present (false) for the target to be valid.
        /// </summary>
        public bool toggle;

        public override bool IsTarget(EncapsuleTile tile)
        {
            if (tile == null || tile.content == null)
                return false;

            bool hasComponent = tile.content.GetComponent<Undead>() != null;
            if (hasComponent != toggle)
                return false;

            Stats s = tile.content.GetComponent<Stats>();
            return s != null && s[EnumStatTypes.HP] > 0;
        }
    }
}