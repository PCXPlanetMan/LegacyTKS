using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGameRootCanvas : Singleton<UIGameRootCanvas>
{
    public Image Cursor;

    public Image WorldMap;

    public UISaveLoad SaveOrLoad;
    public UIInfoSettings Settings;
    public UILittleGames LittleGames;
    public Transform DialogsPanel;
    public Text TextNodes;
    public UIToastHints ToastHints;
    public UIToastLBForICM ToastForICM;

    public RawImage BlackMask;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (CursorManager.Instance != null)
        {
            CursorManager.Instance.CursorUI = Cursor;
            CursorManager.Instance.ShowCursor(true);
        }
    }

    public enum UI_PANEL_CONTENT
    {
        None = 0,
        MainTitle,
        Save,
        Load,
        InfoSettings_Status,
        InfoSettings_Skills,
        InfoSettings_Items,
        InfoSettings_Equipments,
        InfoSettings_Teams,
        InfoSettings_System,
        GameResult,
        LittleGames_InputName,
        LittleGames_PuzzleBgGua,
        LittleGames_PuzzleHuaRongDao,
    }


    private UI_PANEL_CONTENT _lastContent = UI_PANEL_CONTENT.None;
    private UI_PANEL_CONTENT _curContent = UI_PANEL_CONTENT.None;

    private string curUIParam;

    public RectTransform Title;
    
    public RectTransform Result;

    private EnumGameMode lastGameMode;
    public void ExitUIPanelContent(UI_PANEL_CONTENT content = UI_PANEL_CONTENT.None)
    {
        DisableAllContents();
        _curContent = UI_PANEL_CONTENT.None;
        _lastContent = _curContent;
    }

    public string GetCurrentUIParam()
    {
        return curUIParam;
    }

    public void DisplayUIPanelContent(UI_PANEL_CONTENT content, Action<bool> cb = null, string strParam = null)
    {
        if (_curContent == content)
            return;

        curUIParam = strParam;

        _lastContent = _curContent;
        _curContent = content;
        DisableAllContents();
        switch (content)
        {
            case UI_PANEL_CONTENT.MainTitle:
                {
                    ShowMainTitleUi();
                }
                break;
            case UI_PANEL_CONTENT.Save:
                {
                    ShowSaveLoadUi(false);
                }
                break;
            case UI_PANEL_CONTENT.Load:
                {
                    ShowSaveLoadUi(true);
                }
                break;
            case UI_PANEL_CONTENT.InfoSettings_Status:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabStatus);
            }
                break;
            case UI_PANEL_CONTENT.InfoSettings_Skills:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabSkills);
            }
                break;
            case UI_PANEL_CONTENT.InfoSettings_Items:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabItems);
            }
                break;
            case UI_PANEL_CONTENT.InfoSettings_Equipments:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabEquipments);
            }
                break;
            case UI_PANEL_CONTENT.InfoSettings_Teams:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabTeams);
            }
                break;
            case UI_PANEL_CONTENT.InfoSettings_System:
            {
                ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB.TabSystem);
            }
                break;
            case UI_PANEL_CONTENT.GameResult:
                {
                    ShowGameResultUi();
                }
                break;
            case UI_PANEL_CONTENT.LittleGames_InputName:
                {
                    ShowLittleGameByType(UILittleGames.UI_LITTLE_GAME_TYPE.InputName);
                }
                break;
            case UI_PANEL_CONTENT.LittleGames_PuzzleBgGua:
            {
                    ShowLittleGameByType(UILittleGames.UI_LITTLE_GAME_TYPE.PuzzleBaGua, cb);
            }
                break;
            case UI_PANEL_CONTENT.LittleGames_PuzzleHuaRongDao:
            {
                    ShowLittleGameByType(UILittleGames.UI_LITTLE_GAME_TYPE.PuzzleHuaRongDao, cb);
            }
                break;
        }
    }

    private void DisableAllContents()
    {
        Title.gameObject.SetActive(false);
        SaveOrLoad.gameObject.SetActive(false);
        Result.gameObject.SetActive(false);
        DialogsPanel.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
        TextNodes.gameObject.SetActive(false);
        ToastHints.gameObject.SetActive(false);
        LittleGames.gameObject.SetActive(false);
        ToastForICM.gameObject.SetActive(false);
    }

    private void ShowMainTitleUi()
    {
        Title.gameObject.SetActive(true);
    }

    private void ShowSaveLoadUi(bool bLoad)
    {
        SaveOrLoad.gameObject.SetActive(true);
        if (bLoad)
        {
            SaveOrLoad.UpdateDocumentStatus(UISaveLoad.DocumentStatus.Load);
        }
        else
        {
            SaveOrLoad.UpdateDocumentStatus(UISaveLoad.DocumentStatus.Save);
        }

        SaveOrLoad.FromPanelContent = _lastContent;
    }

    private void ShowGameResultUi()
    {
        Result.gameObject.SetActive(true);
    }

    /// <summary>
    /// 新建游戏:播放剧情动画->....
    /// </summary>
    public void NewGame()
    {
        DisableAllContents();
        WorldMap.gameObject.SetActive(true);
        //ShowBlackMask();
        GameMainManager.Instance.NextSceneId = ResourceUtils.NEW_GAME_INTO_SCENE_ID;
        GameMainManager.Instance.SwitchToNextScene();
    }

    private void ShowInfoSettingsTab(UIInfoSettings.UI_SETTINGS_TAB tabType)
    {
        Settings.gameObject.SetActive(true);
        Settings.OpenTab(tabType);
    }

    public void ShowCutInOutNotes(bool bShow, string strNotes)
    {
        TextNodes.gameObject.SetActive(bShow);
        TextNodes.text = strNotes;
    }

    public void ShowToastHint(bool bShow, string strHint)
    {
        ToastHints.gameObject.SetActive(bShow);
        ToastHints.ShowAToast(strHint);
    }

    public void ShowToastForICM(string strToast)
    {
        ToastForICM.gameObject.SetActive(true);
        ToastForICM.ShowAToast(strToast);
    }

    public void ShowScenarioTextInMiddle(string strText)
    {

    }

    private void ShowLittleGameByType(UILittleGames.UI_LITTLE_GAME_TYPE littleGameType, Action<bool> cb = null)
    {
        LittleGames.gameObject.SetActive(true);
        LittleGames.ShowLittleGame(littleGameType, cb);
    }

    public void ShowBlackMask(bool bShow)
    {
        if (BlackMask)
        {
            BlackMask.gameObject.SetActive(bShow);
        }
    }

    public void AnimShowBlackMaskForDialog(float ratio = 1f)
    {
        StartCoroutine(FuncAnimBlackMask(ratio));
    }

    /// <summary>
    /// 对话中有时会显示一段黑屏,黑屏结束后需要继续启动下一个对话内容
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    private IEnumerator FuncAnimBlackMask(float ratio)
    {
        ShowBlackMask(true);
        if (BlackMask)
        {
            BlackMask.gameObject.GetComponent<Animator>().Play("AnimBlackMaskForDialog");
        }

        var oldGameMode = GameMainManager.Instance.CurGameMode;
        GameMainManager.Instance.CurGameMode = EnumGameMode.BlackMaskForDialog;

        yield return new WaitForSeconds(3f); // AnimBlackMaskForDialog动画的持续时间

        GameMainManager.Instance.CurGameMode = oldGameMode;
        // 模拟启动下一个对话内容
        if (oldGameMode == EnumGameMode.SceneTaskDialog)
        {
            bool bHasAnyDialog = ScenarioManager.Instance.PopScenarioDialogs();
            if (!bHasAnyDialog)
            {
                // 检测物品所有时,由于有后续对话,所以不应该退出
                if (ScenarioManager.Instance.IsCheckingItemsOwner)
                {
                    ScenarioManager.Instance.FinishPostTasks();
                }
                else
                {
                    ScenarioManager.Instance.EndDoingTaskDialogs();
                }
            }
        }

        ShowBlackMask(false);
    }

    public bool RightClickToExitPanel()
    {
        if (_curContent == UI_PANEL_CONTENT.InfoSettings_Status ||
            _curContent == UI_PANEL_CONTENT.InfoSettings_Skills ||
            _curContent == UI_PANEL_CONTENT.InfoSettings_Items ||
            _curContent == UI_PANEL_CONTENT.InfoSettings_Equipments ||
            _curContent == UI_PANEL_CONTENT.InfoSettings_Teams ||
            _curContent == UI_PANEL_CONTENT.InfoSettings_System)
        {
            if (_curContent == UI_PANEL_CONTENT.InfoSettings_Items)
            {
                if (Settings.PanelItems.IsRightMouseOnItemButton())
                {
                    return false;
                }
            }

            if (_curContent == UI_PANEL_CONTENT.InfoSettings_Equipments)
            {
                if (Settings.PanelEquipments.IsRightMouseOnEquipButton())
                {
                    return false;
                }
            }

            ExitUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.None);
            return true;
        }
        return false;
    }
}
