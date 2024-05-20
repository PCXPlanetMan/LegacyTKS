using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenarioEntry : MonoBehaviour
{
    public int entryId;

    public Transform BornPosition;

    public List<int> PossibleScenarios;

    private SceneEntryItem param = null;

    public TaskHandler HandleScenarioTask;

    void Awake()
    {
        HandleScenarioTask.objId = entryId;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (param == null)
            LoadEntryParam();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetConnectToEntryId()
    {
        if (param != null)
            return param.ConnectToEntry;
        else
            return 0;
    }

    public void SwitchToNextScenarioByEntry()
    {
        if (param != null)
        {
            int connectToEntryId = param.ConnectToEntry;
            if (SceneMapManager.Instance == null)
                return;
            var scenarioParam = SceneMapManager.Instance.GetScenarioParamByEntryId(connectToEntryId);
            GameMainManager.Instance.NextSceneId = scenarioParam.Id;
            GameMainManager.Instance.NextEntryId = connectToEntryId;
            Debug.Log("SwitchToNextScenarioByEntry, NextSceneId = " + scenarioParam.Id + ", NextEntryId = " + connectToEntryId);
            GameMainManager.Instance.SwitchToNextScene();
        }
    }

    public void LoadEntryParam()
    {
        param = SceneMapManager.Instance.GetEntryParamById(entryId);
    }
}
