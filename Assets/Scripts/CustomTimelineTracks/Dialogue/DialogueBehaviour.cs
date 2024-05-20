using System;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    [Serializable]
    public class DialogParam
    {
        public int DialogID;
        [BeginReadOnlyGroup]
        [TextArea(1, 10)]
        public string Text;
        [EndReadOnlyGroup]
        public int ShowSide; // 0,left;1,right;2,os
    }
    public DialogParam[] DialogsParam;

    public bool hasToPause = false;

	private bool clipPlayed = false;
	private bool pauseScheduled = false;
	private PlayableDirector director;

	public override void OnPlayableCreate(Playable playable)
	{
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if (Application.isEditor && !Application.isPlaying)
        {

        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		if(!clipPlayed && info.weight > 0f && DialogsParam != null)
		{
            string[] dialogsParam = new string[DialogsParam.Length];
            for (int i = 0; i < dialogsParam.Length; i++)
            {
                dialogsParam[i] =
                    SchemaParser.JoinIntsToString(new int[] {DialogsParam[i].DialogID, DialogsParam[i].ShowSide});
            }

            if (Application.isPlaying)
                ScenarioManager.Instance.ShowUIDialogs(dialogsParam);

        
            if (hasToPause)
            {
                pauseScheduled = true;
            }

            clipPlayed = true;
		}
    }
    
    public override void OnGraphStart(Playable playable)
    {
        //Debug.Log("OnGraphStart");
    }

    public override void OnGraphStop(Playable playable)
    {

    }
  
    public override void OnBehaviourPause(Playable playable, FrameData info)
	{
        if (clipPlayed)
        {
            if (pauseScheduled)
            {
                pauseScheduled = false;
                if (Application.isPlaying)
                {
                    int dlgHasSelection = 0;
                    int count = DialogsParam.Length;
                    if (count > 0)
                    {
                        int[] allDlgsId = new int[count];
                        for (int i = 0; i < allDlgsId.Length; i++)
                        {
                            allDlgsId[i] = DialogsParam[i].DialogID;
                        }

                        dlgHasSelection = ScenarioManager.Instance.HasSelectionsDialogContent(allDlgsId);
                    }

                    if (dlgHasSelection > 0)
                    {
                        ScenarioManager.Instance.PauseTimelineBySelectionDialog(director, dlgHasSelection);
                    }
                    else
                    {
                        ScenarioManager.Instance.PauseTimelineByDialog(director);
                    }
                }
            }
            else
            {
                if (Application.isPlaying)
                    ScenarioManager.Instance.HideUIDialogs();
            }
        }

        clipPlayed = false;
    }
}
