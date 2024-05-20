using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityEffectTargetParser
{
	public enum EnumAbilityEffectTargetClass
    {
        Invalid = 0,
        Default,
        AbsorbDamage,
        Enemy,
        KOd,
        Undead
    }

    public static EnumAbilityEffectTargetClass ParseAbilityEffectTargetClassType(string strContent)
    {
        if (!string.IsNullOrEmpty(strContent))
        {
            var contents = strContent.Split(';');

            foreach (var content in contents)
            {
                var kv = content.Split(':');
                if (kv.Length == 2)
                {
                    if (kv[0].CompareTo("Type") == 0)
                    {
                        EnumAbilityEffectTargetClass result = (EnumAbilityEffectTargetClass)Enum.Parse(typeof(EnumAbilityEffectTargetClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumAbilityEffectTargetClass.Invalid;
    }
}