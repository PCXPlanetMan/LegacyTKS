using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComBtnDismiss : MonoBehaviour
{
    public Button BtnOK;
    public Button BtnCancel;

    public UISubPanelTeam ParentSubPanelTeam;

    // Start is called before the first frame update
    void Start()
    {
        BtnOK.GetComponent<UIComBtnDismissPointerHandler>().SetParentComBtn(this);
        BtnCancel.GetComponent<UIComBtnDismissPointerHandler>().SetParentComBtn(this);
        BtnOK.GetComponent<UIComBtnDismissPointerHandler>().IsOK = true;
        BtnCancel.GetComponent<UIComBtnDismissPointerHandler>().IsOK = false;

        BtnOK.onClick.AddListener(OnClickComBtnOK);
        BtnCancel.onClick.AddListener(OnClickComBtnCancel);
    }

    public void HighlightWhichButton(bool isOK)
    {
        if (isOK)
        {
            BtnCancel.GetComponent<UIComBtnDismissPointerHandler>().UnHighLight();
        }
        else
        {
            BtnOK.GetComponent<UIComBtnDismissPointerHandler>().UnHighLight();
        }
    }

    private void OnClickComBtnOK()
    {
        ParentSubPanelTeam.DismissCurrentSelectdChar();
        ParentSubPanelTeam.ShowComButtonWhenDismiss(false);
    }

    private void OnClickComBtnCancel()
    {
        ParentSubPanelTeam.ShowComButtonWhenDismiss(false);
    }
}
