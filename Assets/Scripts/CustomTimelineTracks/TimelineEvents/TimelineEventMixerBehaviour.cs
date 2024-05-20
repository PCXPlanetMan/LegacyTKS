using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace TKSPlayables
{
    public class TimelineEventMixerBehaviour : PlayableBehaviour
    {
        public Dictionary<string, double> markerClips;
        private PlayableDirector director;

        public override void OnPlayableCreate(Playable playable)
        {
            director = (playable.GetGraph().GetResolver() as PlayableDirector);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            
        }
    }
}