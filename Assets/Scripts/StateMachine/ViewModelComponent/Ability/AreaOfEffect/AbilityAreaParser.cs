using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityAreaParser
{
    public enum EnumAbilityAreaClass
    {
        Invalid = 0,
        Full,
        Specify,
        Unit
    }

    public static EnumAbilityAreaClass ParseAreaClassType(string strContent)
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
                        EnumAbilityAreaClass result = (EnumAbilityAreaClass)Enum.Parse(typeof(EnumAbilityAreaClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumAbilityAreaClass.Invalid;
    }
}