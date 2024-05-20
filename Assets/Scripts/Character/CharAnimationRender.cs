using com.tksr.data;
using com.tksr.property;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharAnimationRender : MonoBehaviour
{
    private Dictionary<EnumDirection, string> dictStaticDirections;
    private Dictionary<EnumDirection, string> dictRunDirections;
    private Dictionary<EnumDirection, string> dictAttackDirections;


    private Animator animator;
    private int lastDirection;
    private EnumAnimAction lastAnimAction = EnumAnimAction.Static;

    private SpriteRenderer spriteRender;

    public bool IsSupportFullDirection;
    public bool IsNoActionWhenSpeak = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();

        if (IsSupportFullDirection)
        {
            isFullDirections = true;
            dictStaticDirections = AnimationDirectionData.EightStaticDirections;
            dictRunDirections = AnimationDirectionData.EightRunDirections;
        }
        else
        {
            isFullDirections = false;
            dictStaticDirections = AnimationDirectionData.FourStaticDirections;
            dictRunDirections = AnimationDirectionData.FourRunDirections;
        }
        dictAttackDirections = null;
    }

    private bool isFullDirections = false; // 8方向

    public void ParseAnimationDictInBattle(EnumWeaponType weaponType)
    {
        isFullDirections = false;

        switch (weaponType)
        {
            case EnumWeaponType.None:
                {
                    dictStaticDirections = AnimationDirectionData.StaticNoneDirections;
                    dictRunDirections = AnimationDirectionData.RunNoneDirections;
                    dictAttackDirections = AnimationDirectionData.AttackNoneDirections;
                }
                break;
            case EnumWeaponType.Sword:
                {
                    dictStaticDirections = AnimationDirectionData.StaticSwordDirections;
                    dictRunDirections = AnimationDirectionData.RunSwordDirections;
                    dictAttackDirections = AnimationDirectionData.AttackSwordDirections;
                }
                break;
            case EnumWeaponType.Blade:
                {
                    dictStaticDirections = AnimationDirectionData.StaticBladeDirections;
                    dictRunDirections = AnimationDirectionData.RunBladeDirections;
                    dictAttackDirections = AnimationDirectionData.AttackBladeDirections;
                }
                break;
            default:
                {
                    Debug.LogErrorFormat("Worng Weapon Type, no animation. {0}", (int)weaponType);
                }
                break;
        }
    }

    /// <summary>
    /// 专用于世界地图中的人物移动控制 
    /// </summary>
    /// <param name="vecDirection"></param>
    public void WorldUpdateDirection(Vector2 vecDirection)
    {
        EnumDirection direction = (EnumDirection)Enum.ToObject(typeof(EnumDirection), lastDirection);
        EnumAnimAction animAction = EnumAnimAction.Static;

        //measure the magnitude of the input.
        if (vecDirection.magnitude > .01f)
        {
            animAction = EnumAnimAction.Run;
            //we can calculate which direction we are going in
            //use DirectionToIndex to get the index of the slice from the direction vector
            //save the answer to lastDirection
            if (isFullDirections)
            {
                lastDirection = DirectionToIndexEight(vecDirection);
                direction = (EnumDirection)Enum.ToObject(typeof(EnumDirection), lastDirection);

            }
            else
            {
                lastDirection = DirectionToIndexFour(vecDirection);
                lastDirection = lastDirection * 2 + 1;
                direction = (EnumDirection)Enum.ToObject(typeof(EnumDirection), lastDirection);
            }
        }

        SetDirection(animAction, direction);
    }

    public EnumDirection CalcDirectionByVector(Vector2 vecDirection)
    {
        EnumDirection direction;
        if (isFullDirections)
        {
            int curDirection = DirectionToIndexEight(vecDirection);
            direction = (EnumDirection)Enum.ToObject(typeof(EnumDirection), curDirection);

        }
        else
        {
            int curDirection = DirectionToIndexFour(vecDirection);
            curDirection = curDirection * 2 + 1;
            direction = (EnumDirection)Enum.ToObject(typeof(EnumDirection), curDirection);
        }
        return direction;
    }

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndexEight(Vector2 dir)
    {
        int sliceCount = 8;
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);

        //add the halfslice offset
        angle += halfstep;

        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0)
        {
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndexFour(Vector2 dir)
    {
        int sliceCount = 4;
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);

        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0)
        {
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }

    //this function converts a string array to a int (animator hash) array.
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        //allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        //loop through the string array
        for (int i = 0; i < animationArray.Length; i++)
        {
            //do the hash and save it to our hash array
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        //we're done!
        return hashArray;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    private EnumDirection oldAnimDirection;
    private EnumAnimAction oldAnimAction;
    private float oldAnimSpeed = 1f;

    /// <summary>
    /// 当人物处于特殊的动画时,对话将无须面向主角
    /// </summary>
    /// <returns></returns>
    public bool SaveOldAnimationData()
    {
        oldAnimDirection = GetLastDirection();
        oldAnimAction = GetLastAnimAction();
        if (animator != null)
        {
            oldAnimSpeed = animator.speed;
        }

        if (IsNoActionWhenSpeak)
        {
            return true;
        }

        if (oldAnimAction == EnumAnimAction.Drinking || oldAnimAction == EnumAnimAction.Trapped || oldAnimAction == EnumAnimAction.Drunk || oldAnimAction == EnumAnimAction.Dance)
        {
            return true;
        }

        return false;
    }

    public void LoadOldAnimationData()
    {
        SetDirection(oldAnimAction, oldAnimDirection, oldAnimSpeed);
    }

    public float SetDirection(EnumAnimAction animAction, EnumDirection direction, float speed = 1f)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarningFormat("Animator not valid");
            return 0f;
        }

        float oldAnimSpeed = animator.speed;
        animator.speed = speed;

        Dictionary<EnumDirection, string> dict = null;
        bool bSpecialAnim = false;
        if (animAction == EnumAnimAction.Static)
        {
            dict = dictStaticDirections;
        }
        else if (animAction == EnumAnimAction.Run)
        {
            dict = dictRunDirections;
        }
        else if (animAction == EnumAnimAction.Attack)
        {
            dict = dictAttackDirections;
        }
        else if (animAction == EnumAnimAction.Idle 
                 || animAction == EnumAnimAction.Speak 
                 || animAction == EnumAnimAction.Angry
                 || animAction == EnumAnimAction.Scolded
                 || animAction == EnumAnimAction.Disappear
                 || animAction == EnumAnimAction.Trapped
                 || animAction == EnumAnimAction.Cry
                 || animAction == EnumAnimAction.Sleep
                 || animAction == EnumAnimAction.Drinking
                 || animAction == EnumAnimAction.Drunk
                 || animAction == EnumAnimAction.Dance)
        {
            bSpecialAnim = true;
        }
        else
        {
            Debug.LogError("No Supported Animation Action");
        }

        EnumAnimAction oldLastAnimAction = lastAnimAction;
        int oldLastDirection = lastDirection;
        if (dict != null || bSpecialAnim)
        {
            lastAnimAction = animAction;
            lastDirection = (int)direction;
            string strAnim = string.Empty;
            if (bSpecialAnim)
            {
                strAnim = animAction.ToString();
                string strSpecialAnim = string.Format("{0} {1}", animAction, direction.ToString());
                if (animAction == EnumAnimAction.Disappear)
                {
                    strSpecialAnim = string.Format("{0}", animAction);
                }
                if (animator.HasState(0, Animator.StringToHash(strSpecialAnim)))
                {
                    strAnim = strSpecialAnim;
                }
            }
            else
            {
                if (dict.ContainsKey(direction))
                {
                    strAnim = dict[direction];
                }
            }
            
            if (!string.IsNullOrEmpty(strAnim))
            { 
                if (animator.HasState(0, Animator.StringToHash(strAnim)))
                {
                    animator.Play(strAnim);

                    // 获取当前动画时长
                    float length = 0f;
                    if (animator.runtimeAnimatorController != null)
                    {
                        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                        foreach (AnimationClip clip in clips)
                        {
                            if (clip.name.Equals(strAnim))
                            {
                                length = clip.length;
                                break;
                            }
                        }
                    }
                    return length;
                }
                else
                {
                    Debug.LogError("Not found Animation in animator");
                }
            }
        }
        else
        {
            Debug.LogError("No support Animation Action now");
        }

        return 0f;
    }

    public EnumDirection GetLastDirection()
    {
        return (EnumDirection)Enum.ToObject(typeof(EnumDirection), lastDirection);
    }

    public EnumAnimAction GetLastAnimAction()
    {
        return lastAnimAction;
    }

    /// <summary>
    /// 根据当前人物的方向确定人物的朝向(左中右)
    /// </summary>
    /// <returns></returns>
    public EnumOrientation GetCharOrientation()
    {
        EnumOrientation orient = EnumOrientation.Center;
        var lastDirection = GetLastDirection();
        if (lastDirection == EnumDirection.E || lastDirection == EnumDirection.NE || lastDirection == EnumDirection.SE) // right side
        {
            orient = EnumOrientation.Right;
        }
        else if (lastDirection == EnumDirection.W || lastDirection == EnumDirection.NW || lastDirection == EnumDirection.SW) // left side
        {
            orient = EnumOrientation.Left;
        }
        else // middle
        {
            orient = EnumOrientation.Center;
        }
        return orient;
    }

    public void SetDisplayAlpha(float fAlpha)
    {
        if (spriteRender != null)
        {
            fAlpha = Mathf.Clamp(fAlpha, 0f, 1f);
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, fAlpha);
        }
    }

    void Update()
    {
        // TODO:攻击动画结束后停留在最后一帧,然后再切换成Idle状态才合理?
        //if (lastAnimAction == EnumAnimAction.Attack)
        //{
        //    EnumDirection direction = (EnumDirection) lastDirection;
        //    if (dictAttackDirections.ContainsKey(direction))
        //    {
        //        string strAnim = dictAttackDirections[direction];
        //        if (animator.GetCurrentAnimatorStateInfo(0).IsName(strAnim) == true)
        //        {
        //            // TODO:普通攻击动画结束后停留一下,在动画中直接制作？
        //            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //            {
        //                WorldUpdateDirection(Vector2.zero);
        //            }
        //        }
        //    }
        //}
    }

    public Animator GetCurrentAnimator()
    {
        return animator;
    }
}
