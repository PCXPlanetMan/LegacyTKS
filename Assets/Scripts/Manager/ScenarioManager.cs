using com.tksr.schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using com.tksr.data;
using UnityEngine;
using UnityEngine.Playables;
using System;
using com.tksr.property;
using System.Linq;
using System.Threading.Tasks;
using com.tksr.document;
using static SchemaParser;

/// <summary>
/// 协调管理游戏剧情的检测播放以及资源配置的管理
/// 主要管理剧情Scenario中的角色(战斗角色主要由CharactersManager管理)
/// </summary>
public class ScenarioManager : Singleton<ScenarioManager>
{
    private PlayableDirector activeDirector;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region Schema Config Data
    private SchemaStoryLine schemaStoryLine;

    public void LoadStorySchema(string jsonStoryLine)
    {
        schemaStoryLine = JsonConvert.DeserializeObject<SchemaStoryLine>(jsonStoryLine);
    }

    public StoryScenarioItem GetScenarioItemById(int Id)
    {
        if (schemaStoryLine != null)
        {
            if (schemaStoryLine.scenarios.ContainsKey(Id.ToString()))
            {
                return schemaStoryLine.scenarios[Id.ToString()];
            }
        }

        return null;
    }

    public List<int> GetStoryEventNotes(int Id)
    {
        List<int> notesList = new List<int>();
        var storyEvent = GetScenarioItemById(Id);
        if (storyEvent != null)
        {
            var notes = storyEvent.Notes;
            if (!string.IsNullOrEmpty(notes))
            {
                notesList = SchemaParser.ParseParamToInts(notes);
            }
        }
        return notesList;
    }

    public StoryEventItem GetEventItemById(int Id)
    {
        if (schemaStoryLine != null && schemaStoryLine.events != null)
        {
            if (schemaStoryLine.events.ContainsKey(Id.ToString()))
            {
                return schemaStoryLine.events[Id.ToString()];
            }
        }
        return null;
    }

    public List<StoryEventItem> GetEventItemsByType(EnumEventType eventType)
    {
        List<StoryEventItem> foundEvents = new List<StoryEventItem>();
        if (schemaStoryLine != null && schemaStoryLine.events != null)
        {
            var allEvents = schemaStoryLine.events.Values.ToList();
            foundEvents = allEvents.FindAll(x => x.EventType.CompareTo(eventType.ToString()) == 0);
        }

        return foundEvents;
    }

    public StoryDeploymentItem GetDeploymentItemParamById(int Id)
    {
        if (schemaStoryLine != null && schemaStoryLine.deployments != null)
        {
            if (schemaStoryLine.deployments.ContainsKey(Id.ToString()))
            {
                return schemaStoryLine.deployments[Id.ToString()];
            }
        }
        return null;
    }

    public List<LittleGameItem> GetLittleGameItemParamByType(string strGameType)
    {
        List<LittleGameItem> allTypes = new List<LittleGameItem>();
        if (schemaStoryLine != null && schemaStoryLine.littlegames != null)
        {
            foreach (var kv in schemaStoryLine.littlegames)
            {
                if (kv.Value.Type.CompareTo(strGameType) == 0)
                {
                    allTypes.Add(kv.Value);
                }
            }
        }

        return allTypes;
    }

    public LittleGameItem GetLittleGameItemParamById(int Id)
    {
        if (schemaStoryLine != null && schemaStoryLine.littlegames != null)
        {
            if (schemaStoryLine.littlegames.ContainsKey(Id.ToString()))
            {
                return schemaStoryLine.littlegames[Id.ToString()];
            }
        }

        return null;
    }

    public List<StoryDeploymentItem> GetDeploymentItemsByBelongScene(int sceneId)
    {
        if (schemaStoryLine != null && schemaStoryLine.deployments != null)
        {
            var allDeployments = schemaStoryLine.deployments.Values.ToList();
            return allDeployments.FindAll(x => x.BelongScene == sceneId);
        }
        return null;
    }

    public StoryTaskItem GetTaskItemParamById(int Id)
    {
        if (schemaStoryLine != null && schemaStoryLine.tasks != null)
        {
            if (schemaStoryLine.tasks.ContainsKey(Id.ToString()))
            {
                return schemaStoryLine.tasks[Id.ToString()];
            }
        }

        return null;
    }

    public List<int> GetContainScenariosList(int eventId)
    {
        List<int> resultScenariosList = new List<int>();
        var eventItem = GetEventItemById(eventId);
        if (eventItem != null)
        {
            resultScenariosList = SchemaParser.ParseParamToInts(eventItem.PossibleScenarios);
        }

        return resultScenariosList;
    }

    public bool IsGameEventFinished(int eventId)
    {
        bool allFinished = false;
        var eventItem = GetEventItemById(eventId);
        if (eventItem != null)
        {
            var functions = SchemaParser.ParseParamToFunctionsSingle(eventItem.FinishedCondition);
            foreach (var fun in functions)
            {
                var funName = fun.FuncName;
                var funParams = fun.FuncParams;
                EnumFinishedCondition finishedCondition = (EnumFinishedCondition)Enum.Parse(typeof(EnumFinishedCondition), funName);
                switch (finishedCondition)
                {
                    case EnumFinishedCondition.AllScenariosDone:
                        {
                            allFinished = EventFinishedCondition_AllScenariosDone(eventItem);
                        }
                        break;
                    case EnumFinishedCondition.ScenariosDone:
                    {
                        string strParam = string.Join(",", funParams.ToArray());
                        allFinished = EventFinishedCondition_ScenariosDone(strParam);
                    }
                        break;
                    case EnumFinishedCondition.NeverDone:
                    {
                        allFinished = false;
                    }
                        break;
                }
            }
        }

        return allFinished;
    }

    private bool EventFinishedCondition_AllScenariosDone(StoryEventItem eventItem)
    {
        var containsScenarios = SchemaParser.ParseParamToInts(eventItem.PossibleScenarios);
        int containsCount = containsScenarios.Count;
        if (containsCount > 0)
        {
            int doneCount = 0;
            foreach (var id in containsScenarios)
            {
                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(id);
                if (scenarioStatus == EnumScenarioStatus.Finished)
                {
                    doneCount++;
                }
            }

            if (doneCount == containsCount)
            {
                return true;
            }
        }

        return false;
    }

    private bool EventFinishedCondition_ScenariosDone(string funParams)
    {
        var mustScenarios = SchemaParser.ParseParamToInts(funParams);
        int mustCount = mustScenarios.Count;
        if (mustCount > 0)
        {
            int doneCount = 0;
            foreach (var id in mustScenarios)
            {
                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(id);
                if (scenarioStatus == EnumScenarioStatus.Finished)
                {
                    doneCount++;
                }
            }

            if (doneCount == mustCount)
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Scenarios
    /// <summary>
    /// 检测当前剧情是否满足执行条件
    /// (1) Opened  只要当前scenario被开启正在執行就算满足(场景触发)
    /// (2) PreFinished:scenarioId  测试前置条件是否完成(场景触发)
    /// (3) InScene:sceneId 判断当前是否处于某场景中(人物触发)
    /// (4) InSceneDoing:sceneId|scenarioId 当前处于某场景中且某剧情正在执行中,因为某些NPC可能在不同的场景出现,所以需要用sceneId区分(人物触发)
    /// eg. InSceneDoing:2104|9020
    /// (5) InSceneAndPreFinished:sceneId|scenarioId 当前处于某场景中且某剧情已经完成关闭(人物触发)
    /// eg. InSceneAndPreFinished:2104|9024
    /// (6) FromEntryDoing:entryId|scenarioId 一般是主角在切换地图的时候判断当前来自那个出口(即判断进出地图),并且某剧情正在执行
    /// eg. FromEntryDoing:10000022|9035
    /// </summary>
    /// <param name="scenarioId"></param>
    /// <param name="strCondition"></param>
    /// <param name="strParams"></param>
    /// <returns></returns>
    private bool MeasureScenarioCondition(int scenarioId, string strCondition, List<string> strParams)
    {
        bool bSucceed = false;
        EnumScenarioCondition condition = (EnumScenarioCondition)Enum.Parse(typeof(EnumScenarioCondition), strCondition);
        switch (condition)
        {
            case EnumScenarioCondition.Opened:
            {
                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(scenarioId);
                if (scenarioStatus == EnumScenarioStatus.Running)
                {
                    bSucceed = true;
                }
                else
                {
                    bSucceed = false;
                }
            }
                break;
            case EnumScenarioCondition.PreFinished:
            {
                var paramsList = SchemaParser.ParseParamToInts(strParams[0]);
                bSucceed = true;
                foreach (var p in paramsList)
                {
                    int preFinishedId = p;
                    var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(preFinishedId);
                    if (scenarioStatus != EnumScenarioStatus.Finished)
                    {
                        bSucceed = false;
                        break;
                    }
                }
            }
                break;
            case EnumScenarioCondition.InScene:
            {
                int paramSceneId = int.Parse(strParams[0]);
                if (paramSceneId == SceneMapManager.Instance.GetCurrentSceneId())
                {
                    bSucceed = true;
                }
                else
                {
                    bSucceed = false;
                }
            }
                break;
            case EnumScenarioCondition.InSceneDoing:
                {
                    int paramSceneId = int.Parse(strParams[0]);
                    if (paramSceneId == SceneMapManager.Instance.GetCurrentSceneId())
                    {
                        bSucceed = true;
                    }
                    else
                    {
                        bSucceed = false;
                    }

                    for (int i = 1; i < strParams.Count; i++)
                    {
                        var p = strParams[i];
                        int id = int.Parse(p);
                        var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(id);
                        if (scenarioStatus != EnumScenarioStatus.Running)
                        {
                            bSucceed = false;
                            break;
                        }
                    }
                }
                break;
            case EnumScenarioCondition.InSceneAndPreFinished:
                {
                    int paramSceneId = int.Parse(strParams[0]);
                    if (paramSceneId == SceneMapManager.Instance.GetCurrentSceneId())
                    {
                        bSucceed = true;
                    }
                    else
                    {
                        bSucceed = false;
                    }

                    for (int i = 1; i < strParams.Count; i++)
                    {
                        var p = strParams[i];
                        var ids = SchemaParser.ParseParamToInts(p);
                        foreach (var id in ids)
                        {
                            var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(id);
                            if (scenarioStatus != EnumScenarioStatus.Finished)
                            {
                                bSucceed = false;
                                break;
                            }
                        }

                        if (!bSucceed)
                            break;
                    }
                }
                break;
            case EnumScenarioCondition.InSceneInTeam:
            {
                int paramSceneId = int.Parse(strParams[0]);
                if (paramSceneId == SceneMapManager.Instance.GetCurrentSceneId())
                {
                    bSucceed = true;
                    var toCheckCharIds = SchemaParser.ParseParamToInts(strParams[1]);
                    if (toCheckCharIds.Count == 0)
                    {
                        bSucceed = false;
                    }
                    for (int i = 0; i < toCheckCharIds.Count; i++)
                    {
                        int charId = toCheckCharIds[i];
                        bool inTeam = DocumentDataManager.Instance.IsCharInTeam(charId);
                        if (!inTeam)
                        {
                            bSucceed = false;
                            break;
                        }
                    }
                }
                else
                {
                    bSucceed = false;
                }
            }
                break;
            case EnumScenarioCondition.InSceneNotInTeam:
            {
                int paramSceneId = int.Parse(strParams[0]);
                if (paramSceneId == SceneMapManager.Instance.GetCurrentSceneId())
                {
                    bSucceed = false;
                    var toCheckCharIds = SchemaParser.ParseParamToInts(strParams[1]);
                    if (toCheckCharIds.Count == 0)
                    {
                        bSucceed = true;
                    }
                    else
                    {
                        bSucceed = true;
                        for (int i = 0; i < toCheckCharIds.Count; i++)
                        {
                            int charId = toCheckCharIds[i];
                            bool inTeam = DocumentDataManager.Instance.IsCharInTeam(charId);
                            if (inTeam)
                            {
                                bSucceed = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    bSucceed = false;
                }
            }
                break;
            case EnumScenarioCondition.FromEntryDoing:
            {
                int entryId = int.Parse(strParams[0]);
                if (entryId == GameMainManager.Instance.EntryId)
                {
                    bSucceed = true;
                }
                else
                {
                    bSucceed = false;
                }

                for (int i = 1; i < strParams.Count; i++)
                {
                    var p = strParams[i];
                    int id = int.Parse(p);
                    var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(id);
                    if (scenarioStatus != EnumScenarioStatus.Running)
                    {
                        bSucceed = false;
                        break;
                    }
                }
            }
                break;
            case EnumScenarioCondition.EventsDone:
            {
                List<int> eventIds = strParams.Select(int.Parse).ToList();
                bSucceed = true;
                for (int i = 0; i < eventIds.Count; i++)
                {
                    int eventId = eventIds[i];
                    var eventStatus = DocumentDataManager.Instance.GetGameEventStatus(eventId);
                    if (eventStatus != EnumEventStatus.Finished)
                    {
                        bSucceed = false;
                        break;
                    }
                }
            }
                break;
            case EnumScenarioCondition.LevelBelow:
            {
                List<int> paramsData = strParams.Select(int.Parse).ToList();
                int targetLevel = paramsData[0];
                var doc = DocumentDataManager.Instance.GetCurrentDocument();
                var mainRoleInfo = doc.MainRoleInfo;
                bSucceed = true;
                if (mainRoleInfo.Level >= targetLevel)
                {
                    bSucceed = false;
                }
            }
                break;
            case EnumScenarioCondition.LevelNotBelow:
            {
                List<int> paramsData = strParams.Select(int.Parse).ToList();
                int targetLevel = paramsData[0];
                var doc = DocumentDataManager.Instance.GetCurrentDocument();
                var mainRoleInfo = doc.MainRoleInfo;
                bSucceed = true;
                if (mainRoleInfo.Level < targetLevel)
                {
                    bSucceed = false;
                }
            }
                break;
            default:
                break;
        }
        return bSucceed;
    }

    /// <summary>
    /// 执行当前事件中所有的Actions
    /// (1) PlayTimeline 直接播放当前场景中编辑好的Timeline动画(场景触发)
    /// (2) ShowConversations:dialogIds 点击NPC后触发对话(人物触发)
    /// (3) TouchTimelineByClick 点击NPC后产生剧情动画
    /// (4) CheckSelections:[测试条件和参数],测试成功结果,测试失败结果     用于剧情中显示选择对话,根据测试条件不同而格式不同
    /// eg. CheckSelections:HasItem,80100003,40000648,40000647
    /// 包裹里面是否有id=80100003的物品,如果有则显示40000648的对话,如果没有则显示40000647的对话
    /// (5) JumpToPostAction    直接跳去执行PostAction
    /// </summary>
    /// <param name="currentScenarioId"></param>
    /// <returns></returns>

    /// <summary>
    /// 战斗结束(结算后)的处理
    /// </summary>
    /// <param name="strResultAction"></param>
    public void PrepareScenarioAfterBattle(string strResultAction)
    {
        if (!string.IsNullOrEmpty(strResultAction))
        {
            var funcList = SchemaParser.ParseParamToFunctionsMultiParam(strResultAction);
            string strFuncName = funcList[0].FuncName;
            List<string> listFuncParams = funcList[0].FuncParams;

            EnumBattleResultAction resultAction =
                (EnumBattleResultAction)Enum.Parse(typeof(EnumBattleResultAction), strFuncName);
            switch (resultAction)
            {
                case EnumBattleResultAction.SwitchSceneOpenScenario:
                {
                    if (listFuncParams.Count > 0)
                    {
                        var sceneIds = SchemaParser.ParseParamToInts(listFuncParams[0]);
                        int nextSceneId = 0;
                        if (sceneIds.Count == 1)
                        {
                            nextSceneId = sceneIds[0];
                        }
                        if (listFuncParams.Count > 1 && nextSceneId > 0)
                        {
                            var scenarios = SchemaParser.ParseParamToInts(listFuncParams[1]);
                            foreach (var selScenarioId in scenarios)
                            {
                                DocumentDataManager.Instance.OpenScenarioById(selScenarioId);
                            }

                            if (listFuncParams.Count > 2)
                            {
                                var tasks = SchemaParser.ParseParamToInts(listFuncParams[2]);
                                foreach (var taskId in tasks)
                                {
                                    DocumentDataManager.Instance.UpdateScenarioTaskById(taskId);
                                }
                            }
                            

                            GameMainManager.Instance.NextSceneId = nextSceneId;
                            GameMainManager.Instance.NextEntryId = 0;
                            GameMainManager.Instance.SwitchToNextScene();
                        }
                    }
                    
                }
                    break;
                default:
                {

                }
                    break;
            }
        }
        else
        {
            Debug.Log("No result action after battle or failed, so go to main page");

        }
    }
    #endregion

    #region Timeline(TimelineScenarioItem) Events/Actions
    /// <summary>
    /// 剧情动画开始的标志
    /// eg.某些剧情动画需要预先设置主角的位置
    /// </summary>
    /// <param name="currentScenarioId"></param>
    public void TimelineActionWhenScenarioBegin(EnumStoryActionType preAction, List<string> preParams)
    {
        switch (preAction)
        {
            case EnumStoryActionType.TimeStopMainRoleFrame:
            {
                CharactersManager.Instance.MainRoleController.TimeStopAnimationFrame();
            }
                break;
            case EnumStoryActionType.CameraFollowMainRole:
            {
                SetCameraFollowTargetCharacter(ResourceUtils.MAINROLE_ID);
            }
                break;
            case EnumStoryActionType.TimeStopNPCFrame:
            {
                List<int> charsId = preParams.Select(int.Parse).ToList();
                foreach (var charId in charsId)
                {
                    var scenarioChar = CharactersManager.Instance.ScenarioFindCharById(charId);
                    if (scenarioChar != null)
                    {
                        scenarioChar.TimeStopAnimationFrame();
                    }
                }
            }
                break;
            case EnumStoryActionType.PlaceMainRoleInEntry:
            {
                SceneMapManager.Instance.PrepareMainRoleResAndDeploymentByEntry(false);
            }
                break;
            default:
                break;
        }
    }

    public void TimelineActionWhenScenarioEnd(EnumStoryActionType postAction, List<string> postParams)
    {
        var scenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
        scenarioMap.Timelines.HideAllGameEventIdentities();

        switch (postAction)
        {
            case EnumStoryActionType.None:
            {
                // Do Nothing
            }
                break;
            case EnumStoryActionType.FreeActivity:
            {
                GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;
                //UpdateDocumentStoryNotes(currentScenarioId);

                var mainRole = CharactersManager.Instance.MainRoleController;
                if (mainRole != null)
                {
                    if (postParams != null && postParams.Count > 0)
                    {
                        EnumDirection mainRoleDirection = (EnumDirection)Enum.Parse(typeof(EnumDirection), postParams[0]);
                        mainRole.CharAnimRender.SetDirection(EnumAnimAction.Static, mainRoleDirection);
                    }
                    
                    mainRole.SwitchManualControlled(true);
                }
                else
                {
                    Debug.Log("Current Timeline doesn't has MainRole");
                }
            }
                break;
            case EnumStoryActionType.UnloadScenarioChars:
            {
                if (postParams.Count > 0)
                {
                    List<int> toUnloadChars = SchemaParser.ParseParamToInts(postParams[0]);
                    for (int i = 0; i < toUnloadChars.Count; i++)
                    {
                        int charId = toUnloadChars[i];
                        CharactersManager.Instance.ScenarioUnloadCharById(charId);
                    }
                }
            }
                break;
            case EnumStoryActionType.ResetDeployments:
            {
                foreach (var scenarioId in currentRunningScenarioIds)
                {
                    var scenarioItem = GetScenarioItemById(scenarioId);
                    if (scenarioItem != null)
                    {
                        var deployIdsList = SchemaParser.ParseParamToInts(scenarioItem.DeployChars);
                        foreach (var deployId in deployIdsList)
                        {
                            DoDeploymentInScenario(deployId, scenarioId, true);
                        }
                    }
                }
            }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// (1) NextScene:sceneId|scenarioId    剧情结束后进入下一个场景默认打开某个剧情(如果有的话)
    /// (2) FreeActivity:mainRoleDirection|deploymentId|followedScenarios(,)|toClosedScenarios  剧情结束后主角自由活动设置其朝向(自动Close当前剧情),部署其他NPC按照既定路线行动(若为0则无部署),然后开启后续剧情,并关闭其他相关剧情(大部分是人物对话)
    /// eg. FreeActivity:SW|0|9029-->9031,9033|9025,9026,9032
    /// (3) OpenBattle:sceneId|battleFieldId    触发战斗
    /// eg. OpenBattle:3002|5501
    /// (4) DirectCloseOpenScenarios:toCloseScenarioIds|toOpenScenarioIds   在存档中关闭剧情并开启其他剧情
    /// </summary>
    /// <param name="curScenarioId"></param>
    


    /// <summary>
    /// 根据Scenario部署人物(加载资源),开关场景交互元素
    /// </summary>
    /// <param name="toDeployId"></param>
    /// <param name="currentScenarioId"></param>
    private void DoDeploymentInScenario(int toDeployId, int currentScenarioId, bool forceUpdate = false)
    {
        if (toDeployId == 0)
        {
            Debug.LogFormat("DoDeploy with id = 0, so disappear all npcs used in current scenario");
            //var scenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
            //scenarioMap.Timelines.DisableCharsNPCUsedInTimeline(currentScenarioId);
            return;
        }
        var deployment = GetDeploymentItemParamById(toDeployId);
        if (deployment == null)
        {
            Debug.LogWarningFormat("Can't find Deployment data with toDeployId = {0}", toDeployId);
            return;
        }

        if (deployment.BelongScene != SceneMapManager.Instance.GetCurrentSceneId())
        {
            Debug.LogWarningFormat("Not same Scene id");
            return;
        }

        //var goEntriesContainer = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainEntries.ToString());
        //var allEntries = goEntriesContainer.GetComponentsInChildren<ScenarioEntry>(true);
        //foreach (var entry in allEntries)
        //{
        //    entry.gameObject.SetActive(true);
        //}

        if (!string.IsNullOrEmpty(deployment.Deployments))
        {
            var goNPCContainer = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainNPCs.ToString());
            var npcs = goNPCContainer.GetComponentsInChildren<NPCMainController>(true);
            //List<IGameCharacterRenderer> listToHideChars = new List<IGameCharacterRenderer>(npcs);


            var allDeployments = SchemaParser.ParseParamToStringList(deployment.Deployments, SchemaParser.SPLITTER_NORMAL);
            foreach (var item in allDeployments)
            {
                if (SchemaParser.ParseParamAsPairInt(item, out var charId, out var charTweenAnimId))
                {
                    var charIns = CharactersManager.Instance.GetCharacterInstanceById(charId);
                    if (charIns == null || charIns.IsHiddenObj != 0) // 隐藏物体无需加载资源也无需实际部署
                    {
                        var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                        var foundHiddenObj = curScenarioMap.HiddenObjects.Find(x => x.CharID == charId);
                        if (foundHiddenObj != null)
                        {
                            foundHiddenObj.gameObject.SetActive(true);
                            CharactersManager.Instance.ScenarioAddHiddenGameObject(foundHiddenObj.gameObject);
                        }
                    }
                    else
                    {
                        var npc = CharactersManager.Instance.ScenarioFindCharById(charId);
                        if (npc == null)
                        {
                            var foundNPC = npcs.ToList().Find(x => x.CharID == charId);
                            CharactersManager.Instance.ScenarioLoadCharNPCGameObject(foundNPC.gameObject);
                            npc = foundNPC;
                            npc.LoadRenderResource();
                            npc.gameObject.SetActive(true);

                            var container = npc.GetComponent<NPCMainController>();
                            container.RunTweenPathById(charTweenAnimId);
                        }
                        else
                        {
                            var container = npc.GetComponent<NPCMainController>();
                            int oldTweenPathId = container.GetCurrentTweenPathId();
                            if (oldTweenPathId != charTweenAnimId || forceUpdate)
                            {
                                container.RunTweenPathById(charTweenAnimId);
                            }
                            else
                            {
                                // TODO:一般来说,如果角色已经被部署了Path,则如果相同的Deploy再次发生的时候不应该重新执行,否则可能产生错误
                                Debug.LogFormat("Char {0} already has a same tween path {1} running.", charId, charTweenAnimId);
                                //container.RunTweenPathById(charTweenAnimId, false);
                            }
                        }
                        Debug.LogFormat("DoDeploymentInScenario, char = {0}, doTweenAnim = {1}", charId, charTweenAnimId);
                        //listToHideChars.Remove(npc);
                    }
                }
                else
                {
                    int specialZero;
                    if (int.TryParse(item, out specialZero))
                    {
                        if (specialZero == 0)
                        {
                            // 如果部署ID列表中有0,则说明当前剧情在场景中不属于任何NPC
                            // 一旦发现一个ID为0,则立马跳出
                            Debug.LogFormat("Deploy id = 0, so disappear all npcs used in current scenario");
                            var scenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                            scenarioMap.Timelines.DisableCharsNPCUsedInTimeline(currentScenarioId);
                            break;
                        }
                        else
                        {
                            Debug.LogFormat("Something wrong when parsed deploy information: item={0}", item);
                        }
                    }
                }
            }

            //for (int i = 0; i < listToHideChars.Count; i++)
            //{
            //    listToHideChars[i].gameObject.SetActive(false);
            //}
        }

        // 操作和Scenario以及场景相关的交互元素(eg. 开关隐藏物体碰撞,开关出入口)
        if (!string.IsNullOrEmpty(deployment.StoryActions))
        {
            var funcList = SchemaParser.ParseParamToFunctionsMultiParam(deployment.StoryActions);

            for (int i = 0; i < funcList.Count; i++)
            {
                string actionName = funcList[i].FuncName;
                List<string> actionParams = funcList[i].FuncParams;

                var preparedFun = (EnumStoryActionType)Enum.Parse(typeof(EnumStoryActionType), actionName);
                if (preparedFun == EnumStoryActionType.CloseSceneEntryColliders)
                {
                    List<int> entriesColliderIds = actionParams.Select(int.Parse).ToList();
                    var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                    foreach (var id in entriesColliderIds)
                    {
                        var foundEntry = curScenarioMap.Entries.FirstOrDefault(x => x.entryId == id);
                        if (foundEntry != null)
                        {
                            // TODO: 某些剧情需要暂时关闭Entry,并不能简单的Disabled,因为此时还要响应鼠标的ICON
                            foundEntry.gameObject.SetActive(false);
                        }
                    }
                }
                else if (preparedFun == EnumStoryActionType.OpenEnvironmentColliders)
                {
                    var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                    curScenarioMap.WorldSceneSwitchOnColliders(actionParams);
                }
                else if (preparedFun == EnumStoryActionType.CloseEnvironmentColliders)
                {
                    var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                    curScenarioMap.WorldSceneSwitchOffColliders(actionParams);
                }
                else if (preparedFun == EnumStoryActionType.OpenSceneEntryColliders)
                {
                    List<int> entriesColliderIds = actionParams.Select(int.Parse).ToList();
                    var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                    foreach (var id in entriesColliderIds)
                    {
                        var foundEntry = curScenarioMap.Entries.FirstOrDefault(x => x.entryId == id);
                        if (foundEntry != null)
                        {
                            // TODO: 某些剧情需要开启Entry,并不能简单的Disabled,因为此时还要响应鼠标的ICON
                            foundEntry.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }
    private void UpdateDocumentStoryNotes(int scenarioId)
    {
        var notes = GetStoryEventNotes(scenarioId);

        List<int> results = new List<int>();
        for (int i = 0; i < notes.Count; i++)
        {
            var note = notes[i];
            if (note != 0)
            {
                results.Add(note);
            }
        }

        DocumentDataManager.Instance.RecordNotesOfScenario(results);
    }

    public void PauseTimelineByDialog(PlayableDirector whichOne)
    {
        activeDirector = whichOne;
        if (activeDirector)
        {
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        }
            
        GameMainManager.Instance.CurGameMode = EnumGameMode.ScenarioDialogMoment;
        timelineHasSelDlgId = 0;
    }

    private int timelineHasSelDlgId = 0;
    public void PauseTimelineBySelectionDialog(PlayableDirector whichOne, int selDlgId)
    {
        activeDirector = whichOne;
        if (activeDirector)
        {
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        }

        GameMainManager.Instance.CurGameMode = EnumGameMode.ScenarioDialogHasSelection;
        timelineHasSelDlgId = selDlgId;
    }

    public void PauseTimelineByNote(PlayableDirector whichOne)
    {
        activeDirector = whichOne;
        if (activeDirector)
        {
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        }
        GameMainManager.Instance.CurGameMode = EnumGameMode.ScenarioStaticNote;
        timelineHasSelDlgId = 0;
    }

    public void PauseTimelineByInput(PlayableDirector whichOne)
    {
        activeDirector = whichOne;
        if (activeDirector)
        {
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        }
        
        GameMainManager.Instance.CurGameMode = EnumGameMode.WaitingInputName;
        timelineHasSelDlgId = 0;
    }

    public void ResumeCurrentTimeline()
    {
        switch (GameMainManager.Instance.CurGameMode)
        {
            case EnumGameMode.ScenarioDialogMoment:
            {
                HideUIDialogs();
            }
                break;
            case EnumGameMode.ScenarioStaticNote:
            {
                HideCutInOutNotes();
            }
                break;
        }

        if (activeDirector != null)
        {
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            GameMainManager.Instance.CurGameMode = EnumGameMode.RunningScenario;
        }

        timelineHasSelDlgId = 0;
    }
    #endregion

    #region Dialog for Scenario Characters
    private Dictionary<int, UIDialogPlayer> dictSceneCharDialogs = new Dictionary<int, UIDialogPlayer>();

    /// <summary>
    /// 创建剧情或者非战斗场景中的人物对话系统
    /// </summary>
    /// <param name="charRender"></param>
    public void CreateCharDialogUI(IGameCharacterRenderer charRender)
    {
        Transform dialogs = GameMainManager.Instance.UIRoot.DialogsPanel;
        if (dialogs != null && charRender != null)
        {
            bool bShouldAdd = false;
            if (!dictSceneCharDialogs.ContainsKey(charRender.CharID))
            {
                bShouldAdd = true;
            }
            else
            {
                if (dictSceneCharDialogs[charRender.CharID] == null)
                {
                    dictSceneCharDialogs.Remove(charRender.CharID);
                    bShouldAdd = true;
                }
            }

            if (bShouldAdd)
            {
                GameObject dialogPrefab = GameAssetBundlesManager.Instance.LoadPrefabSync(ResourceUtils.AB_UI_DIALOG, ResourceUtils.ASSET_UI_DIALOG);
                if (dialogPrefab == null)
                {
                    Debug.LogError("Dialog Prefab is null");
                }
                GameObject goDialog = GameObject.Instantiate(dialogPrefab);
                var dialogPlayer = goDialog.GetComponent<UIDialogPlayer>();
                dictSceneCharDialogs.Add(charRender.CharID, dialogPlayer);
                dialogPlayer.GetComponent<RectTransform>().SetParent(dialogs);
                dialogPlayer.gameObject.SetActive(false);
            }
        }
    }

    public void ClearCharDialogUI(IGameCharacterRenderer charRender)
    {
        if (charRender == null)
            return;

        foreach (var kv in dictSceneCharDialogs)
        {
            if (charRender.CharID == kv.Key)
            {
                var dialogPlayer = kv.Value;
                dialogPlayer.DetachChar();
                dialogPlayer.transform.SetParent(null);
                GameObject.Destroy(dialogPlayer.gameObject);
                break;
            }
        }
    }

    public void ClearAllCharsDialogUI()
    {
        foreach (var kv in dictSceneCharDialogs)
        {
            UIDialogPlayer dialogPlayer = kv.Value;
            if (dialogPlayer == null)
                continue;
            dialogPlayer.DetachChar();
            dialogPlayer.transform.SetParent(null);
            GameObject.Destroy(dialogPlayer.gameObject);
        }
        dictSceneCharDialogs.Clear();
    }

    /// <summary>
    /// 在Timeline中处理Selection类型的对话时需要特殊处理
    /// </summary>
    /// <param name="dialogsId"></param>
    /// <returns></returns>
    public int HasSelectionsDialogContent(int[] dialogsId)
    {
        int hasSelectionDlgID = 0;

        for (int i = 0; i < dialogsId.Length; i ++)
        {
            int dialogID = dialogsId[i];
            var dialogItem = TextsManager.Instance.GetDialogItemById(dialogID);
            if (dialogItem != null)
            {
                EnumDialogContentType contentType = EnumDialogContentType.Normal;
                if (Enum.TryParse(dialogItem.Type, out contentType))
                {
                    if (contentType == EnumDialogContentType.Selections)
                    {
                        hasSelectionDlgID = dialogID;
                        break;
                    }
                }
            }
        }

        return hasSelectionDlgID;
    }

    /// <summary>
    /// 显示多个对话内容
    /// </summary>
    /// <param name="dialogsParam"></param>
    public void ShowUIDialogs(string[] dialogsParam)
    {
        if (dialogsParam != null && dialogsParam.Length > 0)
        {
            foreach (var param in dialogsParam)
            {
                var items = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_NORMAL);
                if (items.Count == 2)
                {
                    int dialogID = items[0];
                    int showSide = items[1];
                    UIDialogPlayer.EnumUIDialogShowMode mode = (UIDialogPlayer.EnumUIDialogShowMode) showSide;
                    ShowUIDialogById(dialogID, mode);
                }
            }
        }
    }

    /// <summary>
    /// 场景中人物在做Tween动画的时候可能会显示类似Sigh的对话
    /// </summary>
    /// <param name="dialogID"></param>
    /// <param name="show"></param>
    public void AnimationDisplayDialogSigh(int dialogID, bool show)
    {
        if (!show)
        {
            HideUIDialogById(dialogID);
        }
        else
        {
            var dialogItem = TextsManager.Instance.GetDialogItemById(dialogID);
            var speaker = CharactersManager.Instance.ScenarioFindCharById(dialogItem.Speaker);
            if (GameMainManager.Instance.UIRoot == null)
                return;
            Transform dialogs = GameMainManager.Instance.UIRoot.DialogsPanel;
            if (dialogs != null && speaker != null)
            {
                UIDialogPlayer dialogPlayer = dictSceneCharDialogs[speaker.CharID];
                if (dialogPlayer != null && dialogPlayer.gameObject.activeInHierarchy == false)
                {
                    var attachedChar = dialogPlayer.CurrentAttachedChar();
                    if (attachedChar == null)
                    {
                        string strDialogRealContents = ParseDialogContent(dialogItem);
                        if (string.IsNullOrEmpty(strDialogRealContents))
                        {
                            Debug.LogErrorFormat("Error dialog content, id = {0}", dialogID);
                            return;
                        }

                        dialogs.gameObject.SetActive(true);
                        dialogPlayer.gameObject.SetActive(true);
                        UIDialogPlayer.EnumUIDialogShowMode showSide =
                            UIDialogPlayer.EnumUIDialogShowMode.ShowSigh;
                        ParseDialogExtraParam(dialogItem, speaker, ref showSide);
                        dialogPlayer.AttachChar(speaker, strDialogRealContents, showSide);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 由Timeline驱动UI显示对话
    /// </summary>
    /// <param name="dialogID"></param>
    /// <param name="showSide">剧情动画中对话框显示左右模式:0,左边,1右边,2内心OS,3类似OS但无背景,-1在非动画剧情中由当前人物朝向决定</param>
    public void ShowUIDialogById(int dialogID, UIDialogPlayer.EnumUIDialogShowMode showSide = UIDialogPlayer.EnumUIDialogShowMode.NormalByOrientation)
    {
        var dialogItem = TextsManager.Instance.GetDialogItemById(dialogID);
        var speaker = CharactersManager.Instance.ScenarioFindCharById(dialogItem.Speaker);
        if (GameMainManager.Instance.UIRoot == null)
            return;
        Transform dialogs = GameMainManager.Instance.UIRoot.DialogsPanel;
        if (dialogs != null && speaker != null)
        {
            dialogs.gameObject.SetActive(true);
            UIDialogPlayer dialogPlayer = dictSceneCharDialogs[speaker.CharID];
            if (dialogPlayer != null)
            {
                string strDialogRealContents = ParseDialogContent(dialogItem);
                if (string.IsNullOrEmpty(strDialogRealContents))
                {
                    Debug.LogError("Null dialog content.");
                    return;
                }

                dialogPlayer.gameObject.SetActive(true);
                ParseDialogExtraParam(dialogItem, speaker, ref showSide);
                dialogPlayer.AttachChar(speaker, strDialogRealContents, showSide);
            }
        }
        else if (speaker == null) // 将对话中的黑屏FadeOut/In效果作为一种特殊的对话类型处理
        {
            string strDialogRealContents = ParseDialogContent(dialogItem);
            if (string.IsNullOrEmpty(strDialogRealContents))
            {
                Debug.LogFormat("Dialog content, id = {0}", dialogID);
                EnumDialogContentType contentType = EnumDialogContentType.Normal;
                if (Enum.TryParse(dialogItem.Type, out contentType))
                {
                    if (contentType == EnumDialogContentType.BlackMask)
                    {
                        GameMainManager.Instance.UIRoot.AnimShowBlackMaskForDialog();
                        return;
                    }
                }
                Debug.LogError("Null dialog content and it's not a BlackMask.");
                return;
            }
        }
    }

    private Dictionary<int, EnumDirection> dictDlgSpecOldFaceTo = new Dictionary<int, EnumDirection>();

    /// <summary>
    /// 某些对话会启动特定的事件
    /// (1) FaceTo:charId   使得当前说话人面向其他人
    /// </summary>
    /// <param name="dialogItem"></param>
    /// <param name="speaker"></param>
    /// <param name="showSide"></param>
    private void ParseDialogExtraParam(TextsDialogItem dialogItem, IGameCharacterRenderer speaker, ref UIDialogPlayer.EnumUIDialogShowMode showSide, int selExtraIndex = -1)
    {
        EnumDialogContentType contentType = EnumDialogContentType.Normal;
        if (Enum.TryParse(dialogItem.Type, out contentType))
        {
            if (contentType == EnumDialogContentType.OS)
            {
                showSide = UIDialogPlayer.EnumUIDialogShowMode.ShowOS;
            }
            else if (contentType == EnumDialogContentType.Selections)
            {
                showSide = UIDialogPlayer.EnumUIDialogShowMode.ShowSelections;
            }
            else if (contentType == EnumDialogContentType.Sigh)
            {
                showSide = UIDialogPlayer.EnumUIDialogShowMode.ShowSigh;
            }
            else if (contentType == EnumDialogContentType.BlackMask)
            {
                return;
            }

            var dialogExtraActions = SchemaParser.ParseParamToFunctionsMultiParam(dialogItem.ExtraActions);
            int index = 0;
            foreach (var extra in dialogExtraActions)
            {
                // 对于Selection对话框,ExtraActions只执行Index项
                if (selExtraIndex != -1 && selExtraIndex != index)
                {
                    continue;
                }

                var extraFunc = (EnumDialogExtraAction) Enum.Parse(typeof(EnumDialogExtraAction), extra.FuncName);
                switch (extraFunc)
                {
                    case EnumDialogExtraAction.FaceTo:
                    {
                        if (extra.FuncParams.Count > 0)
                        {
                            int faceToNpcId = int.Parse(extra.FuncParams[0]);
                            var faceToSpeaker = CharactersManager.Instance.ScenarioFindCharById(faceToNpcId);
                            if (faceToSpeaker != null)
                            {
                                if (dictDlgSpecOldFaceTo.ContainsKey(speaker.CharID))
                                {
                                    dictDlgSpecOldFaceTo[speaker.CharID] = speaker.CharAnimRender.GetLastDirection();
                                }
                                else
                                {
                                    dictDlgSpecOldFaceTo.Add(speaker.CharID, speaker.CharAnimRender.GetLastDirection());
                                }

                                var npc = speaker;
                                Vector2 vecFaceTo = new Vector2(faceToSpeaker.transform.position.x, faceToSpeaker.transform.position.y);
                                Vector2 vecNPC = new Vector2(npc.transform.position.x, npc.transform.position.y);
                                Vector2 vecDirection = vecFaceTo - vecNPC;
                                Vector2 direction = Vector2.ClampMagnitude(vecDirection, 1f);
                                var enumDirection = npc.CharAnimRender.CalcDirectionByVector(direction);
                                npc.CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
                            }
                            else
                            {
                                Debug.LogFormat("Can't found Npc with id = {0}", faceToNpcId);
                            }
                        }
                        else
                        {
                            Debug.LogFormat("Had none faceto parameters");
                        }
                    }
                        break;
                    case EnumDialogExtraAction.KeepAnim: // TODO：在面向主角后再执行此操作似乎不起作用??
                    {
                        //speaker.CharAnimRender.LoadOldAnimationData();
                    }
                        break;
                    default:
                        break;
                }

                index++;
            }
        }
    }

    private string ParseDialogContent(TextsDialogItem item)
    {
        string dialogContent = null;
        EnumDialogContentType contentType = EnumDialogContentType.Normal;
        if (Enum.TryParse(item.Type, out contentType))
        {
            switch (contentType)
            {
                case EnumDialogContentType.Normal:
                case EnumDialogContentType.OS:
                case EnumDialogContentType.Selections:
                case EnumDialogContentType.Sigh:
                {
                    dialogContent = item.Contents;
                }
                    break;
                case EnumDialogContentType.FullName:
                {
                    string strFirstName = DocumentDataManager.Instance.GetMainRoleFirstName();
                    string strLastName = DocumentDataManager.Instance.GetMainRoleLastName();
                    dialogContent = string.Format(item.Contents, strLastName + strFirstName);
                }
                    break;
                case EnumDialogContentType.FirstName:
                {
                    string strFirstName = DocumentDataManager.Instance.GetMainRoleFirstName();
                    dialogContent = string.Format(item.Contents, strFirstName);
                }
                    break;
                case EnumDialogContentType.LastName:
                {
                    string strLastName = DocumentDataManager.Instance.GetMainRoleLastName();
                    dialogContent = string.Format(item.Contents, strLastName);
                }
                    break;
                case EnumDialogContentType.LastNameX2:
                {
                    string strLastName = DocumentDataManager.Instance.GetMainRoleLastName();
                    dialogContent = string.Format(item.Contents, strLastName, strLastName);
                }
                    break;
                case EnumDialogContentType.SplitName:
                {
                    string strFirstName = DocumentDataManager.Instance.GetMainRoleFirstName();
                    string strLastName = DocumentDataManager.Instance.GetMainRoleLastName();
                    dialogContent = string.Format(item.Contents, strLastName, strFirstName);
                }
                    break;
                case EnumDialogContentType.BlackMask:
                {
                    dialogContent = string.Empty;
                }
                    break;
                default:
                    break;
            }
        }

        return dialogContent;
    }

    /// <summary>
    /// 隐藏UI上当前的所有对话
    /// </summary>
    public void HideUIDialogs()
    {
        if (GameMainManager.Instance.UIRoot != null)
        {
            Transform dialogs = GameMainManager.Instance.UIRoot.DialogsPanel;
            foreach (var kv in dictSceneCharDialogs)
            {
                UIDialogPlayer dialogPlayer = kv.Value;
                if (dialogPlayer != null)
                {
                    dialogPlayer.DetachChar();
                    dialogPlayer.gameObject.SetActive(false);
                }

            }
            dialogs.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 检测当前对话是否是选择状态,若是,则应该不响应屏幕点击
    /// </summary>
    /// <param name="dialogID"></param>
    /// <returns></returns>
    private bool CheckUIDialogSelection(int dialogID)
    {
        var dialogItem = TextsManager.Instance.GetDialogItemById(dialogID);
        if (dialogItem != null)
        {
            EnumDialogContentType type =
                (EnumDialogContentType)Enum.Parse(typeof(EnumDialogContentType), dialogItem.Type);
            if (type == EnumDialogContentType.Selections)
            {
                return true;
            }
        }

        return false;
    }

    private void HideUIDialogById(int dialogID)
    {
        var dialogItem = TextsManager.Instance.GetDialogItemById(dialogID);
        var speaker = CharactersManager.Instance.ScenarioFindCharById(dialogItem.Speaker);

        Transform dialogs = GameMainManager.Instance.UIRoot.DialogsPanel;
        if (dialogs != null && speaker != null)
        {
            UIDialogPlayer dialogPlayer = dictSceneCharDialogs[speaker.CharID];
            if (dialogPlayer != null)
            {
                dialogPlayer.gameObject.SetActive(false);
                dialogPlayer.DetachChar();
            }
        }
    }

    /// <summary>
    /// 显示剧情中的全屏的切换提示(和普通对话一样的响应)
    /// </summary>
    /// <param name="noteID"></param>
    public void ShowCutInOutNoteById(int noteID)
    {
        var informationItem = TextsManager.Instance.GetInformationItemById(noteID);
        if (informationItem != null)
        {
            if (GameMainManager.Instance.UIRoot)
                GameMainManager.Instance.UIRoot.ShowCutInOutNotes(true, informationItem.Contents);
        }
    }

    /// <summary>
    /// 隐藏剧情中的全屏的切换提示
    /// </summary>
    public void HideCutInOutNotes()
    {
        if (GameMainManager.Instance.UIRoot)
            GameMainManager.Instance.UIRoot.ShowCutInOutNotes(false, null);
    }

    /// <summary>
    /// 根据配置数据的类型和ID显示格式化的Toast提示.Toast一般是在持续时间结束后自动隐藏.
    /// </summary>
    /// <param name="toastID"></param>
    /// <param name="type"></param>
    /// <param name="param"></param>
    public void ShowToastHint(int toastID, EnumToastType type, int param)
    {
        var informationItem = TextsManager.Instance.GetInformationItemById(toastID);
        if (informationItem != null)
        {
            if (GameMainManager.Instance.UIRoot)
            {
                string strToast = informationItem.Contents;
                if (type == EnumToastType.TKRItem)
                {
                    EnumTextContentType textType = EnumTextContentType.Normal;
                    if (Enum.TryParse(informationItem.Type, out textType))
                    {
                        if (textType == EnumTextContentType.Format)
                        {
                            EnumGameItemType itemType = EnumGameItemType.Invalid;
                            string itemName = string.Empty;
                            var tkrItem = ItemsManager.Instance.GetTKRItemById(param, out itemType, out itemName);
                            if (tkrItem != null)
                            {
                                strToast = string.Format(strToast, itemName);
                            }
                        }
                    }
                }
                else if (type == EnumToastType.Money)
                {
                    EnumTextContentType textType = EnumTextContentType.Normal;
                    if (Enum.TryParse(informationItem.Type, out textType))
                    {
                        if (textType == EnumTextContentType.Format)
                        {
                            strToast = string.Format(strToast, param);
                        }
                    }
                }
                else
                {

                }
                GameMainManager.Instance.UIRoot.ShowToastHint(true, strToast);
            }
        }
    }

    public void ShowMiddleScenarioText(int toastID)
    {
        var informationItem = TextsManager.Instance.GetInformationItemById(toastID);
        if (informationItem != null)
        {
            string strToast = informationItem.Contents;
            if (GameMainManager.Instance.UIRoot != null)
            {
                GameMainManager.Instance.UIRoot.ShowScenarioTextInMiddle(strToast);
            }
        }
    }

    public void ShowBottomLeftToastForICM(int nInt, int nCour, int nMor)
    {
        string strInt = null;
        string strCour = null;
        string strMor = null;
        if (nInt > 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_INTELLIGENCE_UP);
            strInt = item.Contents;
        }
        else if (nInt < 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_INTELLIGENCE_DOWN);
            strInt = item.Contents;
        }

        if (nCour > 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_COURAGE_UP);
            strCour = item.Contents;
        }
        else if (nCour < 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_COURAGE_DOWN);
            strCour = item.Contents;
        }

        if (nMor > 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_MORALITY_UP);
            strMor = item.Contents;
        }
        else if (nMor < 0)
        {
            var item = TextsManager.Instance.GetInformationItemById(GameConstData.TOAST_ID_MORALITY_DOWN);
            strMor = item.Contents;
        }

        if (GameMainManager.Instance.UIRoot != null)
        {
            if (!string.IsNullOrEmpty(strInt))
            {
                GameMainManager.Instance.UIRoot.ShowToastForICM(strInt);
            }

            if (!string.IsNullOrEmpty(strCour))
            {
                GameMainManager.Instance.UIRoot.ShowToastForICM(strCour);
            }

            if (!string.IsNullOrEmpty(strMor))
            {
                GameMainManager.Instance.UIRoot.ShowToastForICM(strMor);
            }
        }
    }

    public void HideToastHint()
    {
        if (GameMainManager.Instance.UIRoot)
            GameMainManager.Instance.UIRoot.ShowToastHint(false, null);
    }

    private int lastScenarioDialogID = 0;
    private List<int> scenarioDialogsID = new List<int>();

    private void UpdateScenarioDialogsData(List<int> conversionIds, bool append = false)
    {
        if (!append)
        {
            lastScenarioDialogID = 0;
            scenarioDialogsID.Clear();
        }
        scenarioDialogsID.AddRange(conversionIds);
    }

    public bool PopScenarioDialogs()
    {
        bool hasAnyDialog = false;

        if (lastScenarioDialogID != 0)
        {
            // 如果当前是Selections Dialog则鼠标点击不退出该对话框模式,会有单独的按钮消息处理接下来的选择操作
            bool isLastDlgSelection = CheckUIDialogSelection(lastScenarioDialogID);
            if (isLastDlgSelection)
            {
                Debug.Log("This is in Select Dialog now.");
                hasAnyDialog = true;
                return hasAnyDialog;
            }

            HideUIDialogById(lastScenarioDialogID);
        }

        if (scenarioDialogsID.Count <= 0)
        {
            foreach (var kv in dictDlgSpecOldFaceTo)
            {
                int charId = kv.Key;
                EnumDirection direction = kv.Value;
                var foundNPC = CharactersManager.Instance.ScenarioFindCharById(charId);
                if (foundNPC != null)
                {
                    foundNPC.CharAnimRender.SetDirection(EnumAnimAction.Static, direction);
                }
            }
            dictDlgSpecOldFaceTo.Clear();

            lastScenarioDialogID = 0;
            return hasAnyDialog;
        }


        hasAnyDialog = true;
        int dialogID = scenarioDialogsID[0];
        ShowUIDialogById(dialogID);
        scenarioDialogsID.RemoveAt(0);
        lastScenarioDialogID = dialogID;

        return hasAnyDialog;
    }
    #endregion

    /// <summary>
    /// 设置Camera聚焦到目标角色并跟随(一般都是主角)
    /// </summary>
    /// <param name="charId"></param>
    public void SetCameraFollowTargetCharacter(int charId)
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            CameraFollow camFollow = cam.GetComponent<CameraFollow>();

            var character = CharactersManager.Instance.ScenarioFindCharById(charId);
            if (character != null)
                camFollow.FollowTarget = character.transform;
        }
    }

    public void BackToGamePlay()
    {
        CharactersManager.Instance.MainRoleController.FinishDialogWithNPC();
    }

    /// <summary>
    /// 跳转场景后设置主角的出生点和方向
    /// </summary>
    /// <param name="vecEntryBornPos"></param>
    /// <param name="vecDirection"></param>
    public void DeployMainRoleByPosDirection(Vector2 vecEntryBornPos, Vector2 vecDirection, bool bControlled = true)
    {
        if (CharactersManager.Instance.MainRoleController != null)
        {
            CharactersManager.Instance.MainRoleController.gameObject.SetActive(true);
            CharactersManager.Instance.MainRoleController.SwitchManualControlled(bControlled);
            if (bControlled)
                CharactersManager.Instance.MainRoleController.BornInScenario(vecEntryBornPos, vecDirection);
            else
                CharactersManager.Instance.MainRoleController.PlaceInScenario(vecEntryBornPos, vecDirection);
        }
    }

    public void DeployMainRoleInCurrentPos(Vector2 vecDirection, bool bControlled = true)
    {
        if (CharactersManager.Instance.MainRoleController != null)
        {
            Vector2 vecCurrentPos = new Vector2(CharactersManager.Instance.MainRoleController.transform.position.x, CharactersManager.Instance.MainRoleController.transform.position.y);
            CharactersManager.Instance.MainRoleController.gameObject.SetActive(true);
            CharactersManager.Instance.MainRoleController.SwitchManualControlled(bControlled);
            CharactersManager.Instance.MainRoleController.PlaceInScenario(vecCurrentPos, vecDirection);
        }
    }



    public void DisappearDialogWhenClicked()
    {
        GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;
        BackToGamePlay();
    }

    /// <summary>
    /// 根据当前的Task找到其对应的所属者
    /// </summary>
    /// <param name="taskItem"></param>
    /// <returns></returns>
    public TaskHandler ParseObjScenarioTaskHandler(StoryTaskItem taskItem)
    {
        EnumTaskObjBelongType belongType = (EnumTaskObjBelongType)Enum.Parse(typeof(EnumTaskObjBelongType), taskItem.BelongObjType);
        if (belongType == EnumTaskObjBelongType.Char)
        {
            var targetTaskChar = CharactersManager.Instance.ScenarioFindCharById(taskItem.BelongObjId);
            if (targetTaskChar != null)
            {
                return targetTaskChar.HandleScenarioTask;
            }
        }
        else if (belongType == EnumTaskObjBelongType.HiddenObj)
        {
            var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
            var targetTaskHiddenObj = curScenarioMap.HiddenObjects.FirstOrDefault(x => x.CharID == taskItem.BelongObjId);
            if (targetTaskHiddenObj != null)
            {
                return targetTaskHiddenObj.HandleScenarioTask;
            }
        }
        else if (belongType == EnumTaskObjBelongType.Entry)
        {
            var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
            var foundEntry = curScenarioMap.Entries.FirstOrDefault(x => x.entryId == taskItem.BelongObjId);
            if (foundEntry != null)
            {
                return foundEntry.HandleScenarioTask;
            }
        }
        else if (belongType == EnumTaskObjBelongType.Scene)
        {
            var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
            if (curScenarioMap != null && curScenarioMap.HandleScenarioTask != null)
                return curScenarioMap.HandleScenarioTask;
            else
            {
                Debug.LogWarningFormat("Can't find Handle Task of Scene by task id = {0}", taskItem.Id);
            }
        }

        return null;
    }

    private List<int> specialContinueDoTasksList = new List<int>();
    private List<SchemaFunction> specialTotalPostSubTasksList = new List<SchemaFunction>();

    private List<SchemaFunction> SpecialHandlePostTasks(List<SchemaFunction> funcList)
    {
        // 特殊函数的处理,ChkScenariosRunningToDoTasks/ChkItemGivenToCharDoTasks只能同时处理一个函数
        // 如果已经有特殊函数正在处理,则跳过如下处理,防止死循环
        if (specialContinueDoTasksList.Count == 0)
        {
            var foundSpecFun1 = funcList.Find(x => string.Compare(x.FuncName, EnumScenarioPostTasks.ChkScenariosRunningToDoTasks.ToString()) == 0);
            //var foundSpecFun2 = funcList.Find(x => string.Compare(x.FuncName, EnumScenarioPostTasks.ChkItemsOwnedByCharToDoTasks.ToString()) == 0);
            List<SchemaFunction> foundSpecFuns = new List<SchemaFunction>();
            if (foundSpecFun1 != null)
                foundSpecFuns.Add(foundSpecFun1);
            //if (foundSpecFun2 != null)
            //    foundSpecFuns.Add(foundSpecFun2);
            SchemaFunction foundSpecFun = null;
            if (foundSpecFuns.Count > 0) // TODO:同一时间只能有一个特殊PostTask进行处理
                foundSpecFun = foundSpecFuns[0];
            if (foundSpecFun != null)
            {
                EnumScenarioPostTasks specFunType = (EnumScenarioPostTasks)Enum.Parse(typeof(EnumScenarioPostTasks), foundSpecFun.FuncName);
                if (specFunType == EnumScenarioPostTasks.ChkScenariosRunningToDoTasks)
                {
                    var specFunParams = foundSpecFun.FuncParams;
                    Dictionary<int, string> dictToChkScenariosRunning = new Dictionary<int, string>();
                    foreach (var param in specFunParams)
                    {
                        List<string> specParams = SchemaParser.ParseParamToStringList(param, SchemaParser.SPLITTER_PAIR);
                        if (specParams.Count < 2)
                        {
                            continue;
                        }

                        int toChkScenariosId = int.Parse(specParams[0]);
                        specParams.RemoveAt(0);
                        var strValue = specParams.Count > 1
                            ? string.Join(SchemaParser.SPLITTER_PARAM.ToString(), specParams)
                            : specParams[0];
                        if (dictToChkScenariosRunning.ContainsKey(toChkScenariosId))
                        {
                            dictToChkScenariosRunning[toChkScenariosId] = strValue;
                        }
                        else
                        {
                            dictToChkScenariosRunning.Add(toChkScenariosId, strValue);
                        }
                    }

                    specialTotalPostSubTasksList.Clear();
                    List<int> defaultToPushTasks = new List<int>();
                    foreach (var dictItem in dictToChkScenariosRunning)
                    {
                        int key = dictItem.Key;
                        if (key != 0)
                        {
                            var tasksId = SchemaParser.ParseParamToInts(dictItem.Value);
                            if (tasksId.Count == 1)
                            {
                                int taskId = tasksId[0];
                                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(key);
                                if (scenarioStatus == EnumScenarioStatus.Running)
                                {
                                    specialContinueDoTasksList.Add(taskId);

                                    var storyTaskItem = GetTaskItemParamById(taskId);
                                    var toFuncList = SchemaParser.ParseParamToFunctionsSingle(storyTaskItem.PostTasks);
                                    specialTotalPostSubTasksList.AddRange(toFuncList);
                                }
                            }
                        }
                        else
                        {
                            var tasksId = SchemaParser.ParseParamToInts(dictItem.Value);
                            if (tasksId.Count == 1)
                            {
                                int taskId = tasksId[0];
                                defaultToPushTasks.Add(taskId); // 理论上只有一个默认结束标志
                            }
                        }
                    }

                    specialContinueDoTasksList.AddRange(defaultToPushTasks);
                }
                //else if (specFunType == EnumScenarioPostTasks.ChkItemsOwnedByCharToDoTasks)
                //{
                //    var specFunParams = foundSpecFun.FuncParams;
                //    int finalTaskId = 0;
                //    bool allItemsOwned = true;
                //    foreach (var param in specFunParams)
                //    {
                //        var itemParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                //        if (itemParams.Count >= 3)
                //        {
                //            int itemId = itemParams[0];
                //            int ownerCharId = itemParams[1];
                //            int itemCount = itemParams[2];
                //            if (itemParams.Count > 3)
                //            {
                //                int taskId = itemParams[3];
                //            }

                //            if (!DocumentDataManager.Instance.TaskIsCharOwnItemCount(ownerCharId, itemId, itemCount))
                //            {
                //                allItemsOwned = false;
                //                break;
                //            }
                //        }
                //    }

                //    //allItemsOwned = true;
                //    if (allItemsOwned)
                //    {
                //        // 荀彧5本书找齐
                //        foreach (var param in specFunParams)
                //        {
                //            var defaultParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                //            if (defaultParams.Count == 2)
                //            {
                //                int id = defaultParams[0];
                //                int taskId = defaultParams[1]; // 荀彧任务完成后对话
                //                if (id == 0)
                //                {
                //                    finalTaskId = taskId;
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // 检测是否拥有荀彧未获得的书
                //        foreach (var param in specFunParams)
                //        {
                //            var itemParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                //            if (itemParams.Count == 4)
                //            {
                //                int itemId = itemParams[0];
                //                int ownerCharId = itemParams[1];
                //                int itemCount = itemParams[2];
                //                int taskId = itemParams[3];

                //                if (!DocumentDataManager.Instance.TaskIsCharOwnItemCount(ownerCharId, itemId, itemCount))
                //                {
                //                    int existItemCount = DocumentDataManager.Instance.GetCountOfTKRItemInPackage(itemId);
                //                    if (existItemCount >= itemCount)
                //                    {
                //                        finalTaskId = taskId;
                //                        break;
                //                    }
                //                }
                //            }
                //        }

                //        if (finalTaskId == 0)
                //        {
                //            foreach (var param in specFunParams)
                //            {
                //                var defaultParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                //                if (defaultParams.Count == 2)
                //                {
                //                    int id = defaultParams[0];
                //                    int taskId = defaultParams[1]; // 荀彧任务完成后对话
                //                    if (id == -1)
                //                    {
                //                        finalTaskId = taskId;
                //                        break;
                //                    }
                //                }
                //            }
                //        }
                //    }

                //    specialTotalPostSubTasksList.Clear();
                //    specialContinueDoTasksList.Add(finalTaskId);
                //}
            }
        }

        if (specialContinueDoTasksList.Count > 0)
        {
            funcList.Clear();

            int targetTaskId = specialContinueDoTasksList[0];
            specialContinueDoTasksList.RemoveAt(0);

            bool isOnlyRepeatPushTask = false;
            if (specialContinueDoTasksList.Count == 0)
            {
                if (specialTotalPostSubTasksList.Count > 0)
                {
                    funcList.AddRange(specialTotalPostSubTasksList);
                    specialTotalPostSubTasksList.Clear();
                    isOnlyRepeatPushTask = true;
                }
            }

            funcList.Add(new SchemaFunction()
            {
                FuncName = isOnlyRepeatPushTask ? EnumScenarioPostTasks.PushTasks.ToString() : EnumScenarioPostTasks.DoTasks.ToString(),
                FuncParams = new List<string>() { targetTaskId.ToString() }
            });
        }

        return funcList;
    }

    public bool IsCheckingItemsOwner { private set; get; }= false;
    public void FinishPostTasks()
    {
        IsCheckingItemsOwner = false;
        if (curDoingTaskItem != null)
        {
            var funcList = SchemaParser.ParseParamToFunctionsSingle(curDoingTaskItem.PostTasks);
            funcList = SpecialHandlePostTasks(funcList);

            // 1.为了处理有选项的对话
            bool chkSelectOne = false;
            if (lastDialogSelectedIndex >= 0)
            {
                chkSelectOne = true;
            }

            int funcIndex = 0;
            foreach (var f in funcList)
            {
                EnumScenarioPostTasks postTasks = (EnumScenarioPostTasks)Enum.Parse(typeof(EnumScenarioPostTasks), f.FuncName);
                // 2.为了处理有选项的对话
                if (chkSelectOne)
                {
                    if (lastDialogSelectedIndex != funcIndex)
                    {
                        funcIndex++;
                        continue;
                    }
                }
                funcIndex++;

                switch (postTasks)
                {
                    case EnumScenarioPostTasks.Repeat:
                        {
                            var handleTask = ParseObjScenarioTaskHandler(curDoingTaskItem);
                            if (handleTask != null)
                            {
                                handleTask.PushTaskData(curDoingTaskItem.Id);
                            }
                            else
                            {
                                Debug.LogErrorFormat("Can't find Task Handler with TaskId = {0} in Repeat", curDoingTaskItem.Id);
                            }
                            curDoingTaskItem = null;
                        }
                        break;
                    case EnumScenarioPostTasks.PushTasks:
                        {
                            var postTasksList = f.FuncParams.Select(int.Parse).ToList();
                            foreach (var taskId in postTasksList)
                            {
                                var postTask = GetTaskItemParamById(taskId);
                                var handleTask = ParseObjScenarioTaskHandler(postTask);
                                if (handleTask != null)
                                {
                                    handleTask.PushTaskData(postTask.Id);
                                }
                                else
                                {
                                    Debug.LogErrorFormat("Can't find Task Handler with TaskId = {0} in PushTasks", postTask.Id);
                                }
                            }

                            curDoingTaskItem = null;
                        }
                        break;
                    case EnumScenarioPostTasks.UseMoneyDoConversations:
                        {
                            if (f.FuncParams.Count > 0)
                            {
                                var useMoneyParams = SchemaParser.ParseParamToInts(f.FuncParams[0], SchemaParser.SPLITTER_PAIR);
                                if (useMoneyParams.Count >= 3)
                                {
                                    int useMoneyValue = useMoneyParams[0];
                                    int nextTaskId = useMoneyParams[1];
                                    int noMoneyToastId = useMoneyParams[2];

                                    // TODO:DEBUG
                                    //DocumentDataManager.Instance.GetCurrentDocument().Gold = 10000;

                                    uint curGold = DocumentDataManager.Instance.GetCurrentDocument().Gold;
                                    if (curGold < useMoneyValue)
                                    {
                                        EnumToastType toastType = EnumToastType.Money;
                                        ShowToastHint(noMoneyToastId, toastType, useMoneyValue);
                         
                                        var nextTask = GetTaskItemParamById(curDoingTaskItem.Id);
                                        var handleTask = ParseObjScenarioTaskHandler(nextTask);
                                        handleTask.PushTaskData(nextTask.Id);

                                        curDoingTaskItem = null;
                                    }
                                    else
                                    {
                                        DocumentDataManager.Instance.UpdateGoldByValue(-useMoneyValue);

                                        var nextTask = GetTaskItemParamById(nextTaskId);
                                        var handleTask = ParseObjScenarioTaskHandler(nextTask);

                                        handleTask.PushTaskData(nextTask.Id);
                                        DoScenarioTaskWithOther(handleTask);
                                    }
                                }
                            }
                        }
                        break;
                    case EnumScenarioPostTasks.DoTasks: // 理论上同一场景只能有一个task
                        {
                            curDoingTaskItem = null;
                            var postTasksList = f.FuncParams.Select(int.Parse).ToList();
                            if (postTasksList.Count != 1)
                            {
                                Debug.LogWarning("DoTasks but with more than one task");
                            }
                            // 某些Task可能是JumpToTask,会导致直接进入FinishTask而错误的使用了lastDialogSelectedIndex = -1;
                            lastDialogSelectedIndex = -1;
                            foreach (var taskId in postTasksList)
                            {
                                var nextTask = GetTaskItemParamById(taskId);
                                var handleTask = ParseObjScenarioTaskHandler(nextTask);
                                handleTask.PushTaskData(nextTask.Id);
                                DoScenarioTaskWithOther(handleTask);
                            }
                        }
                        break;
                    case EnumScenarioPostTasks.NextScenarios: // 在当前场景中开启Next Scenario后记录触发当前场景的可执行Scenarios
                    {
                        // Pop Self
                        //int curTaskId = curDoingTaskItem.Id;
                        //var handleTask = ParseObjScenarioTaskHandler(curDoingTaskItem);
                        //if (handleTask != null)
                        //    handleTask.PopTaskData();


                        int myParentScenarioId = curDoingTaskItem.BelongScenario;
                        var scenario = GetScenarioItemById(myParentScenarioId);
                        var nextScenarioIds = SchemaParser.ParseParamToInts(scenario.NextScenarioIds);

                        foreach (var id in nextScenarioIds)
                        {
                            DocumentDataManager.Instance.OpenScenarioById(id);
                        }

                        DocumentDataManager.Instance.CloseScenarioById(myParentScenarioId);

                        curDoingTaskItem = null;

                        var scenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                        // 当前场景中直接运行NextScenario
                        RunSceneScenario(scenarioMap.SceneId);
                    }
                        break;
                    case EnumScenarioPostTasks.NextScene: // 自带开启Next Scenario的功能
                    {
                        int myParentScenarioId = curDoingTaskItem.BelongScenario;
                        var scenario = GetScenarioItemById(myParentScenarioId);
                        var nextScenarioIds = SchemaParser.ParseParamToInts(scenario.NextScenarioIds);

                        foreach (var id in nextScenarioIds)
                        {
                            DocumentDataManager.Instance.OpenScenarioById(id);
                        }

                        UpdateDocumentStoryNotes(myParentScenarioId);
                        DocumentDataManager.Instance.CloseScenarioById(myParentScenarioId);

                        curDoingTaskItem = null;

                        CharactersManager.Instance.ScenarioUnLoadCharsGameObjects();
                        int nextSceneId = 0;
                        int nextEntryId = 0;

                        var postTasksList = f.FuncParams.Select(int.Parse).ToList();
                        if (postTasksList.Count == 1) // 只能跳转到一个场景
                        {
                            nextSceneId = postTasksList[0];
                        }
                        else if (postTasksList.Count == 2)
                        {
                            nextSceneId = postTasksList[0];
                            nextEntryId = postTasksList[1];
                        }
                        else
                        {
                            Debug.LogErrorFormat("NextScene, but next scenes ids is not One. Current Scenario id = {0}", myParentScenarioId);
                        }

                        if (nextSceneId == 0)
                        {
                            Debug.LogWarning("NextScene parameter is not correct.");
                        }

                        GameMainManager.Instance.NextSceneId = nextSceneId;
                        GameMainManager.Instance.NextEntryId = nextEntryId;
                        GameMainManager.Instance.SwitchToNextScene();
                    }
                        break;
                    case EnumScenarioPostTasks.InitGameEvents: // 开启除了开场剧情以外的其他所有事件起点
                    {
                        int myParentScenarioId = curDoingTaskItem.BelongScenario;
                        var scenario = GetScenarioItemById(myParentScenarioId);
                        var nextScenarioIds = SchemaParser.ParseParamToInts(scenario.NextScenarioIds);

                        foreach (var id in nextScenarioIds)
                        {
                            DocumentDataManager.Instance.OpenScenarioById(id);
                        }

                        UpdateDocumentStoryNotes(myParentScenarioId);
                        DocumentDataManager.Instance.CloseScenarioById(myParentScenarioId);

                        curDoingTaskItem = null;
                    }
                        break;
                    case EnumScenarioPostTasks.UpdateTasks: // 更新存档中的Tasks信息
                    {
                        var toUpdateTasksId = f.FuncParams.Select(int.Parse).ToList();

                        // 先清除存档中的现存的Tasks
                        foreach (var id in toUpdateTasksId)
                        {
                            DocumentDataManager.Instance.ClearScenarioTaskById(id);
                        }

                        foreach (var id in toUpdateTasksId)
                        {
                            DocumentDataManager.Instance.UpdateScenarioTaskById(id);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.Pop: // 弹出当前执行的Task
                    {
                        if (curDoingTaskItem != null)
                        {
                            int curTaskId = curDoingTaskItem.Id;
                            var handleTask = ParseObjScenarioTaskHandler(curDoingTaskItem);
                            if (handleTask != null)
                                handleTask.PopTaskData();

                            curDoingTaskItem = null;
                            DocumentDataManager.Instance.ClearScenarioTaskById(curTaskId);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.OpenBattle:
                    {
                        if (f.FuncParams.Count == 1)
                        {
                            var strBattleParam = f.FuncParams[0];

                            int sceneId = 0;
                            int battleFieldId = 0;

                            if (SchemaParser.ParseParamAsPairInt(strBattleParam, out sceneId, out battleFieldId))
                            {
                                GameMainManager.Instance.NextSceneId = sceneId;
                                GameMainManager.Instance.NextEntryId = battleFieldId;
                                GameMainManager.Instance.SwitchToNextScene();
                            }
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.CloseScenarios:
                    {
                        var scenarios = f.FuncParams.Select(int.Parse).ToList();
                        foreach (var scenarioId in scenarios)
                        {
                            DocumentDataManager.Instance.CloseScenarioById(scenarioId);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.OpenScenarios:
                    {
                        var scenarios = f.FuncParams.Select(int.Parse).ToList();
                        foreach (var scenarioId in scenarios)
                        {
                            DocumentDataManager.Instance.OpenScenarioById(scenarioId);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.ChkScenariosRunningToDoTasks:
                    {

                    }
                        break;
                    case EnumScenarioPostTasks.OpenLittleGameUI:
                    {
                        if (f.FuncParams.Count >= 1)
                        {
                            var data = SchemaParser.ParseParamToStringList(f.FuncParams[0], SchemaParser.SPLITTER_PAIR);
                            string strGameUI = data[0];
                            int nGameUIParam = 0;
                            if (data.Count > 1)
                                nGameUIParam = int.Parse(data[1]);
                            EnumTinyGameUI gameUI = (EnumTinyGameUI) Enum.Parse(typeof(EnumTinyGameUI), strGameUI);
                            if (gameUI == EnumTinyGameUI.PuzzleBaGua)
                            {
                                GameMainManager.Instance.CurGameMode = EnumGameMode.InLittleGameUI;
                                GameMainManager.Instance.UIRoot.DisplayUIPanelContent(
                                    UIGameRootCanvas.UI_PANEL_CONTENT.LittleGames_PuzzleBgGua, CallBackFunLittleGameUIPuzzleBaGua, nGameUIParam.ToString());
                            }
                            else if (gameUI == EnumTinyGameUI.PuzzleHuaRongDao)
                            {
                                GameMainManager.Instance.CurGameMode = EnumGameMode.InLittleGameUI;
                                GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.LittleGames_PuzzleHuaRongDao, CallBackFunLittleGameUIPuzzleHuaRongDao, nGameUIParam.ToString());
                            }
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.PopTasks:
                    {
                        var tasksId = f.FuncParams.Select(int.Parse).ToList();
                        foreach (var id in tasksId)
                        {
                            int curTaskId = id;
                            var postTask = GetTaskItemParamById(curTaskId);
                            var handleTask = ParseObjScenarioTaskHandler(postTask);
                            if (handleTask != null)
                                handleTask.PopTaskData();

                            DocumentDataManager.Instance.ClearScenarioTaskById(id);
                            if (curDoingTaskItem != null && curDoingTaskItem.Id == id)
                            {
                                curDoingTaskItem = null;
                            }
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.UnloadChars:
                    {
                        List<int> toUnloadChars = f.FuncParams.Select(int.Parse).ToList();
                        for (int i = 0; i < toUnloadChars.Count; i++)
                        {
                            int charId = toUnloadChars[i];
                            CharactersManager.Instance.ScenarioUnloadCharById(charId);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.ConsumeItemsToTaskChar:
                    {
                        foreach (var funParam in f.FuncParams)
                        {
                            var paramInts = SchemaParser.ParseParamToInts(funParam, SPLITTER_PAIR);
                            int charId = paramInts[0];
                            int itemId = paramInts[1];
                            int itemCount = paramInts[2];
                            DocumentDataManager.Instance.AddTKRItemToPackage(itemId, -itemCount);
                            DocumentDataManager.Instance.TaskOwnItemToChar(charId, itemId, itemCount);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.OpenSceneEntries:
                    {
                        List<int> toOpenEntriesId = f.FuncParams.Select(int.Parse).ToList();
                        var curScenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();
                        foreach (var id in toOpenEntriesId)
                        {
                            var foundEntry = curScenarioMap.Entries.FirstOrDefault(x => x.entryId == id);
                            if (foundEntry != null)
                            {
                                foundEntry.gameObject.SetActive(true);
                            }
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.GotItems:
                        {
                            var itemParams = SchemaParser.ParseParamToStringList(f.FuncParams[0], SPLITTER_PAIR);
                            EnumToastType itemType = (EnumToastType)Enum.Parse(typeof(EnumToastType), itemParams[0]);
                            if (itemType == EnumToastType.TKRItem)
                            {
                                if (itemParams.Count >= 2)
                                {
                                    int itemId = int.Parse(itemParams[1]);
                                    int itemCount = 1;
                                    if (itemParams.Count >= 3)
                                        itemCount = int.Parse(itemParams[2]);

                                    DocumentDataManager.Instance.AddTKRItemToPackage(itemId, itemCount);
                                    //if (itemParams.Count >= 4)
                                    //{
                                    //    int itemToCharID = int.Parse(itemParams[3]);
                                    //    if (itemToCharID > 0)
                                    //        DocumentDataManager.Instance.EquipTKRItemToChar(itemId, itemToCharID);
                                    //}
                                }
                            }
                            else if (itemType == EnumToastType.Money)
                            {
                                if (itemParams.Count >= 2)
                                {
                                    int moneyOffset = int.Parse(itemParams[1]);
                                    DocumentDataManager.Instance.UpdateGoldByValue(moneyOffset);
                                }
                            }
                        }
                        break;
                    case EnumScenarioPostTasks.ChkItemsOwnedByCharToDoTasks:
                    {
                        var specFunParams = f.FuncParams;
                        int finalTaskId = 0;
                        bool allItemsOwned = true;
                        foreach (var param in specFunParams)
                        {
                            var itemParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                            if (itemParams.Count >= 3)
                            {
                                int itemId = itemParams[0];
                                int ownerCharId = itemParams[1];
                                int itemCount = itemParams[2];
                                if (itemParams.Count > 3)
                                {
                                    int taskId = itemParams[3];
                                }

                                if (!DocumentDataManager.Instance.TaskIsCharOwnItemCount(ownerCharId, itemId, itemCount))
                                {
                                    allItemsOwned = false;
                                    break;
                                }
                            }
                        }

                        if (allItemsOwned)
                        {
                            // 荀彧5本书找齐
                            foreach (var param in specFunParams)
                            {
                                var defaultParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                                if (defaultParams.Count == 2)
                                {
                                    int id = defaultParams[0];
                                    int taskId = defaultParams[1]; // 荀彧任务完成后对话
                                    if (id == 0)
                                    {
                                        finalTaskId = taskId;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // 检测是否拥有荀彧未获得的书
                            foreach (var param in specFunParams)
                            {
                                var itemParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                                if (itemParams.Count >= 3)
                                {
                                    int itemId = itemParams[0];
                                    int ownerCharId = itemParams[1];
                                    int itemCount = itemParams[2];
                                    int taskId = 0;
                                    if (itemParams.Count > 3)
                                        taskId = itemParams[3];

                                    if (!DocumentDataManager.Instance.TaskIsCharOwnItemCount(ownerCharId, itemId, itemCount))
                                    {
                                        int existItemCount = DocumentDataManager.Instance.GetCountOfTKRItemInPackage(itemId);
                                        if (existItemCount >= itemCount)
                                        {
                                            finalTaskId = taskId;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (finalTaskId == 0)
                            {
                                foreach (var param in specFunParams)
                                {
                                    var defaultParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                                    if (defaultParams.Count == 2)
                                    {
                                        int id = defaultParams[0];
                                        int taskId = defaultParams[1];
                                        if (id == -1)
                                        {
                                            finalTaskId = taskId;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        int itemsOwnedByNPC = 0;
                        int itemsOwnedByMy = 0;
                        int allCheckItemOwnedCount = 0;
                        foreach (var param in specFunParams)
                        {
                            var itemParams = SchemaParser.ParseParamToInts(param, SchemaParser.SPLITTER_PAIR);
                            if (itemParams.Count >= 3)
                            {
                                allCheckItemOwnedCount++;

                                int itemId = itemParams[0];
                                int ownerCharId = itemParams[1];
                                int itemCount = itemParams[2];
                                if (!DocumentDataManager.Instance.TaskIsCharOwnItemCount(ownerCharId, itemId, itemCount))
                                {
                                    int existItemCount = DocumentDataManager.Instance.GetCountOfTKRItemInPackage(itemId);
                                    if (existItemCount >= itemCount)
                                    {
                                        itemsOwnedByMy++;
                                    }
                                }
                                else
                                {
                                    itemsOwnedByNPC++;
                                }
                            }
                        }

                        // 最后一次交物品任务的时候,将会完成整个物品任务
                        if (itemsOwnedByNPC == allCheckItemOwnedCount - 1 && (itemsOwnedByMy + itemsOwnedByNPC) == allCheckItemOwnedCount)
                        {
                            IsCheckingItemsOwner = true;
                        }


                        curDoingTaskItem = null;
                        // 某些Task可能是JumpToTask,会导致直接进入FinishTask而错误的使用了lastDialogSelectedIndex = -1;
                        lastDialogSelectedIndex = -1;
                        List<int> postTasksList = new List<int>();
                        postTasksList.Add(finalTaskId);
                        foreach (var taskId in postTasksList)
                        {
                            var nextTask = GetTaskItemParamById(taskId);
                            var handleTask = ParseObjScenarioTaskHandler(nextTask);
                            handleTask.PushTaskData(nextTask.Id);
                            DoScenarioTaskWithOther(handleTask);
                        }
                    }
                        break;
                    case EnumScenarioPostTasks.UpdateScenariosByTasksDone:
                        {
                            bool repeat = true;
                            if (f.FuncParams.Count == 3)
                            {
                                var testTaskId = int.Parse(f.FuncParams[0]);
                                var toCloseScenarios = SchemaParser.ParseParamToInts(f.FuncParams[1], SchemaParser.SPLITTER_PAIR);
                                var toOpenScenarios = SchemaParser.ParseParamToInts(f.FuncParams[2], SchemaParser.SPLITTER_PAIR);

                                bool running = DocumentDataManager.Instance.ChkTaskRunningById(testTaskId);
                                if (!running)
                                {
                                    foreach (var id in toCloseScenarios)
                                    {
                                        DocumentDataManager.Instance.CloseScenarioById(id);
                                    }
                                    foreach (var id in toOpenScenarios)
                                    {
                                        DocumentDataManager.Instance.OpenScenarioById(id);
                                    }
                                    repeat = true;
                                }
                            }

                            if (repeat)
                            {
                                var handleTask = ParseObjScenarioTaskHandler(curDoingTaskItem);
                                if (handleTask != null)
                                {
                                    handleTask.PushTaskData(curDoingTaskItem.Id);
                                }
                                else
                                {
                                    Debug.LogErrorFormat("Can't find Task Handler with TaskId = {0} in Repeat", curDoingTaskItem.Id);
                                }
                                curDoingTaskItem = null;
                            }
                        }
                        break;
                    case EnumScenarioPostTasks.ChkTasksRunningToDoTasks:
                        {
                            curDoingTaskItem = null;

                            if (f.FuncParams.Count == 1)
                            {
                                var paramsTasks = SchemaParser.ParseParamToStringList(f.FuncParams[0], SPLITTER_PAIR);
                                if (paramsTasks.Count == 2)
                                {
                                    var toTestTasks = SchemaParser.ParseParamToInts(paramsTasks[0], SPLITTER_COM);
                                    var toDoTasks = SchemaParser.ParseParamToInts(paramsTasks[1], SPLITTER_COM);

                                    bool allTasksRunning = true;
                                    for (int i = 0; i < toTestTasks.Count; i++)
                                    {
                                        var taskId = toTestTasks[i];
                                        bool isTaskRunning = DocumentDataManager.Instance.ChkTaskRunningById(taskId);
                                        if (!isTaskRunning)
                                        {
                                            allTasksRunning = false;
                                            break;
                                        }
                                    }

                                    if (allTasksRunning)
                                    {
                                        List<int> postTasksList = new List<int>();
                                        postTasksList.AddRange(toDoTasks);
                                        foreach (var taskId in postTasksList)
                                        {
                                            var nextTask = GetTaskItemParamById(taskId);
                                            var handleTask = ParseObjScenarioTaskHandler(nextTask);
                                            handleTask.PushTaskData(nextTask.Id);
                                            DoScenarioTaskWithOther(handleTask);
                                        }
                                    }
                                }
                            }
                            
                        }
                        break;
                    case EnumScenarioPostTasks.ChkTasksRunningToOpenScenarios:
                    {
                        curDoingTaskItem = null;
                        if (f.FuncParams.Count == 1)
                        {
                            var paramsScenarios = SchemaParser.ParseParamToStringList(f.FuncParams[0], SPLITTER_PAIR);
                            if (paramsScenarios.Count == 2)
                            {
                                var toTestTasks = SchemaParser.ParseParamToInts(paramsScenarios[0], SPLITTER_COM);
                                var toOpenScenarios = SchemaParser.ParseParamToInts(paramsScenarios[1], SPLITTER_COM);

                                bool allTasksRunning = true;
                                for (int i = 0; i < toTestTasks.Count; i++)
                                {
                                    var taskId = toTestTasks[i];
                                    bool isTaskRunning = DocumentDataManager.Instance.ChkTaskRunningById(taskId);
                                    if (!isTaskRunning)
                                    {
                                        allTasksRunning = false;
                                        break;
                                    }
                                }

                                if (allTasksRunning)
                                {
                                    List<int> toOpenScenariosList = new List<int>();
                                    toOpenScenariosList.AddRange(toOpenScenarios);
                                    foreach (var scenario in toOpenScenariosList)
                                    {
                                        DocumentDataManager.Instance.OpenScenarioById(scenario);
                                    }
                                }
                            }
                        }
                    }
                        break;
                    default:
                        break;
                }
            }

            lastDialogSelectedIndex = -1;
        }
    }

    private void CallBackFunLittleGameUIPuzzleBaGua(bool succ)
    {
        string strUIParam = GameMainManager.Instance.UIRoot.GetCurrentUIParam();
        if (!string.IsNullOrEmpty(strUIParam))
        {
            int targetUIParam = int.Parse(strUIParam);
            var littleGameParams = GetLittleGameItemParamByType(EnumTinyGameUI.PuzzleBaGua.ToString());
            var targetGameParam = littleGameParams.Find(x => x.Id == targetUIParam);
            if (targetGameParam != null)
            {
                var succParamsInts = SchemaParser.ParseParamToInts(targetGameParam.ResultSuccTasks);
                var failParamsInts = SchemaParser.ParseParamToInts(targetGameParam.ResultFailedTasks);
                int nSuccTaskFlag = succParamsInts[0];
                int succTaskId = succParamsInts[1];
                int nFailTaskFlag = failParamsInts[0];
                int failTaskId = failParamsInts[1];

                int targetTaskFlag;
                int targetTaskId;
                if (succ)
                {
                    targetTaskFlag = nSuccTaskFlag;
                    targetTaskId = succTaskId;
                }
                else
                {
                    targetTaskFlag = nFailTaskFlag;
                    targetTaskId = failTaskId;
                }

                if (targetTaskFlag == 1) // DoTasks
                {
                    curDoingTaskItem = null;

                    var nextTask = GetTaskItemParamById(targetTaskId);
                    var handleTask = ParseObjScenarioTaskHandler(nextTask);
                    handleTask.PushTaskData(nextTask.Id);
                    DoScenarioTaskWithOther(handleTask);
                }
                else // PushTasks
                {
                    var postTask = GetTaskItemParamById(targetTaskId);
                    var handleTask = ParseObjScenarioTaskHandler(postTask);
                    if (handleTask != null)
                    {
                        handleTask.PushTaskData(postTask.Id);
                    }
                    else
                    {
                        Debug.LogErrorFormat("Can't find Task Handler with TaskId = {0} in PushTasks", postTask.Id);
                    }
                }
            }
        }

        GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;
    }

    private void CallBackFunLittleGameUIPuzzleHuaRongDao(bool succ)
    {
        string strUIParam = GameMainManager.Instance.UIRoot.GetCurrentUIParam();
        if (!string.IsNullOrEmpty(strUIParam))
        {
            int targetUIParam = int.Parse(strUIParam);
            var littleGameParams = GetLittleGameItemParamByType(EnumTinyGameUI.PuzzleHuaRongDao.ToString());
            var targetGameParam = littleGameParams.Find(x => x.Id == targetUIParam);
            if (targetGameParam != null)
            {
                var succParamsInts = SchemaParser.ParseParamToInts(targetGameParam.ResultSuccTasks);
                var failParamsInts = SchemaParser.ParseParamToInts(targetGameParam.ResultFailedTasks);
                int nSuccTaskFlag = succParamsInts[0];
                int succTaskId = succParamsInts[1];
                int nFailTaskFlag = failParamsInts[0];
                int failTaskId = failParamsInts[1];

                int targetTaskFlag;
                int targetTaskId;
                if (succ)
                {
                    targetTaskFlag = nSuccTaskFlag;
                    targetTaskId = succTaskId;
                }
                else
                {
                    targetTaskFlag = nFailTaskFlag;
                    targetTaskId = failTaskId;
                }

                if (targetTaskFlag == 1) // DoTasks
                {
                    curDoingTaskItem = null;

                    var nextTask = GetTaskItemParamById(targetTaskId);
                    var handleTask = ParseObjScenarioTaskHandler(nextTask);
                    handleTask.PushTaskData(nextTask.Id);
                    DoScenarioTaskWithOther(handleTask);
                }
                else // PushTasks
                {
                    var postTask = GetTaskItemParamById(targetTaskId);
                    var handleTask = ParseObjScenarioTaskHandler(postTask);
                    if (handleTask != null)
                    {
                        handleTask.PushTaskData(postTask.Id);
                    }
                    else
                    {
                        Debug.LogErrorFormat("Can't find Task Handler with TaskId = {0} in PushTasks", postTask.Id);
                    }
                }
            }
        }

        GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;
    }

    public void EndDoingTaskTimeline()
    {
        FinishPostTasks();
    }

    /// <summary>
    /// 在最后一次UI操作弹出所有对话框内容后,结束当前的对话Task
    /// </summary>
    public void EndDoingTaskDialogs()
    {
        DisappearDialogWhenClicked();
        FinishPostTasks();
    }

    private List<int> currentRunningScenarioIds = new List<int>();

    /// <summary>
    /// 运行场景事件(剧情)
    /// </summary>
    /// <param name="sceneId">返回场景是否直接有动画执行</param>
    /// <returns></returns>
    public bool RunSceneScenario(int sceneId)
    {
        currentRunningScenarioIds.Clear();

        bool hasSceneScenarioAnim = false;
        // 从配置中过滤场景相关的剧情(正在执行的)
        List<int> selectedScenarios = SceneMapManager.Instance.GetScenariosIDList(sceneId); // 从SceneMap配置中获得scenario列表一定是EnumScenarioOwner.Scene类型的

        List<int> toRunScenarios = new List<int>();
        foreach (var selScenarioId in selectedScenarios)
        {
            var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(selScenarioId);
            if (scenarioStatus == EnumScenarioStatus.Running)
            {
                toRunScenarios.Add(selScenarioId);
            }
        }

        List<int> finalScenarios = new List<int>();
        foreach (var runScenario in toRunScenarios)
        {
            var item = GetScenarioItemById(runScenario);
            if (item != null)
            {
                // 条件参数必须为1个字符串
                var functionsParam = SchemaParser.ParseParamToFunctionsMultiParam(item.Conditions);
                if (functionsParam.Count > 0)
                {
                    bool allSucc = true;
                    for (int i = 0; i < functionsParam.Count; i++)
                    {
                        var funcName = functionsParam[i].FuncName;
                        var paramsList = functionsParam[i].FuncParams;
                        if (!MeasureScenarioCondition(runScenario, funcName, paramsList))
                        {
                            allSucc = false;
                        }
                    }
                    
                    if (allSucc)
                    {
                        finalScenarios.Add(runScenario);
                    }
                }
                else
                {
                    // 无条件则直接成功
                    finalScenarios.Add(runScenario);
                }
            }
        }


        if (finalScenarios.Count > 0)
        {
            Debug.LogFormat("Scene {0} has scenarios = {1}", sceneId, string.Join(",", finalScenarios));

            // 布置当前Scenario包含的人物
            for (int i = 0; i < finalScenarios.Count; i++)
            {
                int scenarioId = finalScenarios[i];
                var scenarioItem = GetScenarioItemById(scenarioId);
                if (scenarioItem != null)
                {
                    var deployIdsList = SchemaParser.ParseParamToInts(scenarioItem.DeployChars);
                    foreach (var deployId in deployIdsList)
                    {
                        DoDeploymentInScenario(deployId, scenarioId);
                    }

                    currentRunningScenarioIds.Add(scenarioId);
                }

                // 在部署人物后,根据存档向人物分配Task
                var foundDocScenario = DocumentDataManager.Instance.GetGameScenarioById(scenarioId);
                if (foundDocScenario != null && foundDocScenario.ScenarioDone == false && foundDocScenario.CurrentTasks != null)
                {
                    var tasksList = foundDocScenario.CurrentTasks.Values.ToList();
                    for (int j = 0; j < tasksList.Count; j ++)
                    {
                        int taskId = tasksList[j];
                        var taskItem = GetTaskItemParamById(taskId);
                        var handleTask = ParseObjScenarioTaskHandler(taskItem);
                        if (handleTask != null)
                        {
                            handleTask.InitTaskData(taskItem.Id);
                        }

                        // 一旦有场景Task则无需交互立马执行(一般来说都是场景动画,而且同一时间只有这一个Task)
                        EnumTaskObjBelongType belongType = (EnumTaskObjBelongType)Enum.Parse(typeof(EnumTaskObjBelongType), taskItem.BelongObjType);
                        if (belongType == EnumTaskObjBelongType.Scene)
                        {
                            if (handleTask == null)
                            {
                                Debug.LogErrorFormat("Scene to do scenario but no TaskHandler, taskId = {0}", taskId);
                                hasSceneScenarioAnim = false;
                            }
                            else
                            {
                                DoScenarioTaskWithOther(handleTask);
                                hasSceneScenarioAnim = true;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogFormat("Scene {0} had no scene scenario anim to run", sceneId);
            // 尝试加载
        }

        return hasSceneScenarioAnim;
    }

    public bool IsTaskHandlerContainPlayTimeline(TaskHandler taskHandler)
    {
        if (taskHandler == null)
        {
            return false;
        }

        var taskData = taskHandler.GetFrontTaskData();
        if (taskData != null)
        {
            var scenarioTask = GetTaskItemParamById(taskData.Id);
            if (scenarioTask != null)
            {
                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(taskData.BelongScenario);
                if (scenarioStatus == EnumScenarioStatus.Running)
                {
                    var funcList = SchemaParser.ParseParamToFunctionsSingle(scenarioTask.TaskContent);
                    foreach (var f in funcList)
                    {
                        EnumScenarioDoTaskContent doAction =
                            (EnumScenarioDoTaskContent) Enum.Parse(typeof(EnumScenarioDoTaskContent), f.FuncName);
                        if (doAction == EnumScenarioDoTaskContent.PlayTimeline)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    private StoryTaskItem curDoingTaskItem = null;
    /// <summary>
    /// 在触发了和NPC的TASK后执行
    /// </summary>
    /// <param name="npc"></param>
    public void DoScenarioTaskWithOther(TaskHandler objTaskHandler)
    {
        if (objTaskHandler == null)
        {
            Debug.LogError("Task Handler is Null");
            return;
        }
        var taskData = objTaskHandler.FrontTaskData();
        curDoingTaskItem = taskData;

        if (taskData != null)
        {
            var scenarioTask = GetTaskItemParamById(taskData.Id);
            if (scenarioTask != null)
            {
                var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(taskData.BelongScenario);
                if (scenarioStatus == EnumScenarioStatus.Running)
                {
                    var funcList = SchemaParser.ParseParamToFunctionsSingle(scenarioTask.TaskContent);

                    var scenarioMap = SceneMapManager.Instance.GetCurrentScenarioMap();

                    foreach (var f in funcList)
                    {
                        EnumScenarioDoTaskContent doAction = (EnumScenarioDoTaskContent)Enum.Parse(typeof(EnumScenarioDoTaskContent), f.FuncName);

                        switch (doAction)
                        {
                            case EnumScenarioDoTaskContent.PlayTimeline: // 场景触发的剧情直接执行
                                {
                                    if (f.FuncParams.Count == 1)
                                    {
                                        string targetTimelineName = f.FuncParams[0];
                                        scenarioMap.Timelines.PlayTimelineByScenarioName(targetTimelineName);
                                        GameMainManager.Instance.CurGameMode = EnumGameMode.RunningScenario;
                                    }
                                }
                                break;
                            case EnumScenarioDoTaskContent.ShowConversations:
                                {
                                    GameMainManager.Instance.CurGameMode = EnumGameMode.SceneTaskDialog;
                                    UpdateScenarioDialogsData(f.FuncParams.Select(int.Parse).ToList());
                                    PopScenarioDialogs();
                                }
                                break;
                            case EnumScenarioDoTaskContent.ShowSelections:
                                {
                                    GameMainManager.Instance.CurGameMode = EnumGameMode.SceneTaskDialog;
                                    UpdateScenarioDialogsData(f.FuncParams.Select(int.Parse).ToList());
                                    PopScenarioDialogs();
                                }
                                break;
                            case EnumScenarioDoTaskContent.JumpToPost:
                                {
                                    FinishPostTasks();
                                }
                                break;
                            case EnumScenarioDoTaskContent.ShowToast:
                            {
                                var funcParamList = SchemaParser.ParseParamToStringList(f.FuncParams[0], SPLITTER_PAIR);

                                int toastID = int.Parse(funcParamList[0]);
                                EnumToastType toastType = (EnumToastType)Enum.Parse(typeof(EnumToastType), funcParamList[1]);
                                if (toastType == EnumToastType.TKRItem)
                                {
                                    int itemID = int.Parse(funcParamList[2]);
                                    ScenarioManager.Instance.ShowToastHint(toastID, toastType, itemID);
                                }
                                else if (toastType == EnumToastType.Money)
                                {
                                    int moneyOffset = int.Parse(funcParamList[2]);
                                    ScenarioManager.Instance.ShowToastHint(toastID, toastType, moneyOffset);
                                }
                            }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                //Debug.LogErrorFormat("Not found Scenario. id = {0}", currentScenarioId);
            }
        }
    }

    private int lastDialogSelectedIndex = -1;
    public void OnDialogItemSelectedDoTask(int index)
    {
        lastDialogSelectedIndex = -1;
        if (lastScenarioDialogID != 0)
        {
            bool isLastDlgSelection = CheckUIDialogSelection(lastScenarioDialogID);
            if (isLastDlgSelection)
            {
                HideUIDialogById(lastScenarioDialogID);
                lastScenarioDialogID = 0;
                lastDialogSelectedIndex = index;
                EndDoingTaskDialogs();
            }
            else
            {
                Debug.LogErrorFormat("Last dialog id is not Selection type, {0}", lastScenarioDialogID);
            }
        }
        else
        {
            Debug.LogWarningFormat("Last dialog id is zero when select dialog item");
        }
    }

    public void SetLastScenarioDialogID(int id)
    {
        lastScenarioDialogID = id;
    }
}
