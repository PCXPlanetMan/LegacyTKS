using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubPanelTeam : MonoBehaviour
{
    public Text Win;
    public Text Lose;
    public Text Kills;
    public Text JobsRatio;
    public Text SpecialsRatio;
    public Text Souls;
    public Text Animals;

    public Text Intel;
    public Text Kind;
    public Text Courage;
    public Text Medic;

    public List<UITeamMemberDismiss> ListMembers;

    public UISideListTeams SideListTeams;

    public Transform ComBtnsPanel;

    void OnEnable()
    {
        UpdateSubPanelData();
    }

    private void UpdateSubPanelData()
    {
        var currentDocument = DocumentDataManager.Instance.GetCurrentDocument();
        Intel.text = currentDocument.Intelligence.ToString();
        Kind.text = currentDocument.Morality.ToString();
        Courage.text = currentDocument.Courage.ToString();


        var members = currentDocument.Team;
        if (members != null)
        {
            for (int i = 0; i < members.Count; i++)
            {
                var btn = ListMembers[i];
                var member = members[i];
                var instanceItem = CharactersManager.Instance.GetCharacterInstanceById(member);
                var charRenderInfo = CharactersManager.Instance.GetCharacterResById(instanceItem.AttachResID);
                var charInfo = DocumentDataManager.Instance.FindCharInfoFromCandidates(member);
                btn.UpdateMemberInfo(member, instanceItem.InstanceName, charInfo.Level);
            }
        }
    }

    private void ClearTeamMembersInSettingPanel()
    {
        SideListTeams.UpdateData();

        for (int i = 0; i < ListMembers.Count; i++)
        {
            var btn = ListMembers[i];
            btn.ClearMemberInfo();
        }

        UpdateSubPanelData();
    }

    public void ShowComButtonWhenDismiss(bool bShow)
    {
        ComBtnsPanel.gameObject.SetActive(bShow);
    }

    private int currentDismissCharId = 0;
    public void SetCurrentDismissedIndex(int charId)
    {
        currentDismissCharId = charId;
    }

    public void DismissCurrentSelectdChar()
    {
        //Debug.Log("Click Dismiss index = " + index);
        DocumentDataManager.Instance.DismissCharFromTeam(currentDismissCharId);
        ClearTeamMembersInSettingPanel();
        currentDismissCharId = 0;
    }
}
