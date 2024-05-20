using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIComBtnDismissPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UIComBtnDismiss parentComBtn;

    private Image BtnImage;

    [HideInInspector]
    public bool IsOK;

    // Start is called before the first frame update
    void Start()
    {
        BtnImage = this.gameObject.GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BtnImage.color = Color.white;
        if (parentComBtn != null)
        {
            parentComBtn.HighlightWhichButton(IsOK);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void SetParentComBtn(UIComBtnDismiss parent)
    {
        parentComBtn = parent;
    }

    public void UnHighLight()
    {
        Color colorHide = Color.white;
        colorHide.a = 0f;
        BtnImage.color = colorHide;
    }
}
