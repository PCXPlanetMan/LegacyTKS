using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRender;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playMode == EnumEffectPlayMode.AutoClose && curClip != null && !curClip.isLooping)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                animator.Play("None");
                curClip = null;
                playMode = EnumEffectPlayMode.ByAnim;
            }
        }
    }

    public enum EnumEffectPlayMode
    {
        AutoClose,
        ByAnim
    }

    private EnumEffectPlayMode playMode = EnumEffectPlayMode.AutoClose;
    private AnimationClip curClip;
    public float PlayEffect(string strEffectName, EnumEffectPlayMode mode = EnumEffectPlayMode.ByAnim)
    {
        curClip = null;
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.CompareTo(strEffectName) == 0)
            {
                curClip = clip;
                break;
            }
        }
        float fDuration = 0f;
        if (curClip != null)
        {
            animator.Play(strEffectName);
            fDuration = curClip.length;
            playMode = mode;
        }
        else
        {
            Debug.LogErrorFormat("No animation exist : {0}", strEffectName);
        }
        return fDuration;
    }

    public int SpriteOrderInLayer()
    {
        return spriteRender.sortingOrder;
    }

    public void UpdateSpriteOrder(int newOrder)
    {
        spriteRender.sortingOrder = newOrder;
    }
}
