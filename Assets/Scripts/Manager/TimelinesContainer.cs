using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelinesContainer : MonoBehaviour
{ 
    public List<TimelineScenarioItem> Scenarios;

    /// <summary>
    /// 一般剧情结束后需要暂停所有Timeline,这样才可以去控制主角(否则主角受控于Timeline将会始终停留在Timeline最后一帧的状态)
    /// </summary>
    public void HideAllGameEventIdentities()
    {
        foreach (var e in Scenarios)
        {
            e.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 播放场景中Timeline管理器下记录的事件Timeline动画
    /// </summary>
    /// <param name="scenarioId"></param>
    /// <returns></returns>
    public bool PlayTimelineByScenarioId(int scenarioId)
    {
        var foundEventGO = Scenarios.Find(x => x.ScenarioId == scenarioId);
        if (foundEventGO != null)
        {
            foundEventGO.gameObject.SetActive(true);
            var director = foundEventGO.GetComponent<PlayableDirector>();
            if (director != null)
            {
                director.Play();
                return true;
            }
        }
        else
        {
            Debug.LogWarningFormat("Can't find Event GameObject under Timeline by SceneId = {0}", scenarioId);
        }

        return false;
    }

    public bool PlayTimelineByScenarioName(string scenarioName)
    {
        var foundEventGO = Scenarios.Find(x => x.name.CompareTo(scenarioName) == 0);
        if (foundEventGO != null)
        {
            foundEventGO.gameObject.SetActive(true);
            var director = foundEventGO.GetComponent<PlayableDirector>();
            if (director != null)
            {
                director.Play();
                return true;
            }
        }
        else
        {
            Debug.LogWarningFormat("Can't find Event GameObject under Timeline by SceneName = {0}", scenarioName);
        }

        return false;
    }

    /// <summary>
    /// 每当剧情动画结束后如果PostAction是FreeActivity且参数为0,则意味着需要自己手动清除当前剧情动画中所用的所有NPC人物的动画
    /// </summary>
    /// <param name="scenarioId"></param>
    public void DisableCharsNPCUsedInTimeline(int scenarioId)
    {
        var foundEventGO = Scenarios.Find(x => x.ScenarioId == scenarioId);
        if (foundEventGO != null)
        {
            foundEventGO.UnloadAllLoadedNPCsInTimeline();
        }
    }
}
