using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevDebugMask : Singleton<DevDebugMask>
{
    void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}
