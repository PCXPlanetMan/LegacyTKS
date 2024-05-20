using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComBtnUseItem : MonoBehaviour
{
    public Button BtnOK;
    public Button BtnCancel;

    public UIInfoPanelItems ParentPanelItems;

    // Start is called before the first frame update
    void Start()
    {
        BtnOK.GetComponent<UIComBtnUseItemPointerHandler>().SetParentComBtn(this);
        BtnCancel.GetComponent<UIComBtnUseItemPointerHandler>().SetParentComBtn(this);
        BtnOK.GetComponent<UIComBtnUseItemPointerHandler>().IsOK = true;
        BtnCancel.GetComponent<UIComBtnUseItemPointerHandler>().IsOK = false;

        BtnOK.onClick.AddListener(OnClickComBtnOK);
        BtnCancel.onClick.AddListener(OnClickComBtnCancel);
    }

    public void HighlightWhichButton(bool isOK)
    {
        if (isOK)
        {
            BtnCancel.GetComponent<UIComBtnUseItemPointerHandler>().UnHighLight();
        }
        else
        {
            BtnOK.GetComponent<UIComBtnUseItemPointerHandler>().UnHighLight();
        }
    }

    private void OnClickComBtnOK()
    {
        ParentPanelItems.UseCurrentSelectedItem();
        ParentPanelItems.ShowComButtonWhenUseItem(false);
    }

    private void OnClickComBtnCancel()
    {
        ParentPanelItems.ShowComButtonWhenUseItem(false);
    }
}
