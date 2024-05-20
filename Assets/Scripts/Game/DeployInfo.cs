using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.property;
using UnityEngine;

public class DeployInfo : MonoBehaviour
{
    public EnumDirection CharDirection;
    public EnumAnimAction ActionInScene;
    public float InterPauseTime = 2f;
    public float AnimSpeed = 1f;
    public int DialogSighId;
    public int LoopEndTask;

    public List<EnumDirection> DeployPauseDirections;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
