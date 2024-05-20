using com.tksr.data;
using com.tksr.property;
using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineScenarioItem : MonoBehaviour
{
    [HideInInspector]
    public int ScenarioId;

    [SerializeField] private List<GameObject> listNPC;
    [SerializeField] private List<GameObject> listTeam;

    private PlayableDirector playableDirector;
    private TimelineAsset someTimelineAsset;

    private List<PlayableDirector> subPlayableDirectors;

    // Start is called before the first frame update
    void Start()
    {
        playableDirector = this.gameObject.GetComponent<PlayableDirector>();
        if (playableDirector != null)
        {
            someTimelineAsset = (TimelineAsset)playableDirector.playableAsset;
            if (someTimelineAsset != null)
            {
                for (int i = 0; i < someTimelineAsset.outputTrackCount; i++)
                {
                    DebugTrack(i);
                }
            }
        }

        // 剧情动画启动之前需加载角色资源
        CharactersManager.Instance.ScenarioLoadCharsGameObjects(listNPC, listTeam);

        var allChildren = this.gameObject.GetComponentsInChildren<PlayableDirector>(true);
        subPlayableDirectors = new List<PlayableDirector>(allChildren);
    }

    void DebugTrack(int trackIndex)
    {
        // Get track from TimelineAsset
        TrackAsset someTimelineTrackAsset = someTimelineAsset.GetOutputTrack(trackIndex);

        // Change TimelineAsset's muted property value
        //someTimelineTrackAsset.muted = !someTimelineTrackAsset.muted;
        Debug.LogFormat("i = {0}, name = {1}", trackIndex, someTimelineTrackAsset.name);

        //double t = playableDirector.time; // Store elapsed time
        //playableDirector.RebuildGraph(); // Rebuild graph
        //playableDirector.time = t; // Restore elapsed time
    }

    public void UnloadAllLoadedNPCsInTimeline()
    {
        if (listNPC.Count == 0)
            return;

        CharactersManager.Instance.ScenarioUnloadNPCGameObjects(listNPC);
    }
    #region Timeline Events

    public void TimelinePlaceHold()
    {

    }

    /// <summary>
    /// Timeline运行到Story开启时执行的动作(目前只是记录事件ID)
    /// </summary>
    public void TimelineBeginStory(string strParam)
    {
        // 防止剧情中主角的Collider与场景发生碰撞
        if (CharactersManager.Instance.MainRoleController != null)
        {
            CharactersManager.Instance.MainRoleController.SwitchManualControlled(false);
        }

        var funcList = SchemaParser.ParseParamToFunctionsMultiParam(strParam);
        foreach (var func in funcList)
        {
            var funcName = func.FuncName;
            var funcParams = func.FuncParams;

            EnumStoryActionType enumPreAction = (EnumStoryActionType)Enum.Parse(typeof(EnumStoryActionType), funcName);
            ScenarioManager.Instance.TimelineActionWhenScenarioBegin(enumPreAction, funcParams);
        }
    }

    /// <summary>
    /// 在Timeline播放Story的时候显示/隐藏鼠标
    /// </summary>
    /// <param name="bShow"></param>
    public void TimelineShowCursor(bool bShow)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Mobile platform doesn't support Cursor.");
        }
        else
        {
            CursorManager.Instance.ShowCursor(bShow);
        }
    }

    /// <summary>
    /// Timeline运行到Story结束的时候执行的操作
    /// </summary>
    public void TimelineEndStory(string strParam)
    {
        if (string.IsNullOrEmpty(strParam))
        {
            EnumStoryActionType enumPostAction = EnumStoryActionType.None;
            ScenarioManager.Instance.TimelineActionWhenScenarioEnd(enumPostAction, null);
        }
        else
        {
            var funcList = SchemaParser.ParseParamToFunctionsMultiParam(strParam);
            foreach (var func in funcList)
            {
                var funcName = func.FuncName;
                var funcParams = func.FuncParams;

                EnumStoryActionType enumPostAction = (EnumStoryActionType)Enum.Parse(typeof(EnumStoryActionType), funcName);
                ScenarioManager.Instance.TimelineActionWhenScenarioEnd(enumPostAction, funcParams);
            }
        }

        ScenarioManager.Instance.EndDoingTaskTimeline();
    }

    /// <summary>
    /// 当人物被给予物品同时显示Toast(有些物品直接装备到人物上)
    /// </summary>
    /// <param name="strParam"></param>
    /// Toast字符串ID,物品显示Toast类型,物品ID(或者是金钱数目),装备到某人物ID身上(若无人装备,则为0或者无值)
    /// eg. 10003,TKRItem,80600001,0
    /// eg. 0,IntCourageMor,10|-10|-10
    /// eg. 11007,Text,0
    public void TimelineItemGivenWithToast(string strParam)
    {
        var toastParams = SchemaParser.ParseParamToStringList(strParam);
        if (toastParams.Count >= 3)
        {
            int toastID = int.Parse(toastParams[0]);
            EnumToastType toastType = (EnumToastType)Enum.Parse(typeof(EnumToastType), toastParams[1]);
            
            if (toastType == EnumToastType.TKRItem)
            {
                int itemParam = int.Parse(toastParams[2]);
                ScenarioManager.Instance.ShowToastHint(toastID, toastType, itemParam);
                DocumentDataManager.Instance.AddTKRItemToPackage(itemParam);
                if (toastParams.Count >= 4)
                {
                    int itemToCharID = int.Parse(toastParams[3]);
                    if (itemToCharID > 0)
                        DocumentDataManager.Instance.EquipTKRItemToChar(itemParam, itemToCharID);
                }
            }
            else if (toastType == EnumToastType.Money)
            {
                int itemParam = int.Parse(toastParams[2]);
                ScenarioManager.Instance.ShowToastHint(toastID, toastType, itemParam);
                int moneyOffset = itemParam;
                DocumentDataManager.Instance.UpdateGoldByValue(moneyOffset);
            }
            else if (toastType == EnumToastType.IntCourageMor)
            {
                var listParams = SchemaParser.ParseParamToInts(toastParams[2], SchemaParser.SPLITTER_PAIR);
                if (listParams.Count == 3)
                {
                    int nInt = listParams[0];
                    int nCour = listParams[1];
                    int nMor = listParams[2];
                    ScenarioManager.Instance.ShowBottomLeftToastForICM(nInt, nCour, nMor);
                }
            }
            else if (toastType == EnumToastType.Text)
            {
                ScenarioManager.Instance.ShowMiddleScenarioText(toastID);
            }
        }
        else
        {
            Debug.LogWarningFormat("Timeline toast format error. {0}", strParam);
        }
    }

    /// <summary>
    /// 剧情动画中显示UI
    /// </summary>
    /// <param name="strUIMode"></param>
    public void TimelineShowUI(string strUIMode)
    {
        EnumTinyGameUI littleGameUI = EnumTinyGameUI.Invalid;
        if (Enum.TryParse(strUIMode, out littleGameUI))
        {
            switch (littleGameUI)
            {
                case EnumTinyGameUI.InputName:
                {
                    PlayableDirector director = this.gameObject.GetComponent<PlayableDirector>();
                    if (director != null)
                    {
                        ScenarioManager.Instance.PauseTimelineByInput(director);
                        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.LittleGames_InputName);
                    }
                }
                    break;
                case EnumTinyGameUI.PuzzleBaGua:
                {
                    PlayableDirector director = this.gameObject.GetComponent<PlayableDirector>();
                    if (director != null)
                    {
                        ScenarioManager.Instance.PauseTimelineByInput(director);
                        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.LittleGames_PuzzleBgGua, UICallbackPuzzleBaGua);
                    }
                }
                    break;
                default:
                    break;
            }
        }
    }

    public void TimelinePlayMusic(int nMusicId)
    {
        //Debug.Log("Music = " + nMusicId);
        AudioManager.Instance.PlayEffectFromBackground(nMusicId);
    }

    public void TimelinePlayBGM(int nBGMId)
    {
        AudioManager.Instance.PlayScenarioBackgroundMusic(nBGMId);
    }

    public void TimelinePlaySubPlayables(string subs)
    {
        if (subPlayableDirectors != null)
        {
            var subPlayables = SchemaParser.ParseParamToStringList(subs);
            for (int i = 0; i < subPlayables.Count; i++)
            {
                var playable = subPlayables[i];
                var found = subPlayableDirectors.Find(x => playable.CompareTo(x.name) == 0);
                if (found != null)
                {
                    found.gameObject.SetActive(true);
                    found.Play();
                }
            }
        }
    }

    /// <summary>
    /// 单个角色切换Sub循环动画
    /// </summary>
    /// <param name="subs"></param>
    public void TimelineSwitchSubPlayables(string subs)
    {
        if (subPlayableDirectors != null)
        {
            var subPlayables = SchemaParser.ParseParamToStringList(subs);
            if (subPlayables.Count == 2)
            {
                var oldSub = subPlayables[0];
                var newSub = subPlayables[1];
                var foundNew = subPlayableDirectors.Find(x => newSub.CompareTo(x.name) == 0);
                if (foundNew != null)
                {
                    foundNew.gameObject.SetActive(true);
                    foundNew.Play();
                }
                var foundOld = subPlayableDirectors.Find(x => oldSub.CompareTo(x.name) == 0);
                if (foundOld != null)
                {
                    foundOld.Stop();
                    foundOld.gameObject.SetActive(false);
                }
            }
        }
    }

    public void TimelineStopSubPlayables(string subs)
    {
        if (subPlayableDirectors != null)
        {
            var subPlayables = SchemaParser.ParseParamToStringList(subs);
            for (int i = 0; i < subPlayables.Count; i++)
            {
                var playable = subPlayables[i];
                var found = subPlayableDirectors.Find(x => playable.CompareTo(x.name) == 0);
                if (found != null)
                {
                    found.Stop();
                    found.gameObject.SetActive(false);
                }
            }
        }
    }

    public void TimelineMakeCharSortingLayerAbove(string strParam)
    {
        var charIdList = SchemaParser.ParseParamToInts(strParam);
        for (int i = 0; i < charIdList.Count; i++)
        {
            int id = charIdList[i];
            CharactersManager.Instance.UpdateCharSpriteSortingLayer(id, ResourceUtils.SORTING_LAYER_OF_SCENARIO_SUB_TIMELINE_ANIM);
        }
    }

    public void TimelineResetCharSortingLayer(string strParam)
    {
        var charIdList = SchemaParser.ParseParamToInts(strParam);
        for (int i = 0; i < charIdList.Count; i++)
        {
            int id = charIdList[i];
            CharactersManager.Instance.UpdateCharSpriteSortingLayer(id, ResourceUtils.SORTING_LAYER_OF_CHARACTER);
        }
    }

    public void TimelineSetSelectionDialogID(string strParam)
    {
        int dialogID = int.Parse(strParam);
        ScenarioManager.Instance.SetLastScenarioDialogID(dialogID);
    }

    public void TimelineManualSetCameraFollow(string strParam)
    {
        int followTargetCharId = int.Parse(strParam);
        ScenarioManager.Instance.SetCameraFollowTargetCharacter(followTargetCharId);
    }

    #endregion

    #region UI Callback Functions

    private void UICallbackPuzzleBaGua(bool uiResult)
    {
        if (uiResult)
        {
            DocumentDataManager.Instance.CloseScenarioById(ScenarioId);
        }
    }

    #endregion
}
