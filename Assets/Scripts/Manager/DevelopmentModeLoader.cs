using com.tksr.data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// 固定数据加载
/// </summary>
public class DevelopmentModeLoader : MonoBehaviour
{
    private int currentSceneID = 0;
    private int currentEntryID = 0;

    private bool inited = false;

    IEnumerator Start()
    {
        // 单独启动当前场景时可以自动创建所需的单例全局管理器
        if (GameMainManager.Instance != null)
        {
            var sceneMapManager = SceneMapManager.Instance;
            var cursorManager = CursorManager.Instance;
            var scenarioManager = ScenarioManager.Instance;
            var textsManager = TextsManager.Instance;
            var charactersManager = CharactersManager.Instance;
            var documentDataManager = DocumentDataManager.Instance;
            var inputManager = InputManager.Instance;
            // 组织音频控制器
            var audioManager = AudioManager.Instance;
            if (audioManager != null)
            {
                GameObject goBackground = new GameObject(GameConstData.GAME_TAG_GO_AUDIO_BACKGROUND);
                AudioSource background = goBackground.AddComponent<AudioSource>();
                goBackground.transform.parent = audioManager.transform;
                audioManager.Background = background;


                GameObject goEffect = new GameObject(GameConstData.GAME_TAG_GO_AUDIO_EFFECT);
                AudioSource effect = goEffect.AddComponent<AudioSource>();
                goEffect.transform.parent = audioManager.transform;
                audioManager.Effect = effect;
            }  

            // 确保所有AssetBundles正确加载
            var gameAssetBundlesManager = GameAssetBundlesManager.Instance;
            while (!gameAssetBundlesManager.InitFinished)
            {
                yield return new WaitForSeconds(0.1f);
            }

            //GameMainManager.Instance.UIRoot = GameObject.FindObjectOfType<UIGameRootCanvas>();
            GameMainManager.Instance.UIRoot = UIGameRootCanvas.Instance;

            // 地图上出入口信息加载
            var entriesContainer = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainEntries.ToString());
            if (entriesContainer != null)
            {
                var entryGameObjects = entriesContainer.gameObject.GetComponentsInChildren<ScenarioEntry>(true);
                List<ScenarioEntry> entries = new List<ScenarioEntry>();
                foreach (var go in entryGameObjects)
                {
                    var entry = go.gameObject.GetComponent<ScenarioEntry>();
                    Debug.LogFormat("Entry {0}, name = {1}", entry.entryId, go.name);
                    if (entry.entryId == 0)
                        continue;
                    entry.LoadEntryParam();
                    entries.Add(entry);
                }
                if (entries.Count > 0)
                {
                    int rand = Random.Range(0, entries.Count);
                    Debug.LogFormat("Found entry with rand = {0}", rand);
                    var randEntry = entries[rand];
                    currentEntryID = randEntry.entryId;
                }
                else
                {
                    Debug.LogWarning("Not found entries");
                }
            }

            // 根据当前场景ID,重组存档中的已完成剧情列表,从而正确测试当前场景的剧情或事件
            sceneMapManager.SimulateLoadScenarioMap();
            currentSceneID = sceneMapManager.GetCurrentSceneId();
            var scenarioEventIDList = sceneMapManager.GetScenariosIDList(currentSceneID);
            foreach (var scenarioId in scenarioEventIDList)
            {
                var item = scenarioManager.GetScenarioItemById(scenarioId);
                if (item != null)
                    currentScenarios.Add(scenarioId, new DevScenarioData() { ScenarioName = item.Name, GUIDevScenarioFinished = false });
            }
        }

        inited = true;
    }

    private class DevScenarioData
    {
        public string ScenarioName;
        public bool GUIDevTargetScenario;
        public bool GUIDevScenarioFinished;
    }

    private Dictionary<int, DevScenarioData> currentScenarios = new Dictionary<int, DevScenarioData>();
    private string strCurScenarioInfo = null;
    private bool isSelectedScenario = false;
    private string devCharScenarioId = string.Empty;

    void OnGUI()
    {
        if (!inited)
        {
            return;
        }

        int scenarioUIWidth = 120;
        int scenarioUIHeight = 40;

        if (!isSelectedScenario)
        {
            int index = 0;
            if (currentScenarios.Count > 0)
            {
                bool running = GUI.Button(new Rect(0, index * scenarioUIHeight, scenarioUIWidth, scenarioUIHeight), "模拟");
                if (running)
                {
                    List<int> devFinishedScenarios = new List<int>();
                    int toRunningDevScenarioId = 0;
                    DevScenarioData toRunningDevScenarioData = null;
                    foreach (var kv in currentScenarios)
                    {
                        if (kv.Value.GUIDevTargetScenario)
                        {
                            toRunningDevScenarioId = kv.Key;
                            toRunningDevScenarioData = kv.Value;
                            break;
                        }
                    }
                    foreach (var kv in currentScenarios)
                    {
                        if (kv.Value.GUIDevScenarioFinished)
                        {
                            devFinishedScenarios.Add(kv.Key);
                        }
                    }

                    if (devFinishedScenarios.Count == currentScenarios.Count)
                    {
                        currentScenarios.Clear();
                        return;
                    }

                    if (toRunningDevScenarioId == 0)
                    {
                        strCurScenarioInfo = "No Scenario";
                    }
                    else
                    {
                        strCurScenarioInfo = string.Format("SceneScenarioID:{0} -- {1}", toRunningDevScenarioId, toRunningDevScenarioData.ScenarioName);
                    }

                    GameMainManager.Instance.SimulateLoadSceneByEventScenario(currentSceneID, currentEntryID, toRunningDevScenarioId);
                    isSelectedScenario = true;
                }
                index++;

                int lastClickScenarioID = 0;
                foreach (var kv in currentScenarios)
                {
                    int id = kv.Key;
                    DevScenarioData data = kv.Value;

                    bool oldToggle = data.GUIDevTargetScenario;
                    data.GUIDevTargetScenario = GUI.Toggle(new Rect(0, index * scenarioUIHeight, scenarioUIWidth, scenarioUIHeight), data.GUIDevTargetScenario, data.ScenarioName);
                    bool newToggle = data.GUIDevTargetScenario;
                    if (oldToggle == false && newToggle == true)
                    {
                        lastClickScenarioID = id; 
                        Debug.LogFormat("lastClickScenarioID = {0}", lastClickScenarioID);
                    }

                    data.GUIDevScenarioFinished = GUI.Toggle(new Rect(scenarioUIWidth, index * scenarioUIHeight, scenarioUIWidth, scenarioUIHeight), data.GUIDevScenarioFinished, "Done");
                    index++;
                }

                // Target的优先级最高.如果设置当前剧情为Target,则必定不能备选为Finished
                foreach (var kv in currentScenarios)
                {
                    int id = kv.Key;
                    DevScenarioData data = kv.Value;

                    if (data.GUIDevTargetScenario)
                    {
                        data.GUIDevScenarioFinished = false;
                    }

                    if (lastClickScenarioID != id && lastClickScenarioID != 0)
                    {
                        data.GUIDevTargetScenario = false;
                    }
                }
            }
            else // 没有场景触发的剧情,只有NPC或物品触发的剧情
            {
                bool running = GUI.Button(new Rect(0, index * scenarioUIHeight, scenarioUIWidth, scenarioUIHeight), "模拟");
                if (running)
                {
                    if (SceneManager.GetActiveScene().name == "Battle_Demo")
                    {
                        currentEntryID = 5001;
                    }
                    
                    GameMainManager.Instance.SimulateSceneLoadedOfOther(currentSceneID, currentEntryID);
                    isSelectedScenario = true;
                }
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(strCurScenarioInfo))
                GUI.Label(new Rect(0, 0, 300, scenarioUIHeight), strCurScenarioInfo);
        }
    }
}
