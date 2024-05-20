using com.tksr.document;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using com.tksr.property;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.UI;

public class UIMainTitle : MonoBehaviour
{
    public UIGameRootCanvas parentUI;

    public Button buttonNewGame;
    public Button buttonLoadGame;
    public Button buttonExitGame;

    // Start is called before the first frame update
    void Start()
    {
        buttonNewGame.onClick.AddListener(OnClickNewGameButton);
        buttonLoadGame.onClick.AddListener(OnClickLoadGameButton);
        buttonExitGame.onClick.AddListener(OnClickExitGameButton);
    }

    private void OnClickNewGameButton()
    {
        var curDoc = DocumentDataManager.Instance.NewDefaultDocument();
        // 新游戏后开启游戏剧情起点
        DocumentDataManager.Instance.OpenScenarioById(ResourceUtils.NEW_GAME_SCENARIO_ID);
        // 新游戏开始后扫描所有SubTask和HelpScenario的Event并开启
        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.SubTask);
        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.HelpScenario);

        // 剧情动画不需要设定主角初始坐标
        GameMainManager.Instance.NextSceneId = curDoc.SceneId;

        parentUI.NewGame();
    }

    private void OnClickLoadGameButton()
    {
        parentUI.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.Load);
    }

    private void OnClickExitGameButton()
    {
        Application.Quit();
    }
}
