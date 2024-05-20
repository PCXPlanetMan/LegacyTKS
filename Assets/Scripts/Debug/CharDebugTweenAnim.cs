using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;

public class CharDebugTweenAnim : MonoBehaviour
{
    public DOTweenPath path;

    // Start is called before the first frame update
    void Start()
    {
        var myPathTween = path.GetTween();
        myPathTween.OnWaypointChange(WPCallback);
    }

    void WPCallback(int waypointIndex)
    {
        Debug.Log("wayPointIndex = " + waypointIndex);
        if (waypointIndex > 0)
        {
            path.DOPause();
            pauseDuration = 2f;
            isWayPointPause = true;
        }
        
    }


#if UNITY_EDITOR
    void OnGUI()
    {

        if (GUI.Button(new Rect(0, Screen.height - 60, 100, 40), "CharPath"))
        {
            
        }
    }
#endif

    public void OnClickTest()
    {
        path.DOPlay();
    }

    bool isWayPointPause = false;
    float pauseDuration = 2f;
    private void Update()
    {
        if (!isWayPointPause)
            return;

        pauseDuration -= Time.deltaTime;
        if (pauseDuration <= 0)
        {
            path.DOPlay();
            isWayPointPause = false;
        }
    }
}
