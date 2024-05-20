using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelTeams : MonoBehaviour
{
    private int curTabIndex = 0;
    private readonly int MAX_TAB_COUNT = 4;

    public Button BtnTeam;
    public Image ImgTeamHighLighted;
    public Button BtnNote;
    public Image ImgNoteHighLighted;
    public Button BtnFame;
    public Image ImgFameHighLighted;
    public Button BtnHistory;
    public Image ImgHistoryHighLighted;

    public UISubPanelTeam Team;
    public UISubPanelNote Note;
    public UISubPanelFame Fame;
    public UISubPanelHistory History;

    void Awake()
    {
        AddButtonListeners();
    }

    void OnEnable()
    {
        UpdatePanelData();
    }

    private void UpdatePanelData()
    {
        UpdateTab(curTabIndex);
    }

    private void AddButtonListeners()
    {
        BtnTeam.onClick.AddListener(OnClickBtnTeam);
        BtnNote.onClick.AddListener(OnClickBtnNote);
        BtnFame.onClick.AddListener(OnClickBtnFame);
        BtnHistory.onClick.AddListener(OnClickBtnHistory);
    }

    private void OnClickBtnTeam()
    {
        if (curTabIndex != 0)
        {
            curTabIndex = 0;
            UpdateTab(curTabIndex);
        }
    }

    private void OnClickBtnNote()
    {
        if (curTabIndex != 1)
        {
            curTabIndex = 1;
            UpdateTab(curTabIndex);
        }
    }

    private void OnClickBtnFame()
    {
        if (curTabIndex != 2)
        {
            curTabIndex = 2;
            UpdateTab(curTabIndex);
        }
    }

    private void OnClickBtnHistory()
    {
        if (curTabIndex != 3)
        {
            curTabIndex = 3;
            UpdateTab(curTabIndex);
        }
    }

    private void UpdateTab(int index)
    {
        ClearAllHighLighted();
        ClearTabContents();

        switch (index)
        {
            case 0:
            {
                ImgTeamHighLighted.gameObject.SetActive(true);
                Team.gameObject.SetActive(true);
            }
                break;
            case 1:
            {
                ImgNoteHighLighted.gameObject.SetActive(true);
                Note.gameObject.SetActive(true);
            }
                break;
            case 2:
            {
                ImgFameHighLighted.gameObject.SetActive(true);
                Fame.gameObject.SetActive(true);
            }
                break;
            case 3:
            {
                ImgHistoryHighLighted.gameObject.SetActive(true);
                History.gameObject.SetActive(true);
            }
                break;
        }
    }

    private void ClearAllHighLighted()
    {
        ImgTeamHighLighted.gameObject.SetActive(false);
        ImgNoteHighLighted.gameObject.SetActive(false);
        ImgFameHighLighted.gameObject.SetActive(false);
        ImgHistoryHighLighted.gameObject.SetActive(false);
    }

    private void ClearTabContents()
    {
        Team.gameObject.SetActive(false);
        Note.gameObject.SetActive(false);
        Fame.gameObject.SetActive(false);
        History.gameObject.SetActive(false);
    }
}
