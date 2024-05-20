using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioDynCollider : MonoBehaviour
{
    [Header("Colliders")]
    public string ColliderName;
    public Collider2D Collider;

    [Header("Collider Animations")] 
    public List<Animator> AttachedAnims;
}
