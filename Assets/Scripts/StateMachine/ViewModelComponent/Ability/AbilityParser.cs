using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class AbilityParser
{
    public static bool ParseAbilityInt(string strContent, string strKey, ref int nResult)
    {
        if (!string.IsNullOrEmpty(strContent) && !string.IsNullOrEmpty(strKey))
        {
            var contents = strContent.Split(';');

            foreach (var content in contents)
            {
                var kv = content.Split(':');
                if (kv.Length == 2)
                {
                    if (kv[0].CompareTo(strKey) == 0)
                    {
                        nResult = int.Parse(kv[1]);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool ParseAbilityFloat(string strContent, string strKey, ref float fResult)
    {
        if (!string.IsNullOrEmpty(strContent) && !string.IsNullOrEmpty(strKey))
        {
            var contents = strContent.Split(';');

            foreach (var content in contents)
            {
                var kv = content.Split(':');
                if (kv.Length == 2)
                {
                    if (kv[0].CompareTo(strKey) == 0)
                    {
                        fResult = float.Parse(kv[1]);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static bool ParseAbilityString(string strContent, string strKey, ref string strResult)
    {
        if (!string.IsNullOrEmpty(strContent) && !string.IsNullOrEmpty(strKey))
        {
            var contents = strContent.Split(';');

            foreach (var content in contents)
            {
                var kv = content.Split(':');
                if (kv.Length == 2)
                {
                    if (kv[0].CompareTo(strKey) == 0)
                    {
                        strResult = kv[1];
                        return true;
                    }
                }
            }
        }
        return false;
    }
}