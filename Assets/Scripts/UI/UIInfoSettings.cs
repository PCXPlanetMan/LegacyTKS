using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoSettings : MonoBehaviour
{
    public UIBtnSimpleHighlightAnim BtnStatus;
    public UIBtnSimpleHighlightAnim BtnSkills;
    public UIBtnSimpleHighlightAnim BtnItems;
    public UIBtnSimpleHighlightAnim BtnEquipments;
    public UIBtnSimpleHighlightAnim BtnTeams;
    public UIBtnSimpleHighlightAnim BtnSystem;

    public UIInfoPanelStatus PanelStatus;
    public UIInfoPanelSkills PanelSkills;
    public UIInfoPanelItems PanelItems;
    public UIInfoPanelEquipments PanelEquipments;
    public UIInfoPanelTeams PanelTeams;
    public UIInfoPanelSystem PanelSystem;

    public enum UI_SETTINGS_TAB
    {
        TabInvalid,
        TabStatus,
        TabSkills,
        TabItems,
        TabEquipments,
        TabTeams,  
        TabSystem
    }

    // Start is called before the first frame update
    void Awake()
    {
        AddBtnListenersForTabs();
    }

    private void AddBtnListenersForTabs()
    {
        BtnStatus.GetComponent<Button>().onClick.AddListener(OnClickTabStatus);
        BtnSkills.GetComponent<Button>().onClick.AddListener(OnClickTabSkills);
        BtnItems.GetComponent<Button>().onClick.AddListener(OnClickTabItems);
        BtnEquipments.GetComponent<Button>().onClick.AddListener(OnClickTabEquipments);
        BtnTeams.GetComponent<Button>().onClick.AddListener(OnClickTabTeams);
        BtnSystem.GetComponent<Button>().onClick.AddListener(OnClickTabSystem);
    }

    private UI_SETTINGS_TAB _currentTabType = UI_SETTINGS_TAB.TabInvalid;

    public void OpenTab(UI_SETTINGS_TAB tabType)
    {
        if (_currentTabType != tabType)
            _currentTabType = tabType;
        else
        {
            return;
        }

        UnHighlightTabs();
        HideAllPanels();
        switch (tabType)
        {
            case UI_SETTINGS_TAB.TabStatus:
            {
                BtnStatus.ShowHighlighted(true);
                PanelStatus.gameObject.SetActive(true);
            }
                break;
            case UI_SETTINGS_TAB.TabSkills:
            {
                BtnSkills.ShowHighlighted(true);
                PanelSkills.gameObject.SetActive(true);
            }
                break;
            case UI_SETTINGS_TAB.TabItems:
            {
                BtnItems.ShowHighlighted(true);
                PanelItems.gameObject.SetActive(true);
            }
                break;
            case UI_SETTINGS_TAB.TabEquipments:
            {
                BtnEquipments.ShowHighlighted(true);
                PanelEquipments.gameObject.SetActive(true);
            }
                break;
            case UI_SETTINGS_TAB.TabTeams:
            {
                BtnTeams.ShowHighlighted(true);
                PanelTeams.gameObject.SetActive(true);
            }
                break;
            case UI_SETTINGS_TAB.TabSystem:
            {
                BtnSystem.ShowHighlighted(true);
                PanelSystem.gameObject.SetActive(true);
            }
                break;
            default:
                break;
        }
    }

    private void UnHighlightTabs()
    {
        BtnStatus.ShowHighlighted(false);
        BtnSkills.ShowHighlighted(false);
        BtnItems.ShowHighlighted(false);
        BtnEquipments.ShowHighlighted(false);
        BtnTeams.ShowHighlighted(false);
        BtnSystem.ShowHighlighted(false);
    }

    private void HideAllPanels()
    {
        PanelStatus.gameObject.SetActive(false);
        PanelSkills.gameObject.SetActive(false);
        PanelItems.gameObject.SetActive(false);
        PanelEquipments.gameObject.SetActive(false);
        PanelTeams.gameObject.SetActive(false);
        PanelSystem.gameObject.SetActive(false);
    }

    private void OnClickTabStatus()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_Status);
    }

    private void OnClickTabSkills()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_Skills);
    }

    private void OnClickTabItems()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_Items);
    }

    private void OnClickTabEquipments()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_Equipments);
    }

    private void OnClickTabTeams()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_Teams);
    }

    private void OnClickTabSystem()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_System);
    }
}
