using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityPowerParser
{
	public enum EnumAbilityPowerClass
    {
        Invalid = 0,
        None,
        Physical,
        Magical,
        Weapon
    }

    public static EnumAbilityPowerClass ParsePowerClassType(string strContent)
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
                        EnumAbilityPowerClass result = (EnumAbilityPowerClass)Enum.Parse(typeof(EnumAbilityPowerClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumAbilityPowerClass.Invalid;
    }
}