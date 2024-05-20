using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TKSPlayables
{
    [TrackColor(0f, 0.4866645f, 1f)]
    [TrackClipType(typeof(FaceToTargetClip))]
    [TrackBindingType(typeof(IGameCharacterRenderer))]
    public class FaceToTargetTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var c in GetClips())
            {
                //Clips are renamed after the actionType of the clip itself
                FaceToTargetClip clip = (FaceToTargetClip) c.asset;
                c.displayName = clip.faceType.ToString();
            }

            return ScriptPlayable<FaceToTargetMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
