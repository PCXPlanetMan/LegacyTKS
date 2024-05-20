using System;
using System.Collections.Generic;
using System.Linq;
using com.tksr.data;
using com.tksr.property;
using com.tksr.schema;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

/// <summary>
/// 对应每个UnityScene的管理器：
/// 查询地图参数；检测地图类型；
/// 加载剧情相关资源配置(Timeline,对话等)；检测剧情进度触发剧情；场景元素交互；
/// 战斗地图资源配置加载；管理战斗逻辑(触发FSM)
/// </summary>
public class SceneMapManager : Singleton<SceneMapManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region Schema Config Data
    private SchemaSceneMap schemaSceneMap;

    /// <summary>
    /// 由GameAssetBundlesManager加载AB中的配置数据(可能早于Start)
    /// </summary>
    /// <param name="jsonSceneMap"></param>
    public void LoadScenarioSchema(string jsonSceneMap)
    {
        schemaSceneMap = JsonConvert.DeserializeObject<SchemaSceneMap>(jsonSceneMap);
    }

    public SceneMapItem GetSceneMapItemById(int Id)
    {
        if (schemaSceneMap == null)
        {
            Debug.LogErrorFormat("Scenario Data is not Loaded. id = {0}", Id);
            return null;
        }
        if (schemaSceneMap.maps.ContainsKey(Id.ToString()))
        {
            return schemaSceneMap.maps[Id.ToString()];
        }
        return null;
    }

    public SceneEntryItem GetEntryParamById(int Id)
    {
        if (schemaSceneMap == null)
        {
            Debug.LogErrorFormat("No Schema Scenario Data Loaded. id = {0}", Id);
            return null;
        }
        if (schemaSceneMap.entries.ContainsKey(Id.ToString()))
        {
            return schemaSceneMap.entries[Id.ToString()];
        }
        return null;
    }

    /// <summary>
    /// 通过地图上的出入点获取其所属的地图的信息
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public SceneMapItem GetScenarioParamByEntryId(int Id)
    {
        SceneEntryItem item = GetEntryParamById(Id);
        if (item == null)
        {
            Debug.LogErrorFormat("No found entry item. id = {0}", Id);
            return null;
        }
        int sceneId = item.BelongToScenario;
        return GetSceneMapItemById(sceneId);
    }

    /// <summary>
    /// 通过地图ID直接获取当前场景的类型
    /// </summary>
    /// <param name="Id">场景(Scene)地图的ID</param>
    /// <returns></returns>
    public EnumMapType GetScenarioMapTypeById(int Id)
    {
        var curScene = GetSceneMapItemById(Id);
        if (curScene != null)
        {
            EnumMapType mapType = (EnumMapType)Enum.Parse(typeof(EnumMapType), curScene.MapType);
            return mapType;
        }

        return EnumMapType.Invalid;
    }

    public int GetScenarioBgMusicById(int Id)
    {
        var curScene = GetSceneMapItemById(Id);
        if (curScene != null)
        {
            return curScene.BgMusicId;
        }

        return 0;
    }

    public List<int> GetScenariosIDList(int Id)
    {
        List<int> resultScenariosList = new List<int>();
        int sceneId = Id;
        var mapParam = GetSceneMapItemById(sceneId);
        if (mapParam != null)
        {
            resultScenariosList = SchemaParser.ParseParamToInts(mapParam.ScenariosList);
        }

        return resultScenariosList;
    }

    public BattleFieldItem GetBattleFieldById(int Id)
    {
        if (schemaSceneMap == null)
        {
            Debug.LogWarning("No Schema Scenario Data Loaded");
            return null;
        }
        if (schemaSceneMap.battlefields.ContainsKey(Id.ToString()))
        {
            return schemaSceneMap.battlefields[Id.ToString()];
        }
        return null;
    }
    #endregion

    #region 地图场景
    private ScenarioMap curScenarioMap;

    public int GetCurrentSceneId()
    {
        return curScenarioMap.SceneId;
    }

    public ScenarioMap GetCurrentScenarioMap()
    {
        return curScenarioMap;
    }

    /// <summary>
    /// 场景加载完成后的处理,包含资源配置和逻辑的开启
    /// </summary>
    /// <param name="sceneId"></param>
    public void EnterScene(int sceneId)
    {
        // Prepare GameObjects for Scene/Story/Timeline/Battle
        AttachScenarioMapInScene(sceneId);

        // Play the Map by Type (Scenario or Battle or BigMap?)
        PlaySceneMap();
    }

    /// <summary>
    /// 若是剧情地图则需要考虑是否创建剧情；若是战斗地图则需要初始化相关战斗数据
    /// </summary>
    private void PlaySceneMap()
    {
        EnumMapType mapType = GetScenarioMapTypeById(curScenarioMap.SceneId);
        if (mapType == EnumMapType.Scenarios || mapType == EnumMapType.BigMap)
        {
            PlayMapStory();
        }
        else if (mapType == EnumMapType.Battle)
        {
            PlayMapBattle();
        }
        else
        {
            Debug.LogErrorFormat("Scenario schema map type is error, sceneId = {0}, MapType = {1}", curScenarioMap.SceneId, mapType);
        }
    }

    /// <summary>
    /// 判断当前场景是剧情地图,则查询该地图上相关事件的执行情况,从而确定剧情动画或者场景人物的部署情况
    /// </summary>
    private void PlayMapStory()
    {
        var currentSceneId = GetCurrentSceneId();
        bool scenarioRunning = ScenarioManager.Instance.RunSceneScenario(currentSceneId);
        if (!scenarioRunning)
        {
            DeployMainRoleWhenNoSceneScenario();
        }
    }

    public void DeployMainRoleWhenNoSceneScenario()
    {
        // 部署主角
        if (GameMainManager.Instance.FromLoadGame)
        {
            PrepareMainRoleResAndDeploymentByDocument();
        }
        else
        {
            // 非加载存档模式
            PrepareMainRoleResAndDeploymentByEntry(true);
        }

        // 场景部署人物完成后设置GameMode以启动主角控制
        GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;

        // 设置镜头跟随主角
        ScenarioManager.Instance.SetCameraFollowTargetCharacter(ResourceUtils.MAINROLE_ID);

        // 如果没有事件发生,则播放当前场景地图的主题背景音乐
        int nBgMusicId = GetScenarioBgMusicById(curScenarioMap.SceneId);
        if (nBgMusicId != 0)
        {
            AudioManager.Instance.PlayScenarioBackgroundMusic(nBgMusicId);
        }
    }

    public void PrepareMainRoleResAndDeploymentByEntry(bool controlled)
    {
        if (CharactersManager.Instance.MainRoleController == null)
        {
            var goTeams = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainTeam.ToString());
            var teammates = goTeams.GetComponentsInChildren<CharMainController>(true);
            var foundMainRole = teammates.ToList().Find(x => x.CharID == ResourceUtils.MAINROLE_ID);
            CharactersManager.Instance.ScenarioLoadCharMainRoleGameObject(foundMainRole.gameObject);
            CharactersManager.Instance.MainRoleController.LoadRenderResource(true);
        }

        if (GameMainManager.Instance.EntryId != 0) // 必定是从上一个场景中通过Entry进入当前场景的,则计算其Entry的出生点和方向
        {
            Vector2 vecBorn = Vector2.zero;
            Vector2 vecDirection = Vector2.zero;
            bool bRes = LoadEntryBornPosition(GameMainManager.Instance.EntryId, ref vecBorn, ref vecDirection);
            if (bRes)
            {
                ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection, controlled);
            }
            else
            {
                Debug.LogFormat("Load Entry failed, entryId = {0}", GameMainManager.Instance.EntryId);
            }
        }
        else
        {
            Debug.LogFormat("No EntryId");
        }
    }

    private void PrepareMainRoleResAndDeploymentByDocument()
    {
        if (CharactersManager.Instance.MainRoleController == null)
        {
            var goTeams = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainTeam.ToString());
            var teammates = goTeams.GetComponentsInChildren<CharMainController>(true);
            var foundMainRole = teammates.ToList().Find(x => x.CharID == ResourceUtils.MAINROLE_ID);
            CharactersManager.Instance.ScenarioLoadCharMainRoleGameObject(foundMainRole.gameObject);
            CharactersManager.Instance.MainRoleController.LoadRenderResource(true);
        }

        var document = DocumentDataManager.Instance.GetCurrentDocument();
        Vector2 vecBorn = new Vector2(document.PosX, document.PosY);
        Vector2 vecDirection = Vector2.right;
        EnumDirection direction = EnumDirection.SE;
        if (!string.IsNullOrEmpty(document.Direction))
        {
            if (Enum.TryParse(document.Direction, out direction))
            {
                if (direction == EnumDirection.N)
                {
                    vecDirection = Vector2.up;
                }
                else if (direction == EnumDirection.E)
                {
                    vecDirection = Vector2.right;
                }
                else if (direction == EnumDirection.S)
                {
                    vecDirection = Vector2.down;
                }
                else if (direction == EnumDirection.W)
                {
                    vecDirection = Vector2.left;
                }
                else if (direction == EnumDirection.NE)
                {
                    vecDirection = new Vector2(1, 1);
                }
                else if (direction == EnumDirection.NW)
                {
                    vecDirection = new Vector2(-1, 1);
                }
                else if (direction == EnumDirection.SE)
                {
                    vecDirection = new Vector2(1, -1);
                }
                else if (direction == EnumDirection.SW)
                {
                    vecDirection = new Vector2(-1, -1);
                }
                ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection);
            }
            else
            {
                Debug.LogErrorFormat("Error Direction in Document: {0}", document.Direction);
            }
        }
        else
        {
            Debug.LogWarningFormat("No saved position in Document");
            bool bRes = RandomEntryBornPosition(ref vecBorn, ref vecDirection);
            if (bRes)
                ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection);
        }
    }
    #endregion

    public void SimulateLoadScenarioMap()
    {
        curScenarioMap = GameObject.FindGameObjectWithTag(EnumGameTagType.GOGameMap.ToString()).GetComponent<ScenarioMap>();
        if (curScenarioMap != null)
        {
            foreach (var entry in curScenarioMap.Entries)
            {
                entry.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("No found 'GOGameMap' tag GameObject");
        }
    }

    /// <summary>
    /// 在当前场景中记录地图资源节点
    /// </summary>
    /// <param name="sceneId"></param>
    private void AttachScenarioMapInScene(int sceneId)
    {
        curScenarioMap = GameObject.FindGameObjectWithTag(EnumGameTagType.GOGameMap.ToString()).GetComponent<ScenarioMap>();
        if (curScenarioMap != null)
        {
            curScenarioMap.SceneId = sceneId;
            foreach (var entry in curScenarioMap.Entries)
            {
                entry.gameObject.SetActive(true);
            }
        }
    }

    public bool LoadEntryBornPosition(int nEntryId, ref Vector2 vecBorn, ref Vector2 vecDirection)
    {
        if (nEntryId != 0 && curScenarioMap != null)
        {
            foreach (var entry in curScenarioMap.Entries)
            {
                if (entry.entryId == nEntryId)
                {
                    vecBorn = new Vector2(entry.BornPosition.position.x, entry.BornPosition.position.y);
                    vecDirection = new Vector2(vecBorn.x - entry.gameObject.transform.position.x, vecBorn.y - entry.gameObject.transform.position.y);
                    break;
                }
            }
            return true;
        }
        return false;
    }

    public bool RandomEntryBornPosition(ref Vector2 vecBorn, ref Vector2 vecDirection)
    {
        if (curScenarioMap != null && curScenarioMap.Entries.Length > 0)
        {
            var entry = curScenarioMap.Entries[Random.Range(0, curScenarioMap.Entries.Length)];
            vecBorn = new Vector2(entry.BornPosition.position.x, entry.BornPosition.position.y);
            vecDirection = new Vector2(vecBorn.x - entry.gameObject.transform.position.x, vecBorn.y - entry.gameObject.transform.position.y);
            return true;
        }
        return false;
    }


    public void DeployMainCharacter(bool autoFromTimeline = true, EnumDirection charDirection = EnumDirection.NE)
    {
        // 如果非延续剧情布置人物,则需要配置主角
        if (!autoFromTimeline)
        {
            if (GameMainManager.Instance.EntryId != 0) // 必定是从上一个场景中通过Entry进入当前场景的,则计算其Entry的出生点和方向
            {
                Vector2 vecBorn = Vector2.zero;
                Vector2 vecDirection = Vector2.zero;
                bool bRes = LoadEntryBornPosition(GameMainManager.Instance.EntryId, ref vecBorn, ref vecDirection);
                if (bRes)
                {
                    ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection);
                }
            }
            else
            {
                var document = DocumentDataManager.Instance.GetCurrentDocument();
                Vector2 vecBorn = new Vector2(document.PosX, document.PosY);
                Vector2 vecDirection = Vector2.right;
                EnumDirection direction = EnumDirection.SE;
                if (!string.IsNullOrEmpty(document.Direction))
                {
                    if (Enum.TryParse(document.Direction, out direction))
                    {
                        if (direction == EnumDirection.N)
                        {
                            vecDirection = Vector2.up;
                        }
                        else if (direction == EnumDirection.E)
                        {
                            vecDirection = Vector2.right;
                        }
                        else if (direction == EnumDirection.S)
                        {
                            vecDirection = Vector2.down;
                        }
                        else if (direction == EnumDirection.W)
                        {
                            vecDirection = Vector2.left;
                        }
                        else if (direction == EnumDirection.NE)
                        {
                            vecDirection = new Vector2(1, 1);
                        }
                        else if (direction == EnumDirection.NW)
                        {
                            vecDirection = new Vector2(-1, 1);
                        }
                        else if (direction == EnumDirection.SE)
                        {
                            vecDirection = new Vector2(1, -1);
                        }
                        else if (direction == EnumDirection.SW)
                        {
                            vecDirection = new Vector2(-1, -1);
                        }
                        ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection);
                    }
                    else
                    {
                        Debug.LogErrorFormat("Error Direction in Document: {0}", document.Direction);
                    }
                }
                else
                {
                    bool bRes = RandomEntryBornPosition(ref vecBorn, ref vecDirection);
                    if (bRes)
                        ScenarioManager.Instance.DeployMainRoleByPosDirection(vecBorn, vecDirection);
                }
            }
        }
        else
        {
            EnumDirection mainRoleDirection = charDirection;
            Vector2 vecDirection = Vector2.zero;
            if (mainRoleDirection == EnumDirection.N)
            {
                vecDirection = Vector2.up;
            }
            else if (mainRoleDirection == EnumDirection.E)
            {
                vecDirection = Vector2.right;
            }
            else if (mainRoleDirection == EnumDirection.S)
            {
                vecDirection = Vector2.down;
            }
            else if (mainRoleDirection == EnumDirection.W)
            {
                vecDirection = Vector2.left;
            }
            else if (mainRoleDirection == EnumDirection.NE)
            {
                vecDirection = new Vector2(1, 1);
            }
            else if (mainRoleDirection == EnumDirection.NW)
            {
                vecDirection = new Vector2(-1, 1);
            }
            else if (mainRoleDirection == EnumDirection.SE)
            {
                vecDirection = new Vector2(1, -1);
            }
            else if (mainRoleDirection == EnumDirection.SW)
            {
                vecDirection = new Vector2(-1, -1);
            }
            ScenarioManager.Instance.DeployMainRoleInCurrentPos(vecDirection);
        }
    }

    private void PlayMapBattle()
    {
        // 从当前场景中触发与之相关的战斗(剧情触发,大地图根据地域随机触发)
        int battleField = GameMainManager.Instance.EntryId;
        curScenarioMap.LoadMapParams();
        curScenarioMap.LoadBattleFieldDataAndRes(battleField);
        GameMainManager.Instance.CurGameMode = EnumGameMode.Battle;
    }

    public void ExitBattleScene(bool succ = true)
    {
        if (curScenarioMap != null)
        {
            int curBattleFieldId = curScenarioMap.GetCurrentBattleFieldId();
            var battleField = SceneMapManager.Instance.GetBattleFieldById(curBattleFieldId);
            if (battleField != null)
            {
                string strResultAction = battleField.ResultSuccAction;
                if (succ == false)
                {
                    strResultAction = battleField.ResultFailedAction;
                }
                ScenarioManager.Instance.PrepareScenarioAfterBattle(strResultAction);
            }
        }
    }

    public void LeaveScene()
    {
        if (curScenarioMap != null && curScenarioMap.Timelines != null)
            curScenarioMap.Timelines.HideAllGameEventIdentities();

        // Delete current playing map
        ReleaseCurrentPlayingMap();
    }

    private void ReleaseCurrentPlayingMap()
    {
        if (curScenarioMap == null || curScenarioMap.SceneId == 0)
        {
            Debug.LogWarning("Current Scenario Map is null or id = 0");
            return;
        }

        EnumMapType mapType = GetScenarioMapTypeById(curScenarioMap.SceneId);
        if (mapType == EnumMapType.Scenarios || mapType == EnumMapType.BigMap)
        {
            CharactersManager.Instance.ScenarioUnLoadCharsGameObjects();
        }
        else if (mapType == EnumMapType.Battle)
        {
            
        }
        else
        {
            Debug.LogErrorFormat("Scenario schema map type is error, sceneId = {0}, MapType = {1}", curScenarioMap.SceneId, mapType);
        }
    }
}
