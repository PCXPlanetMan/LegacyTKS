using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtnShowSkillInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Text TextName;
    public Text TextCount;

    [HideInInspector]
    public bool IsPointerEnter = false;

    [HideInInspector] public int SkillID = 0;
    [HideInInspector] public int BtnIndex = -1;

    public UIInfoPanelSkills ParentInfoPanelSkills;
    //public UIItemsContentContainer ParentContentItems;

    // Start is called before the first frame update
    void Start()
    {
        Background.enabled = false;
        TextName.color = Color.white;
        TextCount.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInLearning)
            return;

        Background.enabled = true;
        TextName.color = Color.red;
        TextCount.color = Color.red;
        IsPointerEnter = true;

        ParentInfoPanelSkills.ShowSkillDetails(SkillID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInLearning)
            return;
        Background.enabled = false;
        TextName.color = Color.white;
        TextCount.color = Color.white;
        IsPointerEnter = false;

        ParentInfoPanelSkills.ShowSkillDetails(0);
    }

    private bool isShowingTips = false;
    [HideInInspector]
    public bool isInLearning = false;

    // TODO:鼠标右键点击到Item中间的空隙时会导致直接关闭UI
    void Update()
    {
        if (IsPointerEnter)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (ParentInfoPanelSkills != null)
                {
                    ParentInfoPanelSkills.SetCurrentInLearnSkillIndex(BtnIndex);
                    ParentInfoPanelSkills.ShowComButtonWhenLearnSkill(true);

                    isInLearning = true;

                    Background.enabled = true;
                    TextName.color = Color.red;
                    TextCount.color = Color.red;
                }
            }
        }
    }

    public void UnLearnSkill()
    {
        isInLearning = false;
        IsPointerEnter = false;
    }
}
