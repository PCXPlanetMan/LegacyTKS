using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class DefaultAbilityEffectTarget : AbilityEffectTarget
    {
        public override bool IsTarget(EncapsuleTile tile)
        {
            if (tile == null || tile.content == null)
                return false;

            Stats s = tile.content.GetComponent<Stats>();
            return s != null && s[EnumStatTypes.HP] > 0;
        }
    }
}