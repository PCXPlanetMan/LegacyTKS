using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class FaceToTargetBehaviour : PlayableBehaviour
{
    public FaceParam.FaceType faceType;
    public Transform targetTransform;
    public Vector2 targetPosition;

    [HideInInspector]
    public bool commandExecuted = false; //the user shouldn't author this, the Mixer does
}
