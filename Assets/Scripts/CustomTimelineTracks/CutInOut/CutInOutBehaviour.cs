using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class CutInOutBehaviour : PlayableBehaviour
{

    public int CutInOutNoteID;

    public bool hasToPause = false;

    private bool clipPlayed = false;
    private bool pauseScheduled = false;
    private PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!clipPlayed && info.weight > 0f)
        {
            if (Application.isPlaying)
            {
                ScenarioManager.Instance.ShowCutInOutNoteById(CutInOutNoteID);
                if (hasToPause)
                {
                    pauseScheduled = true;
                }
            }

            clipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (pauseScheduled)
        {
            pauseScheduled = false;
            ScenarioManager.Instance.PauseTimelineByNote(director);
        }
        else
        {
            // TODO: Unity编辑模式下访问单例模式会出现问题.因此加入模式判断.
            if (Application.isPlaying)
                ScenarioManager.Instance.HideCutInOutNotes();
        }

        clipPlayed = false;
    }
}
