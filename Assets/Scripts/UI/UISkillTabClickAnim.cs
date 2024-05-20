using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISkillTabClickAnim : MonoBehaviour
{
    private Texture normalTex;
    public Image SourceImg;
    public Animator Anim;

    // Start is called before the first frame update
    void Start()
    {
        if (SourceImg != null)
        {
            normalTex = SourceImg.image;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowClickAnimation(bool bShow)
    {
        if (bShow)
        {
            Anim.Play("Pressed");
        }
        else
        {
            if (normalTex != null)
            {
                SourceImg.image = normalTex;
            }
            Anim.Play("Normal");
        }
    }
}
