using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComBtnLearnSkill : MonoBehaviour
{
    public Button BtnOK;
    public Button BtnCancel;

    public UIInfoPanelSkills ParentPanelSkills;

    // Start is called before the first frame update
    void Start()
    {
        BtnOK.GetComponent<UIComBtnLearnSkillPointerHandler>().SetParentComBtn(this);
        BtnCancel.GetComponent<UIComBtnLearnSkillPointerHandler>().SetParentComBtn(this);
        BtnOK.GetComponent<UIComBtnLearnSkillPointerHandler>().IsOK = true;
        BtnCancel.GetComponent<UIComBtnLearnSkillPointerHandler>().IsOK = false;

        BtnOK.onClick.AddListener(OnClickComBtnOK);
        BtnCancel.onClick.AddListener(OnClickComBtnCancel);
    }

    public void HighlightWhichButton(bool isOK)
    {
        if (isOK)
        {
            BtnCancel.GetComponent<UIComBtnLearnSkillPointerHandler>().UnHighLight();
        }
        else
        {
            BtnOK.GetComponent<UIComBtnLearnSkillPointerHandler>().UnHighLight();
        }
    }

    private void OnClickComBtnOK()
    {
        ParentPanelSkills.LearnCurrentSelectedSkill();
        ParentPanelSkills.ShowComButtonWhenLearnSkill(false);
    }

    private void OnClickComBtnCancel()
    {
        ParentPanelSkills.ShowComButtonWhenLearnSkill(false);
    }
}
