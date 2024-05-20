using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CutInOutClip : PlayableAsset, ITimelineClipAsset
{
    public CutInOutBehaviour template = new CutInOutBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CutInOutBehaviour>.Create(graph, template);

        return playable;
    }
}
