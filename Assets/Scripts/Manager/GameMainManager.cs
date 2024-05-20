using System;
using System.Collections.Generic;
using System.Linq;
using com.tksr.data;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏核心管理器,链接其他管理器以管理游戏
/// </summary>
public class GameMainManager : Singleton<GameMainManager>
{
    public int SceneId { get; set; }
    public int NextSceneId { get; set; }
    public int EntryId { get; set; } // 当用在战斗地图时,代表战场数据参数
    public int NextEntryId { get; set; }
    public bool FromLoadGame { get; private set; }

    [HideInInspector]
    public ScenarioEntry FromEntry;

    [HideInInspector]
    public UIGameRootCanvas UIRoot;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        ReInitScenarioData();
    }

    private void ReInitScenarioData()
    {
        this.SceneId = 0;
        this.NextSceneId = 0;
        this.EntryId = 0;
        this.NextEntryId = 0;
        this.FromEntry = null;
        this.FromLoadGame = false;
    }

    #region Scenes Flows or Switches
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Unity场景切换成功后续逻辑
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded, Scene Name = " + scene.name + ", nextScenarioId = " + NextSceneId + ", nextEntryId = " + NextEntryId);
        this.SceneId = this.NextSceneId;
        this.EntryId = this.NextEntryId;
        this.NextSceneId = 0;
        this.NextEntryId = 0;
        HandleSceneLoaded();
    }

    /// <summary>
    /// 递归搜索目标Scenario的前置链表(PreFinished),用于模拟该动画剧情所有前置条件已经完成
    /// </summary>
    /// <param name="fromScenario"></param>
    /// <param name="scenariosLink"></param>
    private void DevSearchScenariosToClosed(int fromScenario, List<int> scenariosLink)
    {
        var scenarioItem = ScenarioManager.Instance.GetScenarioItemById(fromScenario);
        if (scenarioItem == null)
        {
            return;
        }
        var funcList = SchemaParser.ParseParamToFunctionsMultiParam(scenarioItem.Conditions);
        if (funcList.Count == 0)
        {
            return;
        }

        List<int> foundIds = new List<int>();
        var foundPreFinishedFunc = funcList.Find(x => x.FuncName.CompareTo(EnumScenarioCondition.PreFinished.ToString()) == 0);
        if (foundPreFinishedFunc != null)
        {
            var ids = SchemaParser.ParseParamToInts(foundPreFinishedFunc.FuncParams[0]);
            foreach (var param in ids)
            {
                int id = param;
                foundIds.Add(id);
            }
        }

        var foundInScenePreFinishedFunc = funcList.Find(x =>
            x.FuncName.CompareTo(EnumScenarioCondition.InSceneAndPreFinished.ToString()) == 0);
        if (foundInScenePreFinishedFunc != null)
        {
            // InSceneAndPreFinished的PreFinished参数从index=1开始
            for (int i = 1; i < foundInScenePreFinishedFunc.FuncParams.Count; i++)
            {
                var param = foundInScenePreFinishedFunc.FuncParams[i];
                var ids = SchemaParser.ParseParamToInts(param);
                foundIds.AddRange(ids);
            }
        }

        for (int i = 0; i < foundIds.Count; i++)
        {
            int id = foundIds[i];
            scenariosLink.Add(id);
            DevSearchScenariosToClosed(id, scenariosLink);
        }
    }

    /// <summary>
    /// 递归搜索当前Scenario前置的Scenario链表,在调试状态下模拟前置Scenarios都被关闭(TODO:目前只能模拟关闭直系前置链表)
    /// </summary>
    /// <param name="fromScenarioId"></param>
    /// <param name="scenariosLink"></param>
    private void DevRecursiveScenariosToClosed(int fromScenarioId, List<int> scenariosLink)
    {
        var scenarioItem = ScenarioManager.Instance.GetScenarioItemById(fromScenarioId);
        if (scenarioItem == null)
        {
            return;
        }

        var preScenariosList = SchemaParser.ParseParamToInts(scenarioItem.PreScenarioIds);
        if (preScenariosList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < preScenariosList.Count; i++)
        {
            int id = preScenariosList[i];
            scenariosLink.Add(id);
            DevRecursiveScenariosToClosed(id, scenariosLink);
        }
    }

    /// <summary>
    /// 模拟调试单场景
    /// </summary>
    /// <param name="curSceneId"></param>
    /// <param name="curEntryId"></param>
    /// <param name="targetScenarioId"></param>
    public void SimulateLoadSceneByEventScenario(int curSceneId, int curEntryId, int targetScenarioId)
    {
        this.SceneId = curSceneId;
        this.NextSceneId = 0;
        this.EntryId = curEntryId;
        this.NextEntryId = 0;
        Debug.LogFormat("Simulate scene:{0}, entry:{1}, scenario:{2}", this.SceneId, this.EntryId, targetScenarioId);

        // 先开启当前Scenario(因为会开启事件默认的Scenario)
        var openedScenario = DocumentDataManager.Instance.OpenScenarioById(targetScenarioId);
        if (openedScenario == null)
        {
            Debug.LogErrorFormat("Open document scenario error. targetScenarioId = {0}", targetScenarioId);
            return;
        }

        // 添加Scenario执行序列,关闭前置的Scenarios
        List<int> scenariosLink = new List<int>();
        DevRecursiveScenariosToClosed(targetScenarioId, scenariosLink);
        scenariosLink = scenariosLink.Distinct().ToList();
        scenariosLink.Sort();
        Debug.LogFormat("Dev force close scenarios list = {0}", string.Join(",", scenariosLink));

        foreach (var toCloseScenarioId in scenariosLink)
        {
            DocumentDataManager.Instance.CloseScenarioById(toCloseScenarioId);
        }

        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.SubTask);
        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.HelpScenario);

        HandleSceneLoaded();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SimulateSceneLoadedOfOther(int curScene, int curEntry)
    {
        this.SceneId = curScene;
        this.NextSceneId = 0;
        this.EntryId = curEntry;
        this.NextEntryId = 0;
        Debug.LogFormat("Simulate scene:{0}, entry:{1}", this.SceneId, this.EntryId);

        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.SubTask);
        DocumentDataManager.Instance.ScanEventsToOpen(EnumEventType.HelpScenario);

        HandleSceneLoaded();
    }

    /// <summary>
    /// Unity Scene加载完毕后,创建游戏逻辑:
    /// 检测地图类型，若是剧情地图则需要考虑是否创建剧情；若是战斗地图则需要初始化相关战斗数据。
    /// 调用时机:
    /// 1.正常游戏的Scene切换新Scene被加载后
    /// 2.开发过程中调试单场景被模拟加载(因为开发过程中场景时是通过开发模块DevelopmentModeLoader启动场景切换的)
    /// </summary>
    private void HandleSceneLoaded()
    {
        if (this.SceneId == 0)
        {
            Debug.Log("This is Game Main Page");
            return;
        }

        if (UIRoot)
        {
            UIRoot.ShowBlackMask(false);
        }

        SceneMapManager.Instance.EnterScene(this.SceneId);
    }

    /// <summary>
    /// 在当前场景中根据nextScenarioId切换地图场景
    /// </summary>
    public void SwitchToNextScene(bool fromLoadGame = false)
    {
        this.FromLoadGame = fromLoadGame;

        CharactersManager.Instance.ClearExistCharacters();
        UIRoot.ShowBlackMask(true);

        SceneMapManager.Instance.LeaveScene();
        AudioManager.Instance.StopMusic();

        var scenario = SceneMapManager.Instance.GetSceneMapItemById(this.NextSceneId);
        if (scenario != null)
        {
            string strSceneName = scenario.UnityScene;
            string strSceneAB = scenario.SceneAssetBundle;
            if (!string.IsNullOrEmpty(strSceneName) && !string.IsNullOrEmpty(strSceneAB))
            {
                GameAssetBundlesManager.Instance.LoadSceneAsync(strSceneAB, strSceneName);
            }
        }
        else
        {
            Debug.LogWarningFormat("Error Next Scene Data, NextSceneId = {0}", this.NextEntryId   );
        }
    }
    #endregion

    private EnumGameMode gameMode;
    [HideInInspector]
    public EnumGameMode CurGameMode
    {
        get { return gameMode; }
        set
        {
            gameMode = value;
            bool open = gameMode == EnumGameMode.Gameplay ? true : false;
            if (CharactersManager.Instance.MainRoleController != null)
            {
                CharactersManager.Instance.MainRoleController.SwitchManualControlled(open);
            }
        }
    }

    public void EnableGameContent(bool bEnabled)
    {
        var gameMap = GameObject.FindGameObjectWithTag(EnumGameTagType.GOGameMap.ToString());
        var gameDesigner = GameObject.FindGameObjectWithTag(EnumGameTagType.GOGameDesigner.ToString());
        if (gameMap)
        {
            gameMap.gameObject.SetActive(bEnabled);
        }
        if (gameDesigner)
        {
            gameDesigner.gameObject.SetActive(bEnabled);
        }
    }
}
