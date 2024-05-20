using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComBtnUseEquip : MonoBehaviour
{
    public Button BtnOK;
    public Button BtnCancel;

    public UIInfoPanelEquipments ParentPanelEquips;

    // Start is called before the first frame update
    void Start()
    {
        BtnOK.GetComponent<UIComBtnUseEquipPointerHandler>().SetParentComBtn(this);
        BtnCancel.GetComponent<UIComBtnUseEquipPointerHandler>().SetParentComBtn(this);
        BtnOK.GetComponent<UIComBtnUseEquipPointerHandler>().IsOK = true;
        BtnCancel.GetComponent<UIComBtnUseEquipPointerHandler>().IsOK = false;

        BtnOK.onClick.AddListener(OnClickComBtnOK);
        BtnCancel.onClick.AddListener(OnClickComBtnCancel);
    }

    public void HighlightWhichButton(bool isOK)
    {
        if (isOK)
        {
            BtnCancel.GetComponent<UIComBtnUseEquipPointerHandler>().UnHighLight();
        }
        else
        {
            BtnOK.GetComponent<UIComBtnUseEquipPointerHandler>().UnHighLight();
        }
    }

    private void OnClickComBtnOK()
    {
        ParentPanelEquips.UseCurrentSelectedEquip();
        ParentPanelEquips.ShowComButtonWhenUseEquip(false);
    }

    private void OnClickComBtnCancel()
    {
        ParentPanelEquips.ShowComButtonWhenUseEquip(false);
    }
}
