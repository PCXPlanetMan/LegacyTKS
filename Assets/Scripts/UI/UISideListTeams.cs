using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.UI;

public class UISideListTeams : MonoBehaviour
{
    public List<UIListTeamMember> Members;
    public UIInfoPanelStatus PanelStatus;
    public UIInfoPanelEquipments PanelEquipments;
    public UIInfoPanelSkills PanelSkills;

    private int curSelectedMember = 0;

    private readonly int MAX_MEMBERS_COUNT = 6;

    void OnEnable()
    {
        UpdateData();
    }

    public void OnClickedMemberButton(int index)
    {
        int oldSelected = curSelectedMember;
        curSelectedMember = index;
        if (oldSelected == curSelectedMember)
        {
            return;
        }

        Members[oldSelected].SetSelected(false);
        Members[curSelectedMember].SetSelected(true);

        var status = Members[curSelectedMember].GetCurrentCharActiveStats();
        PanelStatus.UpdateStatusData(status);
        PanelEquipments.UpdateEquipmentData(status);

        var oldType = PanelSkills.GetCurrentShowUnLearnedSkillType();
        PanelSkills.ResetShowUnLearnedSkillType();
        PanelSkills.UpdateSkillData(status, oldType);
    }


    private void HideAllMembers()
    {
        foreach (var member in Members)
        {
            member.gameObject.SetActive(false);
        }
    }

    public void UpdateData()
    {
        HideAllMembers();

        var foundChars = DocumentDataManager.Instance.FindCharsInfoInTeam();
        for (int i = 0; i < Members.Count; i++)
        {
            GameCharInfo charInfo = null;
            if (i < foundChars.Length)
            {
                charInfo = foundChars[i];
            }
            if (charInfo != null)
            {
                var uiMember = Members[i];
                uiMember.LoadCharInformation(charInfo);
                uiMember.SetSelected(false);
                uiMember.gameObject.SetActive(true);
            }
        }

        if (!(curSelectedMember < MAX_MEMBERS_COUNT && curSelectedMember < foundChars.Length))
        {
            curSelectedMember = 0;
        }
        Members[curSelectedMember].SetSelected(true);
        var dataStats = Members[curSelectedMember].GetCurrentCharActiveStats();
        PanelStatus.UpdateStatusData(dataStats);
        PanelEquipments.UpdateEquipmentData(dataStats);
        var oldType = PanelSkills.GetCurrentShowUnLearnedSkillType();
        PanelSkills.ResetShowUnLearnedSkillType();
        if (oldType == EnumGameSkillMainCategory.Invalid)
        {
            oldType = EnumGameSkillMainCategory.Command;
        }
        PanelSkills.UpdateSkillData(dataStats, oldType);
    }
}
