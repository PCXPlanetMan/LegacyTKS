using System.Collections;
using System.Collections.Generic;
using com.tksr.data;
using com.tksr.schema;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{
    [HideInInspector]
    public int objId;

    #region Task
    private List<StoryTaskItem> stackTasks = new List<StoryTaskItem>();

    public void InitTaskData(int taskItemId)
    {
        var taskItem = ScenarioManager.Instance.GetTaskItemParamById(taskItemId);
        if (taskItem != null)
        {
            stackTasks.Add(taskItem);
        }
    }

    public void PushTaskData(int taskItemId, bool clear = true)
    {
        if (clear)
        {
            stackTasks.Clear();
        }

        var newTask = DocumentDataManager.Instance.UpdateScenarioTaskById(taskItemId);
        if (newTask != null)
        {
            stackTasks.Add(newTask);
        }
    }

    public StoryTaskItem PopTaskData()
    {
        if (stackTasks.Count > 0)
        {
            var resultTask = stackTasks[stackTasks.Count - 1];
            DocumentDataManager.Instance.ClearScenarioTaskById(resultTask.Id);
            stackTasks.RemoveAt(stackTasks.Count - 1);
            return resultTask;
        }

        return null;
    }

    public StoryTaskItem FrontTaskData()
    {
        if (stackTasks.Count > 0)
        {
            var resultTask = stackTasks[0];
            DocumentDataManager.Instance.ClearScenarioTaskById(resultTask.Id);
            stackTasks.RemoveAt(0);
            return resultTask;
        }

        return null;
    }

    public StoryTaskItem GetFrontTaskData()
    {
        if (stackTasks.Count > 0)
        {
            return stackTasks[0];
        }

        return null;
    }

    public bool HasDoingTask()
    {
        //return stackTasks.Count > 0 ? true : false;

        if (stackTasks.Count > 0)
        {
            var firstTask = stackTasks[0];
            var scenarioStatus = DocumentDataManager.Instance.GetGameScenarioStatus(firstTask.BelongScenario);
            if (scenarioStatus == EnumScenarioStatus.Running)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion
}
