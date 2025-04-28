using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public struct TimeLineStopped
{
        
}

public struct TimeLineStarted
{
        
}

public class TimeLineSetting : MonoBehaviour
{
        private PlayableDirector playableDirector;

        private void Start()
        {
                playableDirector = GetComponent<PlayableDirector>();
                playableDirector.played += director =>
                {
                        EventManager.Instance.SendEvent<TimeLineStopped>();
                };
                playableDirector.stopped += director =>
                {
                        EventManager.Instance.SendEvent<TimeLineStopped>();
                };
        }
        
}
