using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.tksr.property;
using com.tksr.schema;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class NPCMainController : IGameCharacterRenderer
{
    private List<int> actionList = new List<int>();
    private bool isMovementPause = false;

    #region Tween Path Animation
    public List<DOTweenPath> TweenPaths;
    private bool isTweenStatic = false;
    private EnumDirection curStaticDeployDirection;
    private EnumAnimAction curStaticDeployAnim;
    private float curDeployAnimSpeed = 1f;
    private Tween myPathTween;
    private int lastPathWayPointIndex = -1;
    private List<Vector3> pathWayPoints = new List<Vector3>();
    private float tweenWayPointPauseDuration = 2f;
    private bool isWayPointPause = false;
    private float pauseDuration = 0f;
    private bool sameStartEndWP = false;
    private DeployInfo curDeployInfo = null;

    private void ReInitPathData()
    {
        isMovementPause = false;
        isTweenStatic = false;
        curDeployAnimSpeed = 1f;
        lastPathWayPointIndex = -1;
        pathWayPoints.Clear();
        tweenWayPointPauseDuration = 2f;
        isWayPointPause = false;
        pauseDuration = 0f;
        sameStartEndWP = false;
        curDeployInfo = null;
        foundPath = null;
    }

    private int curPathId = 0;
    private DOTweenPath foundPath = null;

    public int GetCurrentTweenPathId()
    {
        return curPathId;
    }

    public void RunTweenPathById(int pathId)
    {
        curPathId = pathId;
        ReInitPathData();

        if (TweenPaths.Count == 0)
        {
            Debug.LogWarningFormat("No Tween Path data");
            return;
        }

        pauseDuration = tweenWayPointPauseDuration;
        foundPath = TweenPaths.Find(x => x.id.CompareTo(pathId.ToString()) == 0);
        if (foundPath != null)
        {
            var deployInfo = foundPath.gameObject.GetComponent<DeployInfo>();
            tweenWayPointPauseDuration = deployInfo.InterPauseTime;

            curDeployInfo = deployInfo;

            if (curDeployInfo != null && curDeployInfo.DialogSighId > 0)
            {
                ScenarioManager.Instance.AnimationDisplayDialogSigh(curDeployInfo.DialogSighId, true);
            }

            // 如果Tween路径点只有一个,则说明人物没有运动而是停止在原地
            if (foundPath.wps.Count == 1)
            {
                var firstWp = foundPath.wps[0];
                this.transform.position = firstWp;

                if (deployInfo != null)
                {
                    curStaticDeployDirection = deployInfo.CharDirection;
                    curStaticDeployAnim = deployInfo.ActionInScene;
                    curDeployAnimSpeed = deployInfo.AnimSpeed;
                    LoadRenderInScenario(curStaticDeployDirection, curStaticDeployAnim, curDeployAnimSpeed);

                    // TODO:人物自说自话也需要做一个间断动画,并且在与主角说话后要能恢复这个自说自话
                    if (deployInfo != null && deployInfo.DialogSighId > 0)
                    {
                        ScenarioManager.Instance.ShowUIDialogById(deployInfo.DialogSighId, UIDialogPlayer.EnumUIDialogShowMode.ShowSigh);
                    }
                }
                else
                {
                    Debug.LogWarningFormat("Char {0} tween path count == 1, static in scene, but not found DeployInfo", this.CharID);
                }

                isTweenStatic = true;
                isMovementPause = true;
                return;
            }

            if (deployInfo != null)
            {
                curDeployAnimSpeed = deployInfo.AnimSpeed;
            }

            {
                var firstWp = foundPath.wps[0];
                this.transform.position = firstWp;
                pathWayPoints.Add(firstWp);
            }
            
            // 起点和终点一致
            float distance = Vector3.Distance(this.transform.position, foundPath.wps[foundPath.wps.Count - 1]);
            if (distance < float.Epsilon)
            {
                sameStartEndWP = true;
            }

            Vector3 oldFirstVec = foundPath.wps[0];
            foundPath.wps.RemoveAt(0);

            pathWayPoints.AddRange(foundPath.wps);

            OnTweenPathPlay();

            myPathTween = this.transform.DOPath(foundPath.wps.ToArray(), foundPath.duration, foundPath.pathType, foundPath.pathMode).SetOptions(foundPath.isClosedPath, AxisConstraint.None, foundPath.lockRotation);
            myPathTween.SetLoops(foundPath.loops, foundPath.loopType).SetEase(foundPath.easeType);
            myPathTween.OnWaypointChange(WPCallback);
            myPathTween.OnPlay(OnTweenPathPlay);
            myPathTween.OnPause(OnTweenPathPause);
            myPathTween.OnComplete(OnTweenPathCompleted);
            myPathTween.Play();

            foundPath.wps.Insert(0, oldFirstVec);
        }
    }

    public void PauseCurrentTweenPath()
    {
        if (isTweenStatic)
            return;
        myPathTween.Pause();
        isMovementPause = true;
    }

    public void ResumeCurrentTweenPath()
    {
        if (isTweenStatic)
        {
            LoadRenderInScenario(curStaticDeployDirection, curStaticDeployAnim, curDeployAnimSpeed);
            return;
        }

        myPathTween.Play();
        isMovementPause = false;
    }

    private void OnTweenPathPlay()
    {
        //Debug.Log("This is OnPlay");
        int startIndex = 0;
        int nextIndex = 0;
        if (lastPathWayPointIndex == -1)
        {
            nextIndex = 1;
            startIndex = 0;
        }
        else
        {
            startIndex = lastPathWayPointIndex;
            nextIndex = (startIndex + 1) % pathWayPoints.Count;
        }
        

        Vector2 startPos = new Vector2(pathWayPoints[startIndex].x, pathWayPoints[startIndex].y);
        Vector2 nextPos = new Vector2(pathWayPoints[nextIndex].x, pathWayPoints[nextIndex].y);
        Vector2 vecDirection = nextPos - startPos;
        Debug.LogFormat("Tween Path Play, start = {0}|{1}, next = {2}|{3}", startIndex, startPos, nextIndex, nextPos);
        MoveFaceToByDirection(vecDirection, curDeployAnimSpeed);
    }

    private void OnTweenPathPause()
    {
        StandHere();
        // 人物自说自话也需要做一个间断动画,并且在与主角说话后要能恢复这个自说自话
        if (curDeployInfo != null && curDeployInfo.DialogSighId > 0)
        {
            ScenarioManager.Instance.AnimationDisplayDialogSigh(curDeployInfo.DialogSighId, false);
        }
    }

    private void OnTweenPathCompleted()
    {
        // 非Loop的TweenPath在走到终点后停止
        // 并且尝试启动有可以执行的Task
        if (foundPath.loops == 0 && curDeployInfo != null)
        {
            StandHere();
            if (curDeployInfo.LoopEndTask != 0)
            {
                int taskId = curDeployInfo.LoopEndTask;
                var postTask = ScenarioManager.Instance.GetTaskItemParamById(taskId);
                var handleTask = ScenarioManager.Instance.ParseObjScenarioTaskHandler(postTask);
                if (handleTask != null)
                {
                    handleTask.PushTaskData(postTask.Id);
                }
            }
        }
    }

    private void WPCallback(int waypointIndex)
    {
        Debug.Log("wayPointIndex = " + waypointIndex);
        if (waypointIndex > 0)
        {
            myPathTween.Pause();
            pauseDuration = tweenWayPointPauseDuration;
            isWayPointPause = true;
        }
        else if (waypointIndex == 0 && lastPathWayPointIndex != -1 && sameStartEndWP == true)
        {
            myPathTween.Pause();
            pauseDuration = tweenWayPointPauseDuration;
            isWayPointPause = true;
        }

        if (isWayPointPause == true)
        {
            if (curDeployInfo != null && curDeployInfo.DeployPauseDirections.Count > 0)
            {
                if (waypointIndex >= 0 && waypointIndex < curDeployInfo.DeployPauseDirections.Count)
                {
                    var pauseDirection = curDeployInfo.DeployPauseDirections[waypointIndex];
                    StandFaceToByEnumDirection(pauseDirection);
                }
            }
        }

        lastPathWayPointIndex = waypointIndex;
    }


    void Update()
    {
        if (isMovementPause)
            return;

        if (!isWayPointPause)
            return;

        pauseDuration -= Time.deltaTime;
        if (pauseDuration <= 0)
        {
            myPathTween.Play();
            isWayPointPause = false;

            // 人物自说自话也需要做一个间断动画,并且在与主角说话后要能恢复这个自说自话
            if (curDeployInfo != null && curDeployInfo.DialogSighId > 0)
            {
                ScenarioManager.Instance.AnimationDisplayDialogSigh(curDeployInfo.DialogSighId, true);
            }
        }
    }

    void OnDisable()
    {
        if (myPathTween != null)
        {
            myPathTween.Kill();
        }
    }

    public void DestroyPathTween()
    {
        if (myPathTween != null)
        {
            myPathTween.Kill();
            myPathTween = null;
        }
        ReInitPathData();
    }
    #endregion
}
