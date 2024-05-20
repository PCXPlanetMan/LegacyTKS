using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBtnSimpleHighlightAnim : MonoBehaviour
{
    public Image ImageHighlighted;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void ShowHighlighted(bool bHighlighted)
    {
        if (ImageHighlighted != null)
        {
            ImageHighlighted.gameObject.SetActive(bHighlighted);
        }
    }
}
