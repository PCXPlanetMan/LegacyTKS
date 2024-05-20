using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToastAnimation : MonoBehaviour
{
    public Text txContent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isToast)
        {
            checkAnimTime -= Time.deltaTime;
            if (checkAnimTime <= 0f)
            {
                isToast = false;
                this.gameObject.SetActive(false);
            }
        }
    }

    public string GetToastContent()
    {
        return txContent.text;
    }

    public void UpdateToastContent(string strContent)
    {
        txContent.text = strContent;
    }

    private readonly float TOAST_ANIMATION_DURATION = 2f;
    private bool isToast = false;
    private float checkAnimTime = 0f;
    public void DoToast()
    {
        this.gameObject.SetActive(true);
        isToast = true;
        checkAnimTime = TOAST_ANIMATION_DURATION;
    }
}
