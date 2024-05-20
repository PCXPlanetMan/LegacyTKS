using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityEffectParser
{
	public enum EnumAbilityEffectClass
    {
        Invalid = 0,
        Damage,
        Esuna,
        Heal,
        Inflict,
        Revive
    }

    public static EnumAbilityEffectClass ParseAbilityEffectClassType(string strContent)
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
                        EnumAbilityEffectClass result = (EnumAbilityEffectClass)Enum.Parse(typeof(EnumAbilityEffectClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumAbilityEffectClass.Invalid;
    }
}