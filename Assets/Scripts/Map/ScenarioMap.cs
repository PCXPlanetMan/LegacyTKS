using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using com.tksr.statemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

/// <summary>
/// 在每个场景中维护背景地图
/// </summary>
public class ScenarioMap : MonoBehaviour
{
    public int SceneId;
    public TimelinesContainer Timelines;

    private SceneMapItem param = null;
    public Grid GridMap;
    private CameraFollow camFollow;

    public TaskHandler HandleScenarioTask;

    private void Awake()
    {
        SetCameraLimitMapSize();
    }

    private CameraFollow SetCameraLimitMapSize()
    {
        // 提前设置相机的可视范围即是当前地图的像素尺寸
        SpriteRenderer sr = this.transform.GetComponent<SpriteRenderer>();
        Camera cam = Camera.main;
        if (cam != null)
        {
            camFollow = cam.GetComponent<CameraFollow>();
            if (camFollow != null && sr != null && sr.sprite != null) // 对于某些地图(例如剧情虚空),可能没有背景
            {
                camFollow.MapWidth = (int)sr.sprite.rect.width;
                camFollow.MapHeight = (int)sr.sprite.rect.height;
            }
        }

        return cam.GetComponent<CameraFollow>();
    }

    private void UpdateBattleMapBackgroundResource(Sprite battleField)
    {
        SpriteRenderer sr = this.transform.GetComponent<SpriteRenderer>();
        sr.sprite = battleField;
        Camera cam = Camera.main;
        if (cam != null)
        {
            camFollow = cam.GetComponent<CameraFollow>();
            if (camFollow != null && sr != null && sr.sprite != null) // 对于某些地图(例如剧情虚空),可能没有背景
            {
                camFollow.MapWidth = (int)sr.sprite.rect.width;
                camFollow.MapHeight = (int)sr.sprite.rect.height;
            }
        }
    }

    public ScenarioEntry[] Entries;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 加载地图(主要是战斗地图)的参数(状态机相关)
    /// </summary>
    public void LoadMapParams()
    {
        param = SceneMapManager.Instance.GetSceneMapItemById(SceneId);
        if (param != null)
        {
            EnumMapType mapType = (EnumMapType)Enum.Parse(typeof(EnumMapType), param.MapType);
            //Debug.LogFormat("Current Map Type = {0}", mapType.ToString());
            if (mapType == EnumMapType.Battle) // 战斗地图需要加载Tiles数据以及相关的表现元素
            {
                LoadGridTilesData();
                LoadSelectIndicatorTile();
                LoadGridBackground();
            }
        }
    }

    private const string LAYER_BATTLE_REACHABLE_REGION = "Tilemap - Collider";
    private const string LAYER_BATTLE_INDICATORS = "Tilemap - Level 1 - Base";
    private const string LAYER_BATTLE_BACKGROUND = "Tilemap - Ground - Base";

    /// <summary>
    /// 如果当前地图是战斗地图,则从TileMap中加载地图信息;
    /// 并同时启动状态机
    /// </summary>
    private void LoadGridTilesData()
    {
        if (GridMap != null)
        {
            // 获取战场可行走区域
            Transform transformTileMap = GridMap.transform.Find(LAYER_BATTLE_REACHABLE_REGION);
            if (transformTileMap != null)
            {
                Tilemap tileMap = transformTileMap.GetComponent<Tilemap>();
                if (tileMap != null)
                {
                    List<EncapsuleTile> list = new List<EncapsuleTile>();
                    foreach (var position in tileMap.cellBounds.allPositionsWithin)
                    {
                        if (!tileMap.HasTile(position))
                        {
                            continue;
                        }
                        TileBase tile = tileMap.GetTile(position);
                        EncapsuleTile tw = new EncapsuleTile(new Vector2Int(position.x, position.y), tile, GridMap);
                        list.Add(tw);
                        //Debug.LogFormat("tile.name = {0}, position = {1}", tile.name, position.ToString());
                    }

                    CreateStatesMachineControllers(list);
                }
            }
            else
            {
                Debug.LogError("No TileMap");
            }
        }
        else
        {
            Debug.LogError("No Grid");
        }
    }

    /// <summary>
    /// 根据已经加载Tiles信息启动战斗状态机
    /// 通过挂载和状态机交互的控制器来实现
    /// </summary>
    private void CreateStatesMachineControllers(List<EncapsuleTile> list)
    {
        // 战斗主状态机
        BattleController battle = GetComponent<BattleController>();
        if (battle == null)
        {
            battle = gameObject.AddComponent<BattleController>();
            battle.MapManager = this;
            battle.TilesData = list;
        }
        // 战斗相关UI交互控制器
        InputController input = GetComponent<InputController>();
        if (input == null)
        {
            input = gameObject.AddComponent<InputController>();
        }
        // 状态机属性参数监控器
        AutoStatusController autoStatus = GetComponent<AutoStatusController>();
        if (autoStatus == null)
        {
            autoStatus = gameObject.AddComponent<AutoStatusController>();
        }
    }

    /// <summary>
    /// 加载当前地图中的所有人物(敌我)
    /// 从存档以及当前的战斗地图中读取实时数据
    /// </summary>
    /// <returns></returns>
    public List<CharMainController> SMLoadAllCharsInBattle()
    {
        // 为了在整个地图中随机生成所有角色的位置,记录当前所有Tiles
        BattleController battle = GetComponent<BattleController>();
        List<EncapsuleTile> locations = new List<EncapsuleTile>(battle.BoardOfTilesMap.DictTiles.Values);
       

        List<CharMainController> listChars = new List<CharMainController>();

        var allMyCharsInfo = DocumentDataManager.Instance.FindCharsInfoInTeam();
        foreach (var charInfo in allMyCharsInfo)
        {
            int myCharId = charInfo.CharId;
            GameObject goMyChar = CharactersManager.Instance.FindCharacterById(myCharId);
            CharMainController cmcMyChar = goMyChar.GetComponent<CharMainController>();
            cmcMyChar.AttachedMap = this;
            cmcMyChar.CharLevel = (uint)charInfo.Level;

            // 随机生成出生点
            int random = Random.Range(0, locations.Count);
            EncapsuleTile randomTile = locations[random];
            locations.RemoveAt(random);
            cmcMyChar.BornPosOnTile = randomTile.VecPos;

            listChars.Add(cmcMyChar);
        }

        if (curBattleFieldId != 0)
        {
            var battileFieldParam = SceneMapManager.Instance.GetBattleFieldById(curBattleFieldId);
            if (battileFieldParam != null)
            {
                var mainRole = DocumentDataManager.Instance.FindCharInfoFromCandidates(ResourceUtils.MAINROLE_ID);
                int mainRoleLv = mainRole.Level;

                var required = battileFieldParam.RequiredChars;
                List<string> requiredCharList = new List<string>(required.Split(';'));

                // 随机生成出生点
                int random = Random.Range(0, locations.Count);
                EncapsuleTile randomTile = locations[random];
                locations.RemoveAt(random);

                var requiredCharsResult = ParseBattleCharacterData(requiredCharList, mainRoleLv, randomTile.VecPos);
                listChars.AddRange(requiredCharsResult);


                var optional = battileFieldParam.OptionalTemplateChars;
                List<string> optionalCharTemplateList = new List<string>(optional.Split(';'));
                int nMin = battileFieldParam.OptionalMin;
                int nMax = battileFieldParam.OptionalMax;
                int nOptionalCharsCount = Random.Range(nMin, nMax + 1);
                List<string> optionalCharList = new List<string>();
                for (int i = 0; i < nOptionalCharsCount; i ++)
                {
                    int rand = Random.Range(0, optionalCharTemplateList.Count);
                    optionalCharList.Add(optionalCharTemplateList[rand]);
                }

                // 随机生成出生点
                random = Random.Range(0, locations.Count);
                randomTile = locations[random];
                locations.RemoveAt(random);

                var optionalCharsResult = ParseBattleCharacterData(optionalCharList, mainRoleLv, randomTile.VecPos);
                listChars.AddRange(optionalCharsResult);
            }
        }

        return listChars;
    }

    /// <summary>
    /// 从配置中读取对战方角色信息并辅以当前人物等级或角色本身的属性加成进行修正
    /// </summary>
    /// <param name="listChars"></param>
    /// <param name="mainRoleLv"></param>
    /// <returns></returns>
    private List<CharMainController> ParseBattleCharacterData(List<string> listChars, int mainRoleLv, Vector2Int bornPos)
    {
        List<CharMainController> resultList = new List<CharMainController>();
        for (int i = 0; i < listChars.Count; i++)
        {
            var charData = listChars[i];
            var charParams = charData.Split(',');
            if (charParams.Length == 3)
            {
                int charId = int.Parse(charParams[0]);
                int charRatio = int.Parse(charParams[1]); // TODO:对角色进行属性修正
                int charFixedLv = int.Parse(charParams[2]);

                GameObject goEnemyChar = CharactersManager.Instance.FindCharacterById(charId);
                CharMainController cmcEnemyChar = goEnemyChar.GetComponent<CharMainController>();
                cmcEnemyChar.AttachedMap = this;
                if (charFixedLv > 0)
                    cmcEnemyChar.CharLevel = (uint) charFixedLv;
                else
                    cmcEnemyChar.CharLevel = (uint) mainRoleLv;
                cmcEnemyChar.BornPosOnTile = bornPos;
                resultList.Add(cmcEnemyChar);
            }
            else
            {
                Debug.LogErrorFormat("Battle Character Data format error: {0}", charData);
            }
        }

        return resultList;
    }

    /// <summary>
    /// 用选择框表示地图上当前的选中地块
    /// </summary>
    /// <param name="vecCellPos"></param>
    public void ShowSelectIndicatorOnTileMap(Vector2Int vecCellPos)
    {
        if (selectedTile != null && tileMapOfIndicator != null)
        {
            Vector3Int vecPos = new Vector3Int(vecCellPos.x, vecCellPos.y, 0);
            tileMapOfIndicator.SetTile(vecPos, selectedTile);
            tileMapOfIndicator.SetTile(new Vector3Int(lastVecSelectedTile.x, lastVecSelectedTile.y, 0), null);
            lastVecSelectedTile = vecCellPos;
        }
    }

    /// <summary>
    /// 高亮显示Tile
    /// </summary>
    /// <param name="vecCellPos"></param>
    public void HighlightSelectedTile(Vector2Int vecCellPos)
    {
        Vector3Int vecPos = new Vector3Int(vecCellPos.x, vecCellPos.y, 0);
        tileMapBackground.SetTile(vecPos, highlightTile);
    }

    /// <summary>
    /// 取消某高亮显示的Tile
    /// </summary>
    /// <param name="vecCellPos"></param>
    public void UnHighlightSelectedTile(Vector2Int vecCellPos)
    {
        Vector3Int vecPos = new Vector3Int(vecCellPos.x, vecCellPos.y, 0);
        tileMapBackground.SetTile(vecPos, null);
    }

    /// <summary>
    /// Make camera follow the target in Battle
    /// </summary>
    /// <param name="target"></param>
    public void SetCameraFollowTargetOnTileMap(Transform target)
    {
        if (camFollow != null)
        {
            camFollow.FollowTarget = target;
        }
    }

    public void ManualMoveCamera(Vector2Int offset)
    {
        if (camFollow != null)
        {
            camFollow.ExploreBattleMap(offset);
        }
    }

    private TileBase selectedTile;
    private Tilemap tileMapOfIndicator;
    private Vector2Int lastVecSelectedTile;
    /// <summary>
    /// 获得"选择"标识
    /// </summary>
    private void LoadSelectIndicatorTile()
    {
        if (GridMap != null)
        {
            Transform transformTileMap = GridMap.transform.Find(LAYER_BATTLE_INDICATORS);
            if (transformTileMap != null)
            {
                Tilemap tileMap = transformTileMap.GetComponent<Tilemap>();
                if (tileMap != null)
                {
                    tileMapOfIndicator = tileMap;
                    foreach (var position in tileMap.cellBounds.allPositionsWithin)
                    {
                        if (!tileMap.HasTile(position))
                        {
                            continue;
                        }
                        TileBase tile = tileMap.GetTile(position);
                        //Debug.LogFormat("tile.name = {0}, position = {1}", tile.name, position.ToString());
                        selectedTile = tile;
                        lastVecSelectedTile = new Vector2Int(position.x, position.y);

                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("No TileMap");
            }
        }
        else
        {
            Debug.LogError("No Grid");
        }
    }

    private readonly string TILE_RES_COLLIDER = "tile_battle_collider";
    private readonly string TILE_RES_HIGHLIGHT = "tile_battle_blue_alpha";

    private TileBase highlightTile;
    private TileBase tipTile;
    private Tilemap tileMapBackground;
    private void LoadGridBackground()
    {
        if (GridMap != null)
        {
            Transform transformTileMap = GridMap.transform.Find(LAYER_BATTLE_BACKGROUND);
            if (transformTileMap != null)
            {
                Tilemap tileMap = transformTileMap.GetComponent<Tilemap>();
                if (tileMap != null)
                {
                    tileMapBackground = tileMap;
                    foreach (var position in tileMap.cellBounds.allPositionsWithin)
                    {
                        if (!tileMap.HasTile(position))
                        {
                            continue;
                        }
                        TileBase tile = tileMap.GetTile(position);
                        //Debug.LogFormat("tile.name = {0}, position = {1}", tile.name, position.ToString());
                        if (tile.name.CompareTo(TILE_RES_HIGHLIGHT) == 0)
                            highlightTile = tile;
                        else if (tile.name.CompareTo(TILE_RES_COLLIDER) == 0)
                            tipTile = tile;
                    }
                }
            }
            else
            {
                Debug.LogError("No TileMap");
            }
        }
        else
        {
            Debug.LogError("No Grid");
        }
    }

    /// <summary>
    /// 屏幕坐标系转为TileMap坐标(取整)
    /// </summary>
    /// <param name="vecScreen"></param>
    /// <returns></returns>
    public Vector2Int MousePosScreenToTile(Vector3 vecScreen)
    {
        if (camFollow != null)
        {
            Vector3 vecWorld = camFollow.GetComponent<Camera>().ScreenToWorldPoint(vecScreen);
            Vector3Int vecCell = GridMap.WorldToCell(vecWorld);
            //Debug.LogFormat("Mouse Move Tile : {0}, {1}", vecWorld.ToString("F4"), vecCell.ToString());
            return new Vector2Int(vecCell.x, vecCell.y);
        }
        return Vector2Int.zero;
    }

    public SceneMapItem GetScenarioParam()
    {
        return param;
    }

#if UNITY_EDITOR
    // 调试场景,加载调试器以启动游戏管理器
    private bool isSimulatedScene = false;
    void OnGUI()
    {
        if (!isSimulatedScene)
        {
            if (GUI.Button(new Rect(0, 0, 100, 40), "SimulateScene"))
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/DevelopmentModeLoader.prefab", typeof(GameObject));
                if (prefab)
                {
                    var goInstance = GameObject.Instantiate(prefab);
                }
                isSimulatedScene = true;
            }
        }
    }
#endif

    private int curBattleFieldId = 0;
    /// <summary>
    /// 由SceneMapManager在场景加载完成后记录当前的战场ID,用于后续状态机驱动加载战场对战双方角色信息
    /// </summary>
    /// <param name="battleFieldID"></param>
    public void LoadBattleFieldDataAndRes(int battleFieldID)
    {
        var battileFieldParam = SceneMapManager.Instance.GetBattleFieldById(battleFieldID);
        if (battileFieldParam != null)
        {
            var mapRes = GameAssetBundlesManager.Instance.LoadSpriteSync(battileFieldParam.BattleFieldAssetBundle,
                battileFieldParam.BattleFieldRes);
            if (mapRes != null)
            {
                UpdateBattleMapBackgroundResource(mapRes);
                // 重新设置相机跟随的最大范围
                var cameraFollow = SetCameraLimitMapSize();
                if (cameraFollow != null)
                {
                    cameraFollow.ResizeCameraMaxMoveSize();
                }
            }
        }

        curBattleFieldId = battleFieldID;
    }

    public int GetCurrentBattleFieldId()
    {
        return curBattleFieldId;
    }

    // 动态Colliders是指场景中某些根据剧情而开启或者关闭的阻挡物,例如
    // 1.八卦阵的入门闪光墙
    // 2.大地图最终决战之前某些栅栏
    #region World Dynamic Environment Colliders

    // 所有的动态Colliders默认是关闭的
    public List<ScenarioDynCollider> envColliders;

    public void WorldSceneSwitchOnColliders(List<string> colliders)
    {
        if (envColliders == null || envColliders.Count == 0)
        {
            Debug.Log("No dynamic colliders in this world scene");
            return;
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            var name = colliders[i];
            var foundCollider = envColliders.Find(x => name.CompareTo(x.ColliderName) == 0);
            if (foundCollider != null)
            {
                if (foundCollider.Collider != null)
                    foundCollider.Collider.enabled = true;
                else
                {
                    Debug.LogFormat("On Environment '{0}' not has Collider or no need to has Collider", name);
                }

                for (int j = 0; j < foundCollider.AttachedAnims.Count; j++)
                {
                    foundCollider.AttachedAnims[j].gameObject.SetActive(true);
                }
            }
        }
    }

    public void WorldSceneSwitchOffColliders(List<string> colliders)
    {
        if (envColliders == null || envColliders.Count == 0)
        {
            Debug.Log("No dynamic colliders in this world scene");
            return;
        }

        for (int i = 0; i < colliders.Count; i++)
        {
            var name = colliders[i];
            var foundCollider = envColliders.Find(x => name.CompareTo(x.ColliderName) == 0);
            if (foundCollider != null)
            {
                if (foundCollider.Collider)
                    foundCollider.Collider.enabled = false;
                else
                {
                    Debug.LogFormat("Off Environment '{0}' not has Collider or no need to has Collider", name);
                }

                for (int j = 0; j < foundCollider.AttachedAnims.Count; j++)
                {
                    foundCollider.AttachedAnims[j].gameObject.SetActive(false);
                }
            }
        }
    }

    public void WorldSceneOpenAnim(List<string> collidersWithParam)
    {
        if (envColliders == null || envColliders.Count == 0)
        {
            Debug.Log("No dynamic colliders in this world scene");
            return;
        }

        if (collidersWithParam.Count == 2)
        {
            var colliderName = collidersWithParam[0];
            var animName = collidersWithParam[1];
            var foundCollider = envColliders.Find(x => colliderName.CompareTo(x.ColliderName) == 0);
            if (foundCollider != null)
            {
                for (int j = 0; j < foundCollider.AttachedAnims.Count; j++)
                {
                    foundCollider.AttachedAnims[j].gameObject.SetActive(true);
                    foundCollider.AttachedAnims[j].Play(animName);
                }
            }
        }
    }
    #endregion

    public List<NPCMainController> HiddenObjects;
}
