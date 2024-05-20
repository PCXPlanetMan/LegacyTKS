using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class HitRateParser
{
	public enum EnumHitRateClass
    {
        Invalid = 0,
        A,
        S,
        Full
    }

    public static EnumHitRateClass ParseHitRateClassType(string strContent)
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
                        EnumHitRateClass result = (EnumHitRateClass)Enum.Parse(typeof(EnumHitRateClass), kv[1]);
                        return result;
                    }
                }
            }
        }
        return EnumHitRateClass.Invalid;
    }
}