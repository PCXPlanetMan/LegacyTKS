using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityRangeParser 
{
    public enum EnumAbilityRangeClass
    {
        Invalid = 0,
        Self,
        Line,
        Const,
        Cone,
        Infi
    }

    public static EnumAbilityRangeClass ParseRangeClassType(string strContent)
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
                        EnumAbilityRangeClass result = (EnumAbilityRangeClass)Enum.Parse(typeof(EnumAbilityRangeClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumAbilityRangeClass.Invalid;
    }
}