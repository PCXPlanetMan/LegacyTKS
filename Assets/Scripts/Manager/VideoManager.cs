using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayerLogo1;
    public VideoPlayer videoPlayerLogo2;
    public VideoPlayer videoPlayerOpening;

    private VideoPlayer videoPlayerCurrent = null;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0, 100) > 50)
        {
            videoPlayerLogo1.gameObject.SetActive(true);
            videoPlayerCurrent = videoPlayerLogo1;
            
        }
        else
        {
            videoPlayerLogo2.gameObject.SetActive(true);
            videoPlayerCurrent = videoPlayerLogo2;
        }

        videoPlayerCurrent.isLooping = true;
        videoPlayerCurrent.loopPointReached += EndReached;
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        vp.Stop();
        if (vp == videoPlayerOpening)
        {
            vp.gameObject.SetActive(false);
            EnterMainUI();
            return;
        }
        else
        {
            PlayOpening();
        }
        vp.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && videoPlayerCurrent != null)
        {
            if (videoPlayerCurrent != videoPlayerOpening)
            {
                videoPlayerCurrent.Stop();
                videoPlayerCurrent.gameObject.SetActive(false);
                videoPlayerCurrent = null;
                PlayOpening();
            }
            else
            {
                videoPlayerCurrent.Stop();
                videoPlayerCurrent.gameObject.SetActive(false);
                videoPlayerCurrent = null;
                EnterMainUI();
            }
        }
    }

    private void PlayOpening()
    {
        videoPlayerOpening.gameObject.SetActive(true);
        if (videoPlayerCurrent != null)
            videoPlayerCurrent.loopPointReached -= EndReached;
        videoPlayerCurrent = videoPlayerOpening;
        videoPlayerCurrent.isLooping = true;
        videoPlayerCurrent.loopPointReached += EndReached;
    }

    private void EnterMainUI()
    {
        UIGameRootCanvas uiRoot = GameMainManager.Instance.UIRoot;
        if (uiRoot != null)
        {
            uiRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.MainTitle);
            CursorManager.Instance.ShowCursor(true);
            AudioManager.Instance.PlayMainUIBackgroundMusic();
        }
    }
}
