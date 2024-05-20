using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamMemberDismiss : MonoBehaviour
{
    public Text TxMemberName;
    public Text TxMemberLv;

    private int currentMemberId = 0;

    public UISubPanelTeam ParentPanelTeam;

    private readonly int MAX_MEMBER_COUNT = 5;

    public void UpdateMemberInfo(int memberId, string name, int lv)
    {
        currentMemberId = memberId;
        TxMemberName.text = name;
        TxMemberLv.text = string.Format(ResourceUtils.FORMAT_TEAM_MEMBER_LV, lv.ToString());
    }

    public void ClearMemberInfo()
    {
        currentMemberId = 0;
        TxMemberName.text = string.Empty;
        TxMemberLv.text = string.Empty;
    }

    public void OnClickDismissButton(int index)
    {
        if (index >= 0 && index < MAX_MEMBER_COUNT)
        {
            if (currentMemberId != 0)
            {
                ParentPanelTeam.SetCurrentDismissedIndex(currentMemberId);
                ParentPanelTeam.ShowComButtonWhenDismiss(true);
            }
        }
    }
}
